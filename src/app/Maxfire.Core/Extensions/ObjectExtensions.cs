using System;

namespace Maxfire.Core.Extensions
{
	public static class ObjectExtenions
	{
		/// <summary>
		/// Get a (possibly null) string representation of a possible null object.
		/// </summary>
		public static string ToNullString(this object value)
		{
			return value == null ? null : value.ToString();
		}

		/// <summary>
		/// Get a non-null string representation of a possible null object.
		/// </summary>
		public static string ToNullSafeString(this object value)
		{
			return value == null ? String.Empty : value.ToString();
		}

		/// <summary>
		/// Get a (possibly null) string representation of a possible null object.
		/// </summary>
		public static string ToNullString(this object value, IFormatProvider formatProvider)
		{
			return value == null ? null : Convert.ToString(value, formatProvider);
		}

		/// <summary>
		/// Get a non-null string representation of a possible null object.
		/// </summary>
		public static string ToNullSafeString(this object value, IFormatProvider formatProvider)
		{
			return Convert.ToString(value, formatProvider);
		}

		public static bool IsTrimmedEmpty(this object fieldValue)
		{
			return fieldValue.ToTrimmedNullSafeString().Length == 0;
		}

		public static bool IsTrimmedNotEmpty(this object fieldValue)
		{
			return fieldValue.ToTrimmedNullSafeString().Length != 0;
		}

		public static string ToTrimmedNullSafeString(this object fieldValue)
		{
			return fieldValue == null ? string.Empty : fieldValue.ToString().Trim();
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