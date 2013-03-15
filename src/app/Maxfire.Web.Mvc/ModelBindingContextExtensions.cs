using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Maxfire.Web.Mvc
{
	public static class ModelBindingContextExtensions
	{
		public static bool IsRequiredRouteValue(this ModelBindingContext bindingContext, string value)
		{
			return GetRequiredRouteValues(bindingContext)
				.Any(s => value.Equals(s, StringComparison.OrdinalIgnoreCase));
		}

		private static IEnumerable<string> GetRequiredRouteValues(ModelBindingContext bindingContext)
		{
			yield return "area";
			yield return "controller";
			yield return "action";
			foreach (string requiredParam in bindingContext.ModelMetadata.GetRequiredParameterNames())
			{
				yield return requiredParam;
			}
		}

		public static ModelBindingContext ChangeModelName(this ModelBindingContext bindingContext, string modelName = null)
		{
			return new ModelBindingContext
				{
					// FallbackToEmptyPrefix = false
					ModelMetadata = bindingContext.ModelMetadata,
					ModelName = modelName,
					ModelState = bindingContext.ModelState,
					ValueProvider = bindingContext.ValueProvider,
					PropertyFilter = bindingContext.PropertyFilter,
				};
		}

		public static ModelBindingContext ChangeModelMetadata(this ModelBindingContext bindingContext, ModelMetadata modelMetadata)
		{
			return new ModelBindingContext
			{
				// FallbackToEmptyPrefix = false
				ModelMetadata = modelMetadata,
				ModelName = bindingContext.ModelName,
				ModelState = bindingContext.ModelState,
				ValueProvider = bindingContext.ValueProvider,
				PropertyFilter = bindingContext.PropertyFilter,
			};
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