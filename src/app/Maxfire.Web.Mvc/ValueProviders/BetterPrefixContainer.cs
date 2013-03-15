using System;
using System.Collections.Generic;
using System.Linq;
using Maxfire.Core.Extensions;

namespace Maxfire.Web.Mvc.ValueProviders
{
	/// <summary>
	/// This is a container for prefix values. It normalizes all the values into dotted-form and then stores
	/// them in a sorted array. All queries for prefixes are also normalized to dotted-form, and searches
	/// for ContainsPrefix are done with a binary search.
	/// </summary>
	public class BetterPrefixContainer
	{
		private readonly ICollection<string> _originalValues;
		private readonly string[] _sortedValues;

		public BetterPrefixContainer(ICollection<string> values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}

			_originalValues = values;
			//_sortedValues = values.Where(val => val != null).ToArray();
			// We change the sort order here.....
			_sortedValues = values.Where(val => val != null).Map(s => s.Replace("[", " [")).ToArray();
			Array.Sort(_sortedValues, StringComparer.OrdinalIgnoreCase);
		}

		public bool ContainsPrefix(string prefix)
		{
			if (prefix == null)
			{
				throw new ArgumentNullException("prefix");
			}

			if (prefix.Length == 0)
			{
				return _sortedValues.Length > 0; // only match empty string when we have some value
			}

			return Array.BinarySearch(_sortedValues, prefix, new PrefixComparer(prefix)) > -1;
		}

		// Given "foo.bar", "foo.hello", "something.other", foo[abc].baz and asking for prefix "foo" will return:
		// - "bar"/"foo.bar"
		// - "hello"/"foo.hello"
		// - "abc"/"foo[abc]"
		public IDictionary<string, string> GetKeysFromPrefix(string prefix)
		{
			IDictionary<string, string> result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

			foreach (var entry in _originalValues)
			{
				if (entry != null)
				{
					if (entry.Length == prefix.Length)
					{
						// No key in this entry
						continue;
					}

					if (prefix.Length == 0)
					{
						GetKeyFromEmptyPrefix(entry, result);
					}
					else if (entry.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
					{
						GetKeyFromNonEmptyPrefix(prefix, entry, result);
					}
				}
			}

			return result;
		}

		static void GetKeyFromEmptyPrefix(string entry, IDictionary<string, string> results)
		{
			int dotPosition = entry.IndexOf('.');
			int bracketPosition = entry.IndexOf('[');
			int delimiterPosition = -1;

			if (dotPosition == -1)
			{
				if (bracketPosition != -1)
				{
					delimiterPosition = bracketPosition;
				}
			}
			else
			{
				if (bracketPosition == -1)
				{
					delimiterPosition = dotPosition;
				}
				else
				{
					delimiterPosition = Math.Min(dotPosition, bracketPosition);
				}
			}

			string key = delimiterPosition == -1 ? entry : entry.Substring(0, delimiterPosition);
			results[key] = key;
		}

		static void GetKeyFromNonEmptyPrefix(string prefix, string entry, IDictionary<string, string> results)
		{
			string key;
			string fullName;
			int keyPosition = prefix.Length + 1;

			switch (entry[prefix.Length])
			{
				case '.':
					int dotPosition = entry.IndexOf('.', keyPosition);
					if (dotPosition == -1)
					{
						dotPosition = entry.Length;
					}

					key = entry.Substring(keyPosition, dotPosition - keyPosition);
					fullName = entry.Substring(0, dotPosition);
					break;

				case '[':
					int bracketPosition = entry.IndexOf(']', keyPosition);
					if (bracketPosition == -1)
					{
						// Malformed for dictionary
						return;
					}

					key = entry.Substring(keyPosition, bracketPosition - keyPosition);
					fullName = entry.Substring(0, bracketPosition + 1);
					break;

				default:
					return;
			}

			if (!results.ContainsKey(key))
			{
				results.Add(key, fullName);
			}
		}

		class PrefixComparer : IComparer<string>
		{
			private readonly string _prefix;

			public PrefixComparer(string prefix)
			{
				_prefix = prefix;
			}

			//public int Compare(string x, string y)
			//{
			//	string testString = ReferenceEquals(x, _prefix) ? y : x;
			//	if (IsPrefixMatch(_prefix, testString))
			//	{
			//		return 0;
			//	}
			//	return StringComparer.OrdinalIgnoreCase.Compare(x, y);
			//}

			public int Compare(string x, string y)
			{
				string testString = ReferenceEquals(x, _prefix) ? y : x;
				if (IsPrefixMatch(_prefix, testString.Replace(" ", ""))) // return key to normal form
				{
					return 0;
				}

				return StringComparer.OrdinalIgnoreCase.Compare(x, y.Replace("[", " ["));
			}

			static bool IsPrefixMatch(string prefix, string testString)
			{
				if (testString == null)
				{
					return false;
				}

				if (prefix.Length == 0)
				{
					return true; // shortcut - non-null testString matches empty prefix
				}

				if (prefix.Length > testString.Length)
				{
					return false; // not long enough
				}

				if (!testString.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
				{
					return false; // prefix doesn't match
				}

				if (testString.Length == prefix.Length)
				{
					return true; // exact match
				}

				// invariant: testString.Length > prefix.Length
				switch (testString[prefix.Length])
				{
					case '.':
					case '[':
						return true; // known delimiters

					default:
						return false; // not known delimiter
				}
			}

		}
	}
}