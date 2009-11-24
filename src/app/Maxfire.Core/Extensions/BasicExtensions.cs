using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Maxfire.Core.Collections;

namespace Maxfire.Core.Extensions
{
	public static class BasicExtensions
	{
		public static bool IsEmpty<T>(this IEnumerable<T> iterator)
		{
			return !iterator.IsNotEmpty();
		}

		public static bool IsNotEmpty<T>(this IEnumerable<T> iterator)
		{
			return iterator != null && iterator.Any();
		}

		public static T[] AsArrayOfSize<T>(this T value, int size)
		{
			var array = new T[size];
			for (int i = 0; i < size; i++)
			{
				array[i] = value;
			}
			return array;
		}

		[DebuggerStepThrough]
		public static IEnumerable Each(this IEnumerable values, Action<object> eachAction)
		{
			foreach (var item in values)
			{
				eachAction(item);
			}

			return values;
		}

		[DebuggerStepThrough]
		public static IEnumerable<T> Each<T>(this IEnumerable<T> values, Action<T> eachAction)
		{
			foreach (var item in values)
			{
				eachAction(item);
			}

			return values;
		}

		[DebuggerStepThrough]
		public static IEnumerable<T> Each<T>(this IEnumerable<T> values, Action<T, int> eachAction)
		{
			int i = 0;
			foreach (var item in values)
			{
				eachAction(item, i);
				i++;
			}

			return values;
		}

		[DebuggerStepThrough]
		public static void Times(this int maxCount, Action<int> eachAction)
		{
			for (var idx = 0; idx < maxCount; idx++)
			{
				eachAction(idx);
			}
		}

		[DebuggerStepThrough]
		public static void Times(this int maxCount, Action eachAction)
		{
			for (var idx = 0; idx < maxCount; idx++)
			{
				eachAction();
			}
		}

		[DebuggerStepThrough]
		public static void TimesStartingAt(this int maxCount, int startIndex, Action<int> eachAction)
		{
			for (var idx = 0; idx < maxCount; idx++)
			{
				eachAction(idx + startIndex);
			}
		}

		/// <summary>
		/// Creates an inclusive range between two values. The default comparer is used
		/// to compare values.
		/// </summary>
		/// <typeparam name="T">Type of the values</typeparam>
		/// <param name="start">Start of range.</param>
		/// <param name="end">End of range.</param>
		/// <returns>An inclusive range between the start point and the end point.</returns>
		public static Range<T> To<T>(this T start, T end)
		{
			return new Range<T>(start, end);
		}

		public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey tKey)
		{
			return dictionary.GetValueOrDefault(tKey, default(TValue));
		}

		public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey tKey, TValue defaultTValue)
		{
			if (dictionary.ContainsKey(tKey)) return dictionary[tKey];
			return defaultTValue;
		}

		/// <summary>
		/// Returns the value associated with the specified key if there
		/// already is one, or inserts a new value for the specified key and
		/// returns that.
		/// </summary>
		/// <typeparam name="TKey">Type of key</typeparam>
		/// <typeparam name="TValue">Type of value, which must either have
		/// a public parameterless constructor or be a value type</typeparam>
		/// <param name="dictionary">Dictionary to access</param>
		/// <param name="key">Key to lookup</param>
		/// <returns>Existing value in the dictionary, or new one inserted</returns>
		public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
		                                               TKey key)
			where TValue : new()
		{
			TValue ret;
			if (!dictionary.TryGetValue(key, out ret))
			{
				ret = new TValue();
				dictionary[key] = ret;
			}
			return ret;
		}

		/// <summary>
		/// Returns the value associated with the specified key if there already
		/// is one, or calls the specified delegate to create a new value which is
		/// stored and returned.
		/// </summary>
		/// <typeparam name="TKey">Type of key</typeparam>
		/// <typeparam name="TValue">Type of value</typeparam>
		/// <param name="dictionary">Dictionary to access</param>
		/// <param name="key">Key to lookup</param>
		/// <param name="valueProvider">Delegate to provide new value if required</param>
		/// <returns>Existing value in the dictionary, or new one inserted</returns>
		public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
		                                               TKey key,
		                                               Func<TValue> valueProvider)
		{
			TValue ret;
			if (!dictionary.TryGetValue(key, out ret))
			{
				ret = valueProvider();
				dictionary[key] = ret;
			}
			return ret;
		}

		/// <summary>
		/// Returns the value associated with the specified key if there
		/// already is one, or inserts the specified value and returns it.
		/// </summary>
		/// <typeparam name="TKey">Type of key</typeparam>
		/// <typeparam name="TValue">Type of value</typeparam>
		/// <param name="dictionary">Dictionary to access</param>
		/// <param name="key">Key to lookup</param>
		/// <param name="missingValue">Value to use when key is missing</param>
		/// <returns>Existing value in the dictionary, or new one inserted</returns>
		public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
		                                               TKey key,
		                                               TValue missingValue)
		{
			TValue ret;
			if (!dictionary.TryGetValue(key, out ret))
			{
				ret = missingValue;
				dictionary[key] = ret;
			}
			return ret;
		}
	}
}