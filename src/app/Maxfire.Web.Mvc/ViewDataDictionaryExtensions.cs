using System.Collections.Generic;
using System.Web.Mvc;

namespace Maxfire.Web.Mvc
{
	public static class ViewDataDictionaryExtensions
	{
		public static ViewDataDictionary Merge(this ViewDataDictionary viewData, IDictionary<string, object> values)
		{
			if (values != null)
			{
				foreach (KeyValuePair<string, object> kvp in values)
				{
					viewData[kvp.Key] = kvp.Value;
				}
			}
			return viewData;
		}

		public static ViewDataDictionary Merge(this ViewDataDictionary viewData, string name, object value)
		{
			viewData[name] = value;
			return viewData;
		}

		public static ViewDataDictionary<T> Merge<T>(this ViewDataDictionary<T> viewData, IDictionary<string, object> values)
		{
			if (values != null)
			{
				foreach (KeyValuePair<string, object> kvp in values)
				{
					viewData[kvp.Key] = kvp.Value;
				}
			}
			return viewData;
		}

		public static ViewDataDictionary<T> Merge<T>(this ViewDataDictionary<T> viewData, string name, object value)
		{
			viewData[name] = value;
			return viewData;
		}
	}
}