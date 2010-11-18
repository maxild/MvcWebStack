using System.Web.Routing;

namespace Maxfire.Web.Mvc
{
	public interface IUrlHelper
	{
		string GetVirtualPath(RouteValueDictionary routeValues);
		string ApplicationPath { get; }
		INameValueSerializer NameValueSerializer { get; }
	}
}