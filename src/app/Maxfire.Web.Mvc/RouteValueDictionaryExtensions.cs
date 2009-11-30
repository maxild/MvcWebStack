using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Routing;
using Maxfire.Core.Extensions;

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

		public static void Merge(this RouteValueDictionary mergedRouteValues, RouteValueDictionary routeValues)
		{
			if (routeValues != null)
			{
				foreach (KeyValuePair<string, object> kvp in routeValues)
				{
					mergedRouteValues[kvp.Key] = kvp.Value;
				}
			}
		}

		public static void AddValuesWithDotNotation(this RouteValueDictionary routeValues, object values)
		{
			routeValues.AddValuesWithDotNotation(values, null);
		}

		public static void AddValuesWithDotNotation(this RouteValueDictionary routeValues, object values, string prefix)
		{
			if (values != null)
			{
				addValuesCore(routeValues, values, prefix);
			}
		}

		private static void addValuesCore(this IDictionary<string, object> dictionary, object obj, string prefix)
		{
			foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(obj))
			{
				object val = descriptor.GetValue(obj);
				string name = descriptor.Name;
				if (!string.IsNullOrEmpty(prefix))
					name = prefix + "." + name;
				if (descriptor.PropertyType.IsSimpleType())
				{
					// Todo: Use TypeConverter (see IsSimpleType)
					// Todo: TryConvertSimpleType(val, out newValue)
					if (!descriptor.IsReadOnly)
					{
						dictionary.Add(name, val);
					}
				}
				else if (descriptor.PropertyType.IsArray)
				{
					// arrays are ignored (todo: what about List<> etc.)
				}
				else
				{
					dictionary.addValuesCore(val, name);
				}
			}
		}
	}
}