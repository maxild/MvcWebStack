using System;
using System.Linq;

namespace Maxfire.Core.Extensions
{
	public static class StringExtensions
	{
		public static string FormatWith(this string format, object arg0)
		{
			return string.Format(format, arg0);
		}

		public static string FormatWith(this string format, object arg0, object arg1)
		{
			return string.Format(format, arg0, arg1);
		}

		public static string FormatWith(this string format, object arg0, object arg1, object arg2)
		{
			return string.Format(format, arg0, arg1, arg2);
		}

		public static string FormatWith(this string format, params object[] args)
		{
			return string.Format(format, args);
		}

		public static string FormatWith(this string format, IFormatProvider provider, params object[] args)
		{
			return string.Format(provider, format, args);
		}

		public static bool IsEmpty(this string stringValue)
		{
			return String.IsNullOrEmpty(stringValue);
		}

		public static bool IsNotEmpty(this string stringValue)
		{
			return !String.IsNullOrEmpty(stringValue);
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

	}
}