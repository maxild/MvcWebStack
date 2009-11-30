using System.Web.Mvc;

namespace Maxfire.Web.Mvc
{
	public interface ITempDataContainer
	{
		TempDataDictionary TempData { get; }
	}
}