using Spark;

namespace Maxfire.Spark.Web.Mvc
{
	public interface IPrecompileSparkSettings
	{
		ISparkSettings SparkSettings { get; }
		string ViewsAssemblyFile { get; }
	}
}