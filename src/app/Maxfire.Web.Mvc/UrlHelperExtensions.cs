using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;
using Maxfire.Web.Mvc.FluentHtml.Elements;

namespace Maxfire.Web.Mvc
{
	public static class UrlHelperExtensions
	{
		public static UrlBuilder UrlFor<TController>(this IUrlHelper urlHelper, Expression<Action<TController>> action) where TController : Controller
		{
			return urlHelper.UrlFor(action, null);
		}

		public static UrlBuilder UrlFor<TController>(this IUrlHelper urlHelper,
			Expression<Action<TController>> action,
			object routeValues
			) where TController : Controller
		{
			return urlHelper.UrlFor(action, new RouteValueDictionary().Merge(routeValues));
		}

		public static UrlBuilder UrlFor<TController>(this IUrlHelper urlHelper, 
			Expression<Action<TController>> action, 
			RouteValueDictionary routeValues
			) where TController : Controller
		{
			var mergedRouteValues = RouteValuesHelper.GetRouteValuesFromExpression(action, urlHelper.NameValueSerializer).Merge(routeValues);
			return new UrlBuilder(urlHelper, mergedRouteValues);
		}

		public static Link LinkFor<TController>(this IUrlHelper urlHelper, Expression<Action<TController>> action) where TController : Controller
		{
			var urlBuilder = urlHelper.UrlFor(action);
			return new Link(urlBuilder);
		}

		public static string IncludeCss(this IUrlHelper urlHelper, string cssFile)
		{
			cssFile = GetContentPath("css", cssFile);
			string url = urlHelper.SiteResource(cssFile);
			return string.Format("<link type=\"text/css\" rel=\"stylesheet\" href=\"{0}\" />", url);
		}

		public static string IncludeJS(this IUrlHelper urlHelper, string jsFile)
		{
			return string.Format("<script type=\"text/javascript\" src=\"{0}\"></script>", urlHelper.PathOfJS(jsFile));
		}

		public static string PathOfJS(this IUrlHelper urlHelper, string jsFile)
		{
			jsFile = GetContentPath("js", jsFile);
			string url = urlHelper.SiteResource(jsFile);
			return url;
		}

		public static string PathOfImage(this IUrlHelper urlHelper, string imageFile)
		{
			imageFile = GetContentPath("images", imageFile);
			string url = urlHelper.SiteResource(imageFile);
			return url;
		}

		public static string PathOfStaticHtml(this IUrlHelper urlHelper, string htmlFile)
		{
			htmlFile = GetContentPath("html", htmlFile);
			string url = urlHelper.SiteResource(htmlFile);
			return url;
		}

		public static string ResolveUrl(this IUrlHelper urlHelper, string relativeUrl)
		{
			if (relativeUrl == null || !relativeUrl.StartsWith("~"))
			{
				return relativeUrl;
			}

			string url;
			if (relativeUrl.StartsWith("~/"))
			{
				url = urlHelper.SiteRoot + "/" + relativeUrl.Substring(2);
			}
			else
			{
				url = urlHelper.SiteRoot + "/" + relativeUrl.Substring(1);
			}
			
			return url;
		}

		private static string GetContentPath(string contentSubFolder, string file)
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