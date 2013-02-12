namespace Maxfire.Web.Mvc
{
	public interface IDataMutator<in T>
	{
		void SetData(string key, T data);
	}
}