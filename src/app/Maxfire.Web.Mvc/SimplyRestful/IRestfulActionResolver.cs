//////////////////////////////////////////////////////////////////////
// The SimplyRestful routes are contributed by Adam Tybor, see
// http://abombss.com/blog/2007/12/10/ms-mvc-simply-restful-routing/
//////////////////////////////////////////////////////////////////////
using System.Web.Routing;

namespace Maxfire.Web.Mvc.SimplyRestful
{
	public interface IRestfulActionResolver
	{
		RestfulAction ResolveAction(RequestContext context);
	}
}