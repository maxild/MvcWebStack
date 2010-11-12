using System;

namespace Maxfire.Core.Extensions
{
	public static class ObjectExtenions
	{
		public static string ToNullSafeString(this object value)
		{
			return value == null ? String.Empty : value.ToString();
		}

		public static object GetPropertyValueOf(this object @object, string propertyName)
		{
			object propertyValue = null;
			var propertyInfo = @object.GetType().GetProperty(propertyName);
			if (propertyInfo != null)
			{
				propertyValue = propertyInfo.GetValue(@object, null);
			}
			return propertyValue;
		}
	}
}