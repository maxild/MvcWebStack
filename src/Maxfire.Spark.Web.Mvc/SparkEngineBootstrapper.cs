using System;
using System.Reflection;
using System.Web.Mvc;
using Spark;
using Spark.FileSystem;
using Spark.Web.Mvc;

namespace Maxfire.Spark.Web.Mvc
{
    [CLSCompliant(false)]
    public class SparkEngineBootstrapper
	{
		private readonly Action<SparkBatchDescriptor> _describeBatch;
		private readonly Func<IPrecompileSparkSettings> _settingsProvider;
		private IPrecompileSparkSettings _precompileSettings;

		public SparkEngineBootstrapper(Func<IPrecompileSparkSettings> settingsProvider,
		                               Action<SparkBatchDescriptor> describeBatchHandler)
		{
			if (settingsProvider == null)
			{
				throw new ArgumentNullException(nameof(settingsProvider));
			}
			if (describeBatchHandler == null)
			{
				throw new ArgumentNullException(nameof(describeBatchHandler));
			}
			_settingsProvider = settingsProvider;
			_describeBatch = describeBatchHandler;
		}

		public ISparkSettings SparkSettings
		{
			get { return GetPrecompileSettings().SparkSettings; }
		}

		public string ViewsAssemblyFile
		{
			get { return GetPrecompileSettings().ViewsAssemblyFile; }
		}

		public bool Debug
		{
			get { return SparkSettings.Debug; }
		}

		public void RegisterViewEngine(ViewEngineCollection engines)
		{
			engines.Add(SparkEngineStarter.CreateViewEngine(SparkSettings));
		}

		private IPrecompileSparkSettings GetPrecompileSettings()
		{
			return _precompileSettings ?? (_precompileSettings = _settingsProvider());
		}

		/// <summary>
		/// Call this method at application startup time to load all precompiled views
		/// </summary>
		public void LoadPrecompiledViews(ISparkSettings settings)
		{
			LoadPrecompiledViews(SparkSettings, ViewsAssemblyFile);
		}

		public static void LoadPrecompiledViews(ISparkSettings settings, string viewsAssemblyFile)
		{
			var engine = new SparkViewEngine(settings)
			{
				DefaultPageBaseType = typeof(SparkView).FullName // Used only if pageBaseType not specified (as a fallback)
			};
			engine.LoadBatchCompilation(Assembly.Load(viewsAssemblyFile));
		}

		/// <summary>
		/// Call this method either in automated tests to verify spark compilation.
		/// </summary>
		/// <param name="viewsLocation">The location of the Views folder (relative 
		/// to the test harness code base directory).</param>
		/// <returns>The precompiled views assembly.</returns>
		public Assembly PrecompileViews(string viewsLocation)
		{
			var batch = new SparkBatchDescriptor();
			_describeBatch(batch);
			var factory = new SparkViewFactory(SparkSettings)
			              	{
			              		ViewFolder = new FileSystemViewFolder(viewsLocation)
			              	};
			return factory.Precompile(batch);
		}
	}
}
