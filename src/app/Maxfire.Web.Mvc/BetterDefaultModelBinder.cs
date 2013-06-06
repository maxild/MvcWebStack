using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Maxfire.Web.Mvc
{
	public class BetterDefaultModelBinder : ExtensibleDefaultModelBinder
	{
		protected override object BindModelCore(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			// Because it is impossible to decide up front if any keys are matching the properties of 
			// a complex model type, if FallbackToEmptyPrefix == true have resulted in an empty 
			// modelname (=prefix). This check will however cover most cases, where only one model type 
			// is used at a time (on every action)
			var keyEnumerableValueProvider = bindingContext.ValueProvider as IKeyEnumerableValueProvider;
			if (keyEnumerableValueProvider != null)
			{
				// Test that all keys in this request match either required route values (area, controller, action)
				// or other required paremeter names
				if (keyEnumerableValueProvider.GetKeys().All(bindingContext.IsRequiredKey))
				{
					// return non-updated model (possibly null). This overrides the default 
					// no-arg ctor value returned by MVC default model binder, when doing 
					// automatic parameter model binding in the controller-action invoker.
					return bindingContext.Model;
				}
			}

			return base.BindModelCore(controllerContext, bindingContext);
		}

		protected override IEnumerable<string> GetCollectionIndexes(ModelBindingContext bindingContext)
		{
			string indexKey = CreateSubPropertyName(bindingContext.ModelName, "index");
			ValueProviderResult vpResult = bindingContext.ValueProvider.GetValue(indexKey);

			if (vpResult != null)
			{
				// This is the only new line. We store the comma-separated string of arbitrary indexes 
				// in modelstate. This way UI helpers can reach through the CollectionIndexStore and 
				// the ExportModelStateToTempData and ImportModelStateFromTempData are working out of the box.
				// This makes validation in the framework work correctly in both re-rendering
				// and PRG pattern re-rendering scenarioes.
				bindingContext.ModelState.SetModelValue(indexKey, vpResult);

				return vpResult.ConvertTo(typeof(string[])) as string[];
			}

			return null;
		}
	}
}