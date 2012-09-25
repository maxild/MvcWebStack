using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using Maxfire.Core.Extensions;
using Xunit;
using Xunit.Sdk;

namespace Maxfire.Web.Mvc.TestCommons.Routes
{
	// Todo: If a FakeRoutingContextContainer is injected everything works. Then both routes and mocked httpcontext is contained in this instance
	// Todo: Should be done by a fixture, because this is the context. This way duplicate setting up the routes are removed
	public abstract class RoutesTesterBase : IDisposable
	{
		private readonly INameValueSerializer _nameValueSerializer;
		private readonly IControllerFactory _controllerFactory;

		protected RoutesTesterBase() : this(new DefaultNameValueSerializer())
		{
		}

		protected RoutesTesterBase(INameValueSerializer nameValueSerializer)
		{
			_nameValueSerializer = nameValueSerializer;
			
			// Because when testing Url recognition we have a call to IRouteHandler.GetHttpHandler,
			// and the standard MvcRouteHandler will call IControllerFactory.GetControllerSessionBehavior
			// that will use the 'ASP.NET pipeline' based BuildManager. This will throw during testinf, and
			// therefore we use a fake controller factory.
			
			_controllerFactory = ControllerBuilder.Current.GetControllerFactory();
			ControllerBuilder.Current.SetControllerFactory(new FakeControllerFactory());
		}

		void IDisposable.Dispose()
		{
			ControllerBuilder.Current.SetControllerFactory(_controllerFactory);
		}

		public INameValueSerializer NameValueSerializer
		{
			get { return _nameValueSerializer; }
		}

		private RouteCollection _routes;
		public RouteCollection Routes
		{
			get { return _routes ?? (_routes = GetRoutes()); }
		}

		protected RouteCollection GetRoutes()
		{
			var routes = new RouteCollection();
			RegisterRoutes(routes);
			return routes;
		}

		protected abstract void RegisterRoutes(RouteCollection routes);

		class FakeControllerFactory : IControllerFactory
		{
			public IController CreateController(RequestContext requestContext, string controllerName)
			{
				return null;
			}

			public SessionStateBehavior GetControllerSessionBehavior(RequestContext requestContext, string controllerName)
			{
				return SessionStateBehavior.Default;
			}

			public void ReleaseController(IController controller)
			{
			}
		}

		// Todo: This method should get a better name
		protected void CheckRequestForRestResource(string controllerPath, string controller)
		{
			const string id = "1";
			string usersPath = controllerPath;
			string userPath = usersPath + "/" + id;
			string usersPathNew = usersPath + "/new";
			string userPathEdit = userPath + "/edit";

			GetRequestFor(usersPath)
				.ShouldMatchController(controller)
				.AndAction("Index");

			PostRequestFor(usersPath)
				.ShouldMatchController(controller)
				.AndAction("Create");

			GetRequestFor(usersPathNew)
				.ShouldMatchController(controller)
				.AndAction("New");

			GetRequestFor(userPath)
				.ShouldMatchController(controller)
				.AndAction("Show")
				.AndRouteValue("id", id);

			PutRequestFor(userPath)
				.ShouldMatchController(controller)
				.AndAction("Update")
				.AndRouteValue("id", id);

			DeleteRequestFor(userPath)
				.ShouldMatchController(controller)
				.AndAction("Destroy")
				.AndRouteValue("id", id);

			GetRequestFor(userPathEdit)
				.ShouldMatchController(controller)
				.AndAction("Edit")
				.AndRouteValue("id", id);
		}

		protected UriPathRecognizeOptions GetRequestFor(string url)
		{
			return new UriPathRecognizeOptions(_nameValueSerializer, Routes, url, HttpVerbs.Get);
		}

		protected UriPathRecognizeOptions DeleteRequestFor(string url)
		{
			return new UriPathRecognizeOptions(_nameValueSerializer, Routes, url, HttpVerbs.Delete);
		}

		protected UriPathRecognizeOptions PseudoDeleteRequestFor(string url)
		{
			return new UriPathRecognizeOptions(_nameValueSerializer, Routes, url, HttpVerbs.Post, HttpVerbs.Delete);
		}

		protected UriPathRecognizeOptions PutRequestFor(string url)
		{
			return new UriPathRecognizeOptions(_nameValueSerializer, Routes, url, HttpVerbs.Put);
		}

		protected UriPathRecognizeOptions PseudoPutRequestFor(string url)
		{
			return new UriPathRecognizeOptions(_nameValueSerializer, Routes, url, HttpVerbs.Post, HttpVerbs.Put);
		}

