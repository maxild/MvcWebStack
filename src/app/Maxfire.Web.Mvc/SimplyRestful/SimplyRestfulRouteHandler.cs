//////////////////////////////////////////////////////////////////////
// The SimplyRestful routes are contributed by Adam Tybor, see
// http://abombss.com/blog/2007/12/10/ms-mvc-simply-restful-routing/
//////////////////////////////////////////////////////////////////////
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Maxfire.Core.Extensions;

namespace Maxfire.Web.Mvc.SimplyRestful
{
	/// <summary>
	/// A Rails inspired Restful Routing Handler
	/// </summary>
	public class SimplyRestfulRouteHandler : MvcRouteHandler
	{
		/// <summary>Matches anything.</summary>
		public const string MatchAny = ".+";

		/// <summary>Matches a base64 encoded <see cref="Guid"/></summary>
		public const string MatchBase64Guid = @"[a-zA-Z0-9+/=]{22,24}";

		/// <summary>Matches a <see cref="Guid"/><c>@"\{?[a-fA-F0-9]{8}(?:-(?:[a-fA-F0-9]){4}){3}-[a-fA-F0-9]{12}\}?"</c></summary>
		public const string MatchGuid = @"\{?[a-fA-F0-9]{8}(?:-(?:[a-fA-F0-9]){4}){3}-[a-fA-F0-9]{12}\}?";

		/// <summary>Matches a Positive <see cref="int"/> <c>@"\d{1,10}"</c></summary>
		public const string MatchPositiveInteger = @"\d{1,10}";

		/// <summary>Matches a Positive <see cref="long"/> <c>@"\d{1,19}"</c></summary>
		public const string MatchPositiveLong = @"\d{1,19}";

		private IRestfulActionResolver _actionResolver;

		public SimplyRestfulRouteHandler()
		{
		}

		public SimplyRestfulRouteHandler(IRestfulActionResolver actionResolver)
		{
			_actionResolver = actionResolver;
		}

		protected override IHttpHandler GetHttpHandler(RequestContext requestContext)
		{
			ensureActionResolver(requestContext.HttpContext);

			RestfulAction action = _actionResolver.ResolveAction(requestContext);
			if (action != RestfulAction.None)
			{
				// Overwrite action with either Update (form _method := PUT) or Destroy (for _method := DELETE)
				requestContext.RouteData.Values["action"] = action.ToString();
			}
			return base.GetHttpHandler(requestContext);
		}

		/// <summary>
		/// Adds the set of SimplyRestful routes to the <paramref name="routes"/>.
		/// By default a positive integer validator is set for the Id parameter of the <see cref="Route.Values"/>.
		/// </summary>
		/// <param name="routes">The route collection to add the routes to.</param>
		/// <seealso cref="BuildRoutes(RouteCollection,string,string,string)"/>
		public static void BuildRoutes(RouteCollection routes)
		{
			BuildRoutes(routes, "{controller}", MatchPositiveInteger, null);
		}

