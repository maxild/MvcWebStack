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
				// Complex model type with no keys found should return non-updated model (possibly null, instead of no-arg ctor value)
				if (keyEnumerableValueProvider.GetKeys().All(bindingContext.IsRequiredRouteValue))
				{
					return bindingContext.Model;
				}
			}

			return base.BindModelCore(controllerContext, bindingContext);
		}
	}
}