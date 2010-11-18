using System.Collections.Generic;

namespace Maxfire.Web.Mvc
{
	public static class NameValueSerializerExtensions
	{
		public static IDictionary<string, object> GetValues(this INameValueSerializer serializer, object model)
		{
			return serializer.GetValues(model, string.Empty);
		}
	}
}