		/// <summary>
		/// Adds the set of SimplyRestful routes to the <paramref name="routes"/>.
		/// </summary>
		/// <param name="routes">The route collection to add the routes to.</param>
		/// <param name="urlPattern">The beginning of the url pattern for the 7 restful routes.</param>
		/// <param name="idValidationRegex">The <see cref="System.Text.RegularExpressions.Regex"/> 
		/// validator to add to the Id parameter of the <see cref="Route.Values"/>, use <c>null</c> to not validate the id.</param>
		/// <param name="controllerName">The name of the controller. Only required if you are trying to route to a specific 
		/// controller using a non-standard url.</param>
		public static void BuildRoutes(RouteCollection routes, string urlPattern, string idValidationRegex, string controllerName)
		{
			urlPattern = fixPath(urlPattern);

			routes.Add(new Route(
				urlPattern,
				buildDefaults(RestfulAction.Index, controllerName),
				new RouteValueDictionary(new { httpMethod = new HttpMethodConstraint("GET") }),
				new MvcRouteHandler()));

			routes.Add(new Route(
				urlPattern + "/new",
				buildDefaults(RestfulAction.New, controllerName),
				new RouteValueDictionary(new { httpMethod = new HttpMethodConstraint("GET") }),
				new MvcRouteHandler()));

			routes.Add(new Route(
				urlPattern + "/{id}",
				buildDefaults(RestfulAction.Show, controllerName),
				new RouteValueDictionary(new { httpMethod = new HttpMethodConstraint("GET"), id = idValidationRegex ?? MatchAny }),
				new MvcRouteHandler()));

			routes.Add(new Route(
				urlPattern + "/{id}/edit",
				buildDefaults(RestfulAction.Edit, controllerName),
				new RouteValueDictionary(new { httpMethod = new HttpMethodConstraint("GET"), id = idValidationRegex ?? MatchAny }),
				new MvcRouteHandler()));

			// POST action with form _method hack are handled here
			routes.Add(new Route(
				urlPattern + "/{id}",
				buildDefaults(RestfulAction.None, controllerName),
				new RouteValueDictionary(new { httpMethod = new HttpMethodConstraint("POST"), id = idValidationRegex ?? MatchAny }),
				new SimplyRestfulRouteHandler()));

			routes.Add(new Route(
				urlPattern + "/{id}",
				buildDefaults(RestfulAction.Update, controllerName),
				new RouteValueDictionary(new { httpMethod = new HttpMethodConstraint("PUT"), id = idValidationRegex ?? MatchAny }),
				new MvcRouteHandler()));

			routes.Add(new Route(
				urlPattern + "/{id}",
				buildDefaults(RestfulAction.Destroy, controllerName),
				new RouteValueDictionary(new { httpMethod = new HttpMethodConstraint("DELETE"), id = idValidationRegex ?? MatchAny }),
				new MvcRouteHandler()));

			routes.Add(new Route(
				urlPattern,
				buildDefaults(RestfulAction.Create, controllerName),
				new RouteValueDictionary(new { httpMethod = new HttpMethodConstraint("POST") }),
				new MvcRouteHandler()));
		}

		/// <summary>Ensures that a <see cref="IRestfulActionResolver"/> exists.</summary>
		/// <param name="serviceProvider">The <see cref="HttpContextBase"/> as an <see cref="IServiceProvider"/> to try and use to resolve an instance of the <see cref="IRestfulActionResolver"/></param>
		/// <remarks>If no <see cref="IRestfulActionResolver"/> can be resolved the default <see cref="RestfulActionResolver"/> is used.</remarks>
		private void ensureActionResolver(IServiceProvider serviceProvider)
		{
			if (_actionResolver == null)
			{
				_actionResolver = (IRestfulActionResolver)serviceProvider.GetService(typeof(IRestfulActionResolver)) ??
				                  new RestfulActionResolver();
			}
		}

		/// <summary>Fixes an area prefix for the route url.</summary>
		/// <param name="path">The area prefix to fix.</param>
		/// <returns>A non null string with leading and trailing /'s stripped</returns>
		private static string fixPath(string path)
		{
			return path == null ? "" : path.Trim().Trim(new[] { '/' });
		}

		/// <summary>Builds a Default object for a route.</summary>
		/// <param name="restfulAction">The default action for the route.</param>
		/// <param name="controllerName">The default controller for the route.</param>
		/// <returns>An Anonymous Type with a default Action property and and default Controller property
		/// if <paramref name="controllerName"/> is not null or empty.</returns>
		private static RouteValueDictionary buildDefaults(RestfulAction restfulAction, string controllerName)
		{
			string action = restfulAction == RestfulAction.None ? "" : restfulAction.ToString();
			return controllerName.IsEmpty() ? 
				new RouteValueDictionary(new { Action = action }) : 
				new RouteValueDictionary(new { Action = action, Controller = controllerName });
		}
	}
}