		protected UriPathRecognizeOptions PostRequestFor(string url)
		{
			return new UriPathRecognizeOptions(_nameValueSerializer, Routes, url, HttpVerbs.Post);
		}

		protected UriPathRecognizeOptions RequestFor(string url, HttpVerbs httpMethod, HttpVerbs? formMethod = null)
		{
			return new UriPathRecognizeOptions(_nameValueSerializer, Routes, url, httpMethod, formMethod);
		}

		protected UriPathGenerationOptions UrlGeneration
		{
			get { return new UriPathGenerationOptions(Routes); }
		}

		protected enum UrlGenerationHack
		{
			None,
			MapToActionName
		}

		/// <summary>
		/// Starting point when defining route values to be passed to the URL generation of the routes under test.
		/// </summary>
		/// <typeparam name="TController">The type that identifies the controller.</typeparam>
		/// <param name="action">An expression which identifies the action.</param>
		/// <returns>A strongly typed options hash for the fluent interface</returns>
		protected UriPathGenerationOptions UrlGenerationWith<TController>(Expression<Action<TController>> action)
			where TController : Controller
		{
			return UrlGenerationWith(action, UrlGenerationHack.None);
		}

		/// <summary>
		/// Starting point when defining route values to be passed to the URL generation of the routes under test.
		/// </summary>
		/// <remarks>
		/// Some people think that strongly typed URL generation performed in
		/// the view is a violation of the MVC pattern, because the view should
		/// only think about routes as "string-based url segments" without
		/// thinking about the concrete controller classes and the contained
		/// concrete action methods, that the MVC system will eventually map
		/// the recognized route onto.
		/// 
		/// For testing I think it is important to be able to use strongly typed
		/// URL generation. Even though strongly typed URL generation will not
		/// work when using the ActionName attribute, and maybe should be avoided
		/// in the view. This is especially true if you choose to use strongly-typed
		/// URL generation from your views even though the MVC team is not supporting
		/// this as a best practice.
		/// </remarks>
		/// <typeparam name="TController">The type that identifies the controller.</typeparam>
		/// <param name="action">An expression which identifies the action.</param>
		/// <param name="hack">A value indicating what kind of measures should be taken to retrieve the correct route values.</param>
		/// <returns>A strongly typed options hash for the fluent interface</returns>
		protected UriPathGenerationOptions UrlGenerationWith<TController>(Expression<Action<TController>> action, UrlGenerationHack hack)
			where TController : Controller
		{
			// Read this blog post http://haacked.com/archive/2008/08/29/how-a-method-becomes-an-action.aspx,
			// and carefully read the discussion (in the comments) about the following 2 views:
			//		1) The MVC team is in favour of string based route values (controller, action, parameters, etc.)
			//		2) Others are in favour of strongly typed route values (controller, action, parameters, etc.)
			RouteValueDictionary values = RouteValuesHelper.GetRouteValuesFromExpression(action, _nameValueSerializer);

			// This is a hack to map to the action alias defined by the ActionName attribute
			if (hack == UrlGenerationHack.MapToActionName) values["action"] = getActionName(action);

			return new UriPathGenerationOptions(Routes, values);
		}

		private static string getActionName<TController>(Expression<Action<TController>> action)
		{
			Type controllerType = typeof(TController);
			MethodCallExpression call = action.Body as MethodCallExpression;
			string methodName = call != null ? call.Method.Name : "";
			MethodInfo actionMethodInfo = controllerType.GetMethod(methodName);
			var alias = (ActionNameAttribute[])actionMethodInfo.GetCustomAttributes(typeof(ActionNameAttribute), true);

			return alias.Length == 1 ? alias[0].Name : methodName;
		}

		public class UriPathRecognizeOptions
		{
			private readonly RouteData _routeData;
			private readonly NameValueCollection _queryData;
			private readonly INameValueSerializer _nameValueSerializer;

			public UriPathRecognizeOptions(INameValueSerializer nameValueSerializer, RouteCollection routes, string url, HttpVerbs httpMethod, HttpVerbs? formMethod = null)
			{
				_nameValueSerializer = nameValueSerializer;
				var result = recognizePath(routes, url, httpMethod, formMethod);
				_routeData = result.RouteData;
				_queryData = result.QueryData;
			}

			public UriPathRecognizeOptions ShouldMatchController(string controller)
			{
				assertStringsEquivalent(controller, _routeData.GetRequiredString("controller"));
				return this;
			}

			public UriPathRecognizeOptions ShouldMatchController<TController>()
			{
				return ShouldMatchController(RouteValuesHelper.GetControllerName<TController>());
			}

