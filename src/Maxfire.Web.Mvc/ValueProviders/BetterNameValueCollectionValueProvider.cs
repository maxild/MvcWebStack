using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace Maxfire.Web.Mvc.ValueProviders
{
	public class BetterNameValueCollectionValueProvider : NameValueCollectionValueProvider, IKeyEnumerableValueProvider
	{
		private readonly Lazy<BetterPrefixContainer> _prefixContainer;
		private readonly string[] _keys; // we cannot get to _values in base class

		public BetterNameValueCollectionValueProvider(NameValueCollection collection, NameValueCollection unvalidatedCollection, CultureInfo culture)
			: base(collection, unvalidatedCollection, culture)
		{
			if (collection == null)
				throw new ArgumentNullException("collection");

			_keys = (unvalidatedCollection ?? collection).AllKeys.ToArray();
			_prefixContainer = new Lazy<BetterPrefixContainer>(() => new BetterPrefixContainer(_keys), isThreadSafe: true);
		}

		public override bool ContainsPrefix(string prefix)
		{
			return _prefixContainer.Value.ContainsPrefix(prefix);
		}

		public IEnumerable<string> GetKeys()
		{
			return _keys;
		}

		public override IDictionary<string, string> GetKeysFromPrefix(string prefix)
		{
			return _prefixContainer.Value.GetKeysFromPrefix(prefix);
		}
	}
}