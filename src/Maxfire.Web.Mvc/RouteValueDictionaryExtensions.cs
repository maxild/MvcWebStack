using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Routing;

namespace Maxfire.Web.Mvc
{
	public static class RouteValueDictionaryExtensions
	{
		public static string GetRequiredString(this RouteValueDictionary routeValues, string name)
		{
			if (routeValues == null)
			{
				throw new ArgumentNullException("routeValues");
			}
			object value;
			if (routeValues.TryGetValue(name, out value))
			{
				string str = value as string;
				if (!string.IsNullOrEmpty(str))
				{
					return str;
				}
			}
			throw new InvalidOperationException(string.Format("The required key '{0}' cannot be found in route values.", name));
		}

		public static RouteValueDictionary Merge(this RouteValueDictionary routeValues, IDictionary<string, object> values)
		{
			if (values != null)
			{
				foreach (KeyValuePair<string, object> kvp in values)
				{
					routeValues[kvp.Key] = kvp.Value;
				}
			}
			return routeValues;
		}

		public static RouteValueDictionary Merge(this RouteValueDictionary routeValues, object values)
		{
			if (values != null)
			{
				foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(values))
				{
					routeValues[descriptor.Name.ToLowerInvariant()] = descriptor.GetValue(values);
				}
			}
			return routeValues;
		}
	}
}