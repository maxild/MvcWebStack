using System.Web.Routing;

namespace Maxfire.Web.Mvc
{
	public interface IUrlHelper
	{
		/// <summary>
		/// Resolve route values to a virtual/relative path.
		/// </summary>
		/// <param name="routeValues">Name-value pairs (aka route values).</param>
		/// <returns>The generated virtual/relative path to be prefixed with '~/' or '${SiteRoot}/'.</returns>
		string GetVirtualPath(RouteValueDictionary routeValues);
		
		/// <summary>
		/// The path of the site root (aka application path).
		/// </summary>
		string SiteRoot { get; }
		
		/// <summary>
		/// Resolve the relative/virtual path to an absolutte URL.
		/// </summary>
		/// <param name="path">The path to resolve</param>
		/// <returns>An absolute URL to the site.</returns>
		string SiteResource(string path);
		
		/// <summary>
		/// Convert a model to name-value pairs to be used as route values (before generating a URL).
		/// </summary>
		INameValueSerializer NameValueSerializer { get; }

		/// <summary>
		/// Gets the route data for the current request.
		/// </summary>
		RouteData RouteData { get; }
	}
}