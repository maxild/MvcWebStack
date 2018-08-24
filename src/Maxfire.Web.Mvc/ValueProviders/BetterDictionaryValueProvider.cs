using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;

namespace Maxfire.Web.Mvc.ValueProviders
{
	public class BetterDictionaryValueProvider<TValue> : IEnumerableValueProvider, IKeyEnumerableValueProvider
	{
		private readonly Lazy<BetterPrefixContainer> _prefixContainer;
		private readonly Dictionary<string, ValueProviderResult> _values = new Dictionary<string, ValueProviderResult>(StringComparer.OrdinalIgnoreCase);

		public BetterDictionaryValueProvider(IEnumerable<KeyValuePair<string, TValue>> dictionary, CultureInfo culture)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException(nameof(dictionary));
			}

			foreach (KeyValuePair<string, TValue> entry in dictionary)
			{
				object rawValue = entry.Value;
				string attemptedValue = Convert.ToString(rawValue, culture);
				_values[entry.Key] = new ValueProviderResult(rawValue, attemptedValue, culture);
			}

			_prefixContainer = new Lazy<BetterPrefixContainer>(() => new BetterPrefixContainer(_values.Keys), isThreadSafe: true);
		}

		public virtual bool ContainsPrefix(string prefix)
		{
			return _prefixContainer.Value.ContainsPrefix(prefix);
		}

		public virtual ValueProviderResult GetValue(string key)
		{
			if (key == null)
			{
				throw new ArgumentNullException(nameof(key));
			}

		    _values.TryGetValue(key, out var valueProviderResult);
			return valueProviderResult;
		}

		public IEnumerable<string> GetKeys()
		{
			return _values.Keys;
		}

		public virtual IDictionary<string, string> GetKeysFromPrefix(string prefix)
		{
			return _prefixContainer.Value.GetKeysFromPrefix(prefix);
		}
	}
}
