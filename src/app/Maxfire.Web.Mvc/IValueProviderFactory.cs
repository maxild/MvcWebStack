using System.Web.Mvc;

namespace Maxfire.Web.Mvc
{
	public interface IValueProviderFactory
	{
		IValueProvider GetValueProvider(ControllerContext controllerContext);
	}
}