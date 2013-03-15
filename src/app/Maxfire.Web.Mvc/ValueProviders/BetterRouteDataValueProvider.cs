using System.Globalization;
using System.Web.Mvc;

namespace Maxfire.Web.Mvc.ValueProviders
{
	public sealed class BetterRouteDataValueProvider : BetterDictionaryValueProvider<object>
	{
		public BetterRouteDataValueProvider(ControllerContext controllerContext)
			: base(controllerContext.RouteData.Values, CultureInfo.InvariantCulture)
		{
		}
	}
}