using System.Collections.Generic;

namespace Maxfire.Web.Mvc
{
	public interface INameValueSerializer
	{
		IDictionary<string, object> GetValues(object model, string prefix);
	}
}