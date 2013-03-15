using System.Globalization;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Maxfire.Web.Mvc
{
	public class BetterFormValueProvider : BetterNameValueCollectionValueProvider
	{
		public BetterFormValueProvider(ControllerContext controllerContext)
			: base(controllerContext.HttpContext.Request.Form, controllerContext.HttpContext.Request.Unvalidated().Form, CultureInfo.CurrentCulture)
		{
		}
	}
}