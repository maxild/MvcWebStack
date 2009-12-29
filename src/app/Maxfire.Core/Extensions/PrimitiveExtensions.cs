using System;
using System.Linq;

namespace Maxfire.Core.Extensions
{
	public static class PrimitiveExtensions
	{
		public static bool IsEmpty(this string stringValue)
		{
			return String.IsNullOrEmpty(stringValue);
		}

		public static bool IsNotEmpty(this string stringValue)
		{
			return !String.IsNullOrEmpty(stringValue);
		}

		public static string ToNullSafeString(this object value)
		{
			return value == null ? String.Empty : value.ToString();
		}

		public static string UpperCaseFirstWord(this string s)
		{
			if (s.IsEmpty()) return s;
			return s.Substring(0, 1).ToUpper() + s.Substring(1);
		}

		public static string LowerCaseFirstWord(this string s)
		{
			if (s.IsEmpty()) return s;
			return s.Substring(0, 1).ToLower() + s.Substring(1);
		}

		public static string UpperCaseWords(this string s)
		{
			if (s.IsEmpty()) return s;
			return s.ToCharArray().Aggregate(String.Empty,
			                                 (working, next) =>
			                                 working.Length == 0 && next != ' ' ? next.ToString().ToUpper() : (
			                                                                                                  	working.EndsWith(" ") ? working + next.ToString().ToUpper() :
			                                                                                                  	                                                            	working + next.ToString()
			                                                                                                  )
				);
		}

		public static string LowerCaseWords(this string s)
		{
			if (s.IsEmpty()) return s;
			return s.ToCharArray().Aggregate(String.Empty,
			                                 (working, next) =>
			                                 working.Length == 0 && next != ' ' ? next.ToString().ToLower() : (
			                                                                                                  	working.EndsWith(" ") ? working + next.ToString().ToLower() :
			                                                                                                  	                                                            	working + next.ToString()
			                                                                                                  )
				);
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