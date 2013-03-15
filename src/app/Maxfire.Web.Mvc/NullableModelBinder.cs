using System;
using System.Linq;
using System.Web.Mvc;

namespace Maxfire.Web.Mvc
{
	public abstract class NullableModelBinder : IModelBinder
	{
		public virtual object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			if (bindingContext == null)
			{
				throw new ArgumentNullException("bindingContext");
			}

			var keyEnumerableValueProvider = bindingContext.ValueProvider as IKeyEnumerableValueProvider;
			if (keyEnumerableValueProvider != null)
			{
				// Complex model type with no values to be bound should return null (instead of no-arg ctor value)
				if (keyEnumerableValueProvider.GetKeys().All(key => bindingContext.IsRequiredRouteValue(key)))
				{
					return null;
				}
			}

			// Notes: 
			//   1) ContainsPrefix("") == true, for all value providers (even providers with no values)
			//   2) ContainsPrefix(null) => ArgumentNullException
			if (!bindingContext.ValueProvider.ContainsPrefix(bindingContext.ModelName))
			{
				if (string.IsNullOrEmpty(bindingContext.ModelName) || !bindingContext.FallbackToEmptyPrefix)
				{
					return null;
				}

				// We couldn't find any entry that began with the (non-empty) prefix.
				// If this is the top-level element, fall back to the empty prefix.
				bindingContext = new ModelBindingContext
				{
					ModelMetadata = bindingContext.ModelMetadata,
					ModelState = bindingContext.ModelState,
					PropertyFilter = bindingContext.PropertyFilter,
					ValueProvider = bindingContext.ValueProvider
				};
			}

			return BindModelCore(controllerContext, bindingContext);
		}

		protected abstract object BindModelCore(ControllerContext controllerContext, ModelBindingContext bindingContext);

		protected virtual IModelBinder GetBinder(Type modelType)
		{
			// In MVC 3 binder returned from provider is consulted first
			return ModelBinders.Binders.GetBinder(modelType);
		}
	}
}