using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using Maxfire.Web.Mvc.FluentHtml.Elements;

namespace Maxfire.Web.Mvc
{
	public static class UrlHelperExtensions
	{
		public static UrlBuilder UrlFor<TController>(this IUrlHelper urlHelper, Expression<Action<TController>> action) where TController : Controller
		{
			var routeValues = RouteValuesHelper.GetRouteValuesFromExpression(action, urlHelper.QueryStringSerializer);
			return new UrlBuilder(urlHelper, routeValues);
		}

		public static UrlBuilder UrlFor<TController>(this IUrlHelper urlHelper, Expression<Action<TController>> action, string controllerName) where TController : Controller
		{
			var routeValues = RouteValuesHelper.GetRouteValuesFromExpression(action, urlHelper.QueryStringSerializer, controllerName);
			return new UrlBuilder(urlHelper, routeValues);
		}

		public static Link LinkFor<TController>(this IUrlHelper urlHelper, Expression<Action<TController>> action) where TController : Controller
		{
			var urlBuilder = urlHelper.UrlFor(action);
			return new Link(urlBuilder);
		}

		public static string IncludeCss(this IUrlHelper urlHelper, string cssFile)
		{
			cssFile = getContentPath("css", cssFile);
			string url = urlHelper.ResolveUrl(cssFile);
			return string.Format("<link type=\"text/css\" rel=\"stylesheet\" href=\"{0}\" />", url);
		}

		public static string IncludeJS(this IUrlHelper urlHelper, string jsFile)
		{
			return string.Format("<script type=\"text/javascript\" src=\"{0}\"></script>", urlHelper.PathOfJS(jsFile));
		}

		public static string PathOfJS(this IUrlHelper urlHelper, string jsFile)
		{
			jsFile = getContentPath("js", jsFile);
			string url = urlHelper.ResolveUrl(jsFile);
			return url;
		}

		public static string PathOfImage(this IUrlHelper urlHelper, string imageFile)
		{
			imageFile = getContentPath("images", imageFile);
			string url = urlHelper.ResolveUrl(imageFile);
			return url;
		}

		public static string PathOfStaticHtml(this IUrlHelper urlHelper, string htmlFile)
		{
			htmlFile = getContentPath("html", htmlFile);
			string url = urlHelper.ResolveUrl(htmlFile);
			return url;
		}

		public static string ResolveUrl(this IUrlHelper urlHelper, string relativeUrl)
		{
			if (relativeUrl == null || !relativeUrl.StartsWith("~"))
				return relativeUrl;

			var basePath = urlHelper.ApplicationPath ?? string.Empty;
			if (!basePath.EndsWith("/", StringComparison.OrdinalIgnoreCase))
			{
				basePath += "/";
			}

			string url;
			if (relativeUrl.StartsWith("~/"))
			{
				url = basePath + relativeUrl.Substring(2);
			}
			else
			{
				url = basePath + relativeUrl.Substring(1);
			}
			
			return url;
		}

		private static string getContentPath(string contentSubFolder, string file)
		{
			if (!file.StartsWith("~", StringComparison.OrdinalIgnoreCase))
			{
				if (file.StartsWith("/", StringComparison.OrdinalIgnoreCase))
				{
					file = "~/content/" + contentSubFolder + file;
				}
				else
				{
					file = "~/content/" + contentSubFolder + "/" + file;
				}
			}
			return file;
		}
	}
}