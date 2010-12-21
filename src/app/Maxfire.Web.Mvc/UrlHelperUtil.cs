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
	}
}