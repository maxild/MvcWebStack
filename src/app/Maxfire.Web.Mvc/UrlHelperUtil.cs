using System.Web.Routing;

namespace Maxfire.Web.Mvc
{
	public static class UrlHelperUtil
	{
		public static string GetVirtualPath(RouteCollection routes, RequestContext requestContext, RouteValueDictionary routeValues)
		{
			VirtualPathData vpd = routes.GetVirtualPath(requestContext, routeValues);
			if (vpd != null)
			{
				return vpd.VirtualPath;
			}
			return null;
		}

		public static string GetApplicationPath(RequestContext requestContext)
		{
			return requestContext.HttpContext.Request.ApplicationPath;
		}

		public static string GetSiteRoot(RequestContext requestContext)
		{
			var appPath = GetApplicationPath(requestContext);
			string siteRoot;
			// SiteRoot acts as a safe prefix to relative url
			if (string.IsNullOrEmpty(appPath) || string.Equals(appPath, "/"))
			{
				siteRoot = string.Empty;
			}
			else
			{
				siteRoot = "/" + appPath.Trim('/');
			}
			return siteRoot;
		}
	}
}