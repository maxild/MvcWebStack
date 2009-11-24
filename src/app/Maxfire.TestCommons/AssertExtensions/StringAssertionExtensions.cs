using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Sdk;

namespace Maxfire.TestCommons.AssertExtensions
{
	/// <summary>
	/// Extensions which provide assertions to classes derived from <see cref="String"/>.
	/// </summary>
	public static class StringAssertionExtensions
	{
		public static string ShouldBeEmpty(this string s)
		{
			Assert.True(String.IsNullOrEmpty(s));
			return s;
		}

		public static string ShouldNotBeEmpty(this string s)
		{
			Assert.False(String.IsNullOrEmpty(s));
			return s;
		}

		/// <summary>
		/// Verifies that a string contains a given sub-string, using the current culture.
		/// </summary>
		/// <param name="actualString">The string to be inspected</param>
		/// <param name="expectedSubString">The sub-string expected to be in the string</param>
		/// <exception cref="ContainsException">Thrown when the sub-string is not present inside the string</exception>
		public static string ShouldContain(this string actualString,
		                                   string expectedSubString)
		{
			Assert.Contains(expectedSubString, actualString);
			return actualString;
		}

		/// <summary>
		/// Verifies that a string contains a given sub-string, using the given comparison type.
		/// </summary>
		/// <param name="actualString">The string to be inspected</param>
		/// <param name="expectedSubString">The sub-string expected to be in the string</param>
		/// <param name="comparisonType">The type of string comparison to perform</param>
		/// <exception cref="ContainsException">Thrown when the sub-string is not present inside the string</exception>
		public static string ShouldContain(this string actualString,
		                                   string expectedSubString,
		                                   StringComparison comparisonType)
		{
			Assert.Contains(expectedSubString, actualString, comparisonType);
			return actualString;
		}

		/// <summary>
		/// Verifies that a string does not contain a given sub-string, using the current culture.
		/// </summary>
		/// <param name="actualString">The string to be inspected</param>
		/// <param name="expectedSubString">The sub-string which is expected not to be in the string</param>
		/// <exception cref="DoesNotContainException">Thrown when the sub-string is present inside the string</exception>
		public static string ShouldNotContain(this string actualString,
		                                      string expectedSubString)
		{
			Assert.DoesNotContain(expectedSubString, actualString);
			return actualString;
		}

		/// <summary>
		/// Verifies that a string does not contain a given sub-string, using the current culture.
		/// </summary>
		/// <param name="actualString">The string to be inspected</param>
		/// <param name="expectedSubString">The sub-string which is expected not to be in the string</param>
		/// <param name="comparisonType">The type of string comparison to perform</param>
		/// <exception cref="DoesNotContainException">Thrown when the sub-string is present inside the given string</exception>
		public static string ShouldNotContain(this string actualString,
		                                      string expectedSubString,
		                                      StringComparison comparisonType)
		{
			Assert.DoesNotContain(expectedSubString, actualString, comparisonType);
			return actualString;
		}

		public static string ShouldEqual(this string actualString, string expectedString)
		{
			Assert.Equal(expectedString, actualString);
			return actualString;
		}

		public static string ShouldEqual(this string actualString, string expectedString, params object[] formatArgs)
		{
			Assert.Equal(string.Format(expectedString, formatArgs), actualString);
			return actualString;
		}

		public static string ShouldEqual(this string actualString, string expectedString, IComparer<string> comparer)
		{
			Assert.Equal(expectedString, actualString, comparer);
			return actualString;
		}
	}
}