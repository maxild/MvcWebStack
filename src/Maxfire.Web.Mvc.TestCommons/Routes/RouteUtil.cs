using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Rhino.Mocks;

namespace Maxfire.Web.Mvc.TestCommons.Routes
{
	public static class RouteUtil
	{
		/// <summary>
		/// Used to establish a fake context when doing URL generation.
		/// </summary>
		/// <returns>Fake request context</returns>
		public static RequestContext GetRequestContext()
		{
			var httpContext = MockRepository.GenerateMock<HttpContextBase>();
			var request = MockRepository.GenerateMock<HttpRequestBase>();
			var response = MockRepository.GenerateMock<HttpResponseBase>();

			response.Stub(x => x.ApplyAppPathModifier(Arg<string>.Is.Anything)).Do(new Func<string, string>(x => x));
			httpContext.Stub(x => x.Request).Return(request);
			httpContext.Stub(x => x.Response).Return(response);

			var routeData = new RouteData();

			return new RequestContext(httpContext, routeData);
		}

		/// <summary>
		/// Used to establish a fake context when doing URL recognition of a GET request.
		/// </summary>
		/// <param name="url">The virtual path to recognize</param>
		/// <returns>Fake http context</returns>
		public static HttpContextBase GetHttpContext(string url)
		{
			return GetHttpContext(url, HttpVerbs.Get);
		}

		/// <summary>
		/// Used to establish a fake context when doing URL recognition of a HTTP request.
		/// </summary>
		/// <param name="url">The virtual path to recognize</param>
		/// <param name="httpMethod">The HTTP verb used</param>
		/// <param name="formMethod">The form pseudo HTTP verb</param>
		/// <returns>Fake http context</returns>
		public static HttpContextBase GetHttpContext(string url, HttpVerbs httpMethod, HttpVerbs? formMethod = null)
		{
			url = PrepareUrl(url);

			var context = MockRepository.GenerateMock<HttpContextBase>();
			var request = MockRepository.GenerateMock<HttpRequestBase>();

			// GetHttpMethodOverride requires non-null/empty QueryString, Form and Headers, if HttpMethod is POST
			var empty = new NameValueCollection();

			context.Stub(x => x.Request).Return(request).Repeat.Any();
			request.Stub(x => x.QueryString).Return(GetRequestParams(url)).Repeat.Any();
			request.Stub(x => x.AppRelativeCurrentExecutionFilePath).Return(GetRequestPath(url)).Repeat.Any();
			request.Stub(x => x.PathInfo).Return(string.Empty).Repeat.Any();
			request.Stub(x => x.HttpMethod).Return(httpMethod.ToString().ToUpperInvariant()).Repeat.Any();
			request.Stub(x => x.Headers).Return(empty).Repeat.Any();

			if (formMethod != null)
			{
				var form = new NameValueCollection {{"_method", formMethod.ToString().ToUpperInvariant()}};
				request.Stub(r => r.Form).Return(form).Repeat.Any();
			}
			else
			{
				request.Stub(r => r.Form).Return(empty).Repeat.Any();
			}

			return context;
		}

		private static string GetRequestPath(string url)
		{
			if (url.Contains("?"))
				return url.Substring(0, url.IndexOf("?", StringComparison.Ordinal));

			return url;
		}

		private static NameValueCollection GetRequestParams(string url)
		{
			var parameters = new NameValueCollection();

			if (url.Contains("?"))
			{

				string[] parts = url.Split("?".ToCharArray());
				string[] keys = parts[1].Split("&".ToCharArray());

				foreach (string key in keys)
				{
					string[] part = key.Split("=".ToCharArray());
					parameters.Add(part[0], part[1]);
				}
			}

			return parameters;
		}

		private static string PrepareUrl(string url)
		{
			if (!url.StartsWith("~"))
			{
				if (url.StartsWith("/"))
				{
					return "~" + url;
				}
				return "~/" + url;
			}
			return url;
		}
	}
}
