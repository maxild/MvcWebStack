﻿using System;
using System.Reflection;
using System.Web.Mvc;
using Spark;
using Spark.FileSystem;
using Spark.Web.Mvc;

namespace Maxfire.Spark.Web.Mvc
{
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
				throw new ArgumentNullException("settingsProvider");
			}
			if (describeBatchHandler == null)
			{
				throw new ArgumentNullException("describeBatchHandler");
			}
			_settingsProvider = settingsProvider;
			_describeBatch = describeBatchHandler;
		}

		public ISparkSettings SparkSettings
		{
			get { return getPrecompileSettings().SparkSettings; }
		}

		public string ViewsAssemblyFile
		{
			get { return getPrecompileSettings().ViewsAssemblyFile; }
		}

		public bool Debug
		{
			get { return SparkSettings.Debug; }
		}

		public void RegisterViewEngine(ViewEngineCollection engines)
		{
			engines.Add(SparkEngineStarter.CreateViewEngine(SparkSettings));
		}

		public void DescribeBatch(SparkBatchDescriptor batch)
		{
			_describeBatch(batch);
		}

		private IPrecompileSparkSettings getPrecompileSettings()
		{
			return _precompileSettings ?? (_precompileSettings = _settingsProvider());
		}

		/// <summary>
		/// Call this method at application startup time to load all precompiled views
		/// </summary>
		public void LoadPrecompiledViews()
		{
			var engine = new SparkViewEngine(SparkSettings)
			             	{
			             		DefaultPageBaseType = typeof (SparkView).FullName
			             	};
			engine.LoadBatchCompilation(Assembly.Load(ViewsAssemblyFile));
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
			DescribeBatch(batch);
			var factory = new SparkViewFactory(SparkSettings)
			              	{
			              		ViewFolder = new FileSystemViewFolder(viewsLocation)
			              	};
			return factory.Precompile(batch);
		}
	}
}