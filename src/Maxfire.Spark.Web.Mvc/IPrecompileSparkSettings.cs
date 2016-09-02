using System;
using Spark;

namespace Maxfire.Spark.Web.Mvc
{
    [CLSCompliant(false)]
    public interface IPrecompileSparkSettings
	{
		ISparkSettings SparkSettings { get; }
		string ViewsAssemblyFile { get; }
	}
}
