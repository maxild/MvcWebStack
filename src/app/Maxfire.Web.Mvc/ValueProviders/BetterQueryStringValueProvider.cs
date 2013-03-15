using System.Globalization;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Maxfire.Web.Mvc.ValueProviders
{
	public class BetterQueryStringValueProvider : BetterNameValueCollectionValueProvider
	{
		public BetterQueryStringValueProvider(ControllerContext controllerContext, CultureInfo culture = null)
			: base(controllerContext.HttpContext.Request.QueryString, controllerContext.HttpContext.Request.Unvalidated().QueryString, culture ?? CultureInfo.InvariantCulture)
		{
		}
	}
}