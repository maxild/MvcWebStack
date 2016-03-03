using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Maxfire.Web.Mvc
{
	public static class ModelBindingContextExtensions
	{
		/// <summary>
		/// Does the key match any of the _other_ required parameter names or any of the required route values.
		/// </summary>
		public static bool IsRequiredKey(this ModelBindingContext bindingContext, string value)
		{
			return GetRequiredKeys(bindingContext)
				.Any(s => value.Equals(s, StringComparison.OrdinalIgnoreCase));
		}

		private static IEnumerable<string> GetRequiredKeys(ModelBindingContext bindingContext)
		{
			// required route values
			yield return "area";
			yield return "controller";
			yield return "action";
			// other (that is not this parameter name) required parameter names
			foreach (string requiredParam in bindingContext.ModelMetadata.GetRequiredParameterNames())
			{
				yield return requiredParam;
			}
		}

		public static IUnvalidatedValueProvider GetUnvalidatedValueProvider(this ModelBindingContext bindingContext)
		{
			return (bindingContext.ValueProvider as IUnvalidatedValueProvider) ?? new UnvalidatedValueProviderWrapper(bindingContext.ValueProvider);
		}

		class UnvalidatedValueProviderWrapper : IUnvalidatedValueProvider
		{
			private readonly IValueProvider _backingProvider;

			public UnvalidatedValueProviderWrapper(IValueProvider backingProvider)
			{
				_backingProvider = backingProvider;
			}

			public ValueProviderResult GetValue(string key, bool skipValidation)
			{
				// 'skipValidation' isn't understood by the backing provider and can be ignored
				return GetValue(key);
			}

			public bool ContainsPrefix(string prefix)
			{
				return _backingProvider.ContainsPrefix(prefix);
			}

			public ValueProviderResult GetValue(string key)
			{
				return _backingProvider.GetValue(key);
			}
		}
	}
}