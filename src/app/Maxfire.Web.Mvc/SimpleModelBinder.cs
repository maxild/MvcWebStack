using System.Web.Mvc;

namespace Maxfire.Web.Mvc
{
	public abstract class SimpleModelBinder<T> : IModelBinder
	{
		public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			if (bindingContext.ModelType != typeof(T))
			{
				return null;
			}

			var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
			if (valueProviderResult == null)
			{
				return null;
			}

			return BindModelCore(valueProviderResult, bindingContext);
		}

		protected abstract T BindModelCore(ValueProviderResult valueProviderResult, ModelBindingContext bindingContext);
	}
}