using System.Web.Mvc;

namespace Maxfire.Web.Mvc
{
	public static class HtmlHelperExtensions
	{
		public static string SiteResource(this HtmlHelper htmlHelper, string path)
		{
			IUrlHelper urlHelper = htmlHelper.ViewContext.View as IUrlHelper;
			if (urlHelper != null)
			{
				return urlHelper.SiteResource(path);
			}
			return UrlHelper.GenerateContentUrl(path, htmlHelper.ViewContext.HttpContext);
		}
	}
}