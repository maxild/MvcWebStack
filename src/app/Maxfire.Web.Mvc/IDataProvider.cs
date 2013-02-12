namespace Maxfire.Web.Mvc
{
	public interface IDataProvider<out T>
	{
		T GetData(string key);
	}
}