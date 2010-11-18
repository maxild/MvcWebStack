using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Maxfire.Core.Extensions
{
	public static class DictionaryExtenions
	{
		public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> values, IEnumerable<KeyValuePair<TKey, TValue>> valuesToAdd)
		{
			foreach (var kvp in valuesToAdd)
			{
				values.Add(kvp.Key, kvp.Value);
			}
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

		public static IEnumerable<KeyValuePair<string, TValue>> ToPropertyValuePairs<TValue>(this object objectDictionary, Func<object, TValue> valueSelector)
		{
			return TypeDescriptor.GetProperties(objectDictionary)
				.Cast<PropertyDescriptor>()
				.Select(descriptor => new KeyValuePair<string, TValue>(descriptor.Name, valueSelector(descriptor.GetValue(objectDictionary))));
		}

		public static IEnumerable<KeyValuePair<string, object>> ToPropertyObjectValuePairs(this object dictionary)
		{
			return dictionary.ToPropertyValuePairs(x => x);
		}

		public static IEnumerable<KeyValuePair<string, string>> ToPropertyStringValuePairs(this object dictionary)
		{
			return dictionary.ToPropertyValuePairs(x => x.ToNullSafeString());
		}
	}
}