using System;

namespace Maxfire.Core.Extensions
{
	public static class ThrowGuardExtension
	{
		public static void ThrowIfNull<T>(this T data, string name) where T : class
		{
			if (data == null)
			{
				throw new ArgumentNullException(name);
			}
		}

		public static void ThrowIfNullOrEmpty(this string s, string name)
		{
			if (s == null)
			{
				throw new ArgumentNullException();
			}
			if (s.Length == 0)
			{
				throw new ArgumentException("The string argument cannot be empty.", name);
			}
		}

		public static void ThrowIfLessThanOrEqualToZero<T>(this T data, string name)
			where T : IComparable<T>
		{
			T zero = default(T);
			if (data.CompareTo(zero) <= 0)
			{
				throw new ArgumentOutOfRangeException(name, data, "The argument cannot be less than or equal to zero.");
			}
		}
	}
}