using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Maxfire.Web.Mvc.ValueProviders;

namespace Maxfire.Web.Mvc
{
	public class ModelStateValueProvider : IEnumerableValueProvider, IKeyEnumerableValueProvider
	{
		private readonly Lazy<BetterPrefixContainer> _prefixContainer;
		private readonly ModelStateDictionary _dictionary;

		public ModelStateValueProvider(ModelStateDictionary dictionary)
		{
			if (dictionary == null)
				throw new ArgumentNullException("dictionary");

			_prefixContainer = new Lazy<BetterPrefixContainer>(() => new BetterPrefixContainer(dictionary.Keys), isThreadSafe: true);
			_dictionary = dictionary;
		}

		public bool ContainsPrefix(string prefix)
		{
			return _prefixContainer.Value.ContainsPrefix(prefix);
		}

		public ValueProviderResult GetValue(string key)
		{
			ModelState modelState = _dictionary[key];
			return modelState != null ? modelState.Value : null;
		}

		public IDictionary<string, string> GetKeysFromPrefix(string prefix)
		{
			return _prefixContainer.Value.GetKeysFromPrefix(prefix);
		}

		public IEnumerable<string> GetKeys()
		{
			return _dictionary.Keys;
		}
	}
}