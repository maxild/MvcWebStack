using System.Web.Mvc;

namespace Maxfire.Web.Mvc.Html5
{
	public interface IModelStateContainer
	{
		ModelStateDictionary ModelState { get; }
	}
}