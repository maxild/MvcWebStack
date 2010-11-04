using System.Collections.Generic;

namespace Maxfire.Web.Mvc
{
	public interface IQueryStringSerializer
	{
		IDictionary<string, object> GetValues(object model, string prefix);
	}
}