using Spark;

namespace Maxfire.Spark.Web.Mvc
{
	public class PrecompileSparkSettings : IPrecompileSparkSettings
	{
		public PrecompileSparkSettings(ISparkSettings sparkSettings, string viewsAssemblyFile)
		{
			SparkSettings = sparkSettings;
			ViewsAssemblyFile = viewsAssemblyFile;
		}

		public ISparkSettings SparkSettings { get; private set; }

		public string ViewsAssemblyFile { get; private set; }
	}
}