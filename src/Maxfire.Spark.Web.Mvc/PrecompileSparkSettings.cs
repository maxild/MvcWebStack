using System;
using Spark;

namespace Maxfire.Spark.Web.Mvc
{
    [CLSCompliant(false)]
    public class PrecompileSparkSettings : IPrecompileSparkSettings
	{
        public PrecompileSparkSettings(ISparkSettings sparkSettings, string viewsAssemblyFile)
        {
			SparkSettings = sparkSettings;
			ViewsAssemblyFile = viewsAssemblyFile;
		}

		public ISparkSettings SparkSettings { get; }

		public string ViewsAssemblyFile { get; }
	}
}
