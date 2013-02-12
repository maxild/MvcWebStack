namespace Maxfire.Web.Mvc
{
	public interface IDataProviderAndMutator<T> : IDataProvider<T>, IDataMutator<T>
	{
	}
}