			public UriPathRecognizeOptions ShouldMatch<TController>(Expression<Action<TController>> action) where TController : Controller
			{
				var routeValues = RouteValuesHelper.GetRouteValuesFromExpression(action, _nameValueSerializer);

				string controllerAsString = routeValues["Controller"].ToString();
				routeValues.Remove("Controller");

				string actionAsString = routeValues["Action"].ToString();
				routeValues.Remove("Action");

				var self = ShouldMatchController(controllerAsString).AndAction(actionAsString);

				foreach (var item in routeValues)
				{
					//if (item.Value != null)
					self.AndRouteValue(item.Key, item.Value.ToNullSafeString());
				}

				return self;
			}

			public UriPathRecognizeOptions AndAction(string action)
			{
				assertStringsEquivalent(action, _routeData.GetRequiredString("action"));
				return this;
			}

			public UriPathRecognizeOptions AndRouteValue(string key, string value)
			{
				var actualValue = _routeData.Values[key];
				if (actualValue == null)
				{
					throw new AssertException(string.Format("Cannot verify route value, because the key '{0}' failed to be recognized.", key));
				}
				assertStringsEquivalent(value, actualValue.ToString());
				return this;
			}

			public UriPathRecognizeOptions AndQueryValue(string key, string value)
			{
				assertStringsEquivalent(value, _queryData[key]);
				return this;
			}

			private static void assertStringsEquivalent(string expected, string actual)
			{
				Assert.Equal(expected, actual, StringComparer.Create(CultureInfo.InvariantCulture, true));
			}

			private static RecognizePathResult recognizePath(RouteCollection routes, string url, HttpVerbs httpMethod, HttpVerbs? formMethod = null)
			{
				HttpContextBase context = RouteUtil.GetHttpContext(url, httpMethod, formMethod);

				var routeData = routes.GetRouteData(context);
				var queryData = context.Request.QueryString;

				Assert.True(routeData != null, String.Format("The url '{0}' was not recognized by any routes.", url));
				
				// The bound RouteHandler can mutate the routeData when getting the http 
				// handler for the request (See SimplyRestfulRouteHandler)
				routeData.RouteHandler.GetHttpHandler(new RequestContext(context, routeData));

				return new RecognizePathResult { RouteData = routeData, QueryData = queryData };
			}

			private struct RecognizePathResult
			{
				public RouteData RouteData { get; set; }
				public NameValueCollection QueryData { get; set; }
			}
		}

		public class UriPathGenerationOptions
		{
			private readonly RouteCollection _routes;
			private readonly RouteValueDictionary _values;

			public UriPathGenerationOptions(RouteCollection routes)
			{
				_routes = routes;
				_values = new RouteValueDictionary();
			}

			public UriPathGenerationOptions(RouteCollection routes, RouteValueDictionary values)
			{
				_routes = routes;
				_values = values;
			}

			public UriPathGenerationOptions WithController(string controller)
			{
				_values["controller"] = controller;
				return this;
			}

			public UriPathGenerationOptions WithController<TController>()
			{
				return WithController(RouteValuesHelper.GetControllerName<TController>());
			}

			public UriPathGenerationOptions AndAction(string action)
			{
				_values["action"] = action;
				return this;
			}

			public UriPathGenerationOptions AndRouteValue(string key, string value)
			{
				_values[key] = value;
				return this;
			}

			public UriPathGenerationOptions AndRouteValues(object values)
			{
				return AndRouteValues(new RouteValueDictionary(values));
			}

			public UriPathGenerationOptions AndRouteValues(RouteValueDictionary values)
			{
				foreach (var kvp in values)
				{
					_values[kvp.Key] = kvp.Value;
				}
				return this;
			}

			public void ShouldGenerateUriPathOf(string path)
			{
				string uriPath = generateUrl(_routes, _values);
				assertStringsEquivalent(path, uriPath);
			}

			public string GenerateUriPathOf()
			{
				string uriPath = generateUrl(_routes, _values);
				return uriPath;
			}

			private static void assertStringsEquivalent(string expected, string actual)
			{
				Assert.Equal(expected, actual, StringComparer.Create(CultureInfo.InvariantCulture, true));
			}

			// Todo: Could use a mocked IUrlHelper instead
			private static string generateUrl(RouteCollection routes, RouteValueDictionary values)
			{
				var requestContext = RouteUtil.GetRequestContext();
				return UrlHelperUtil.GetVirtualPath(routes, requestContext, values);
			}
		}
	}
}