using System;
using System.Web.Mvc;
using Maxfire.Prelude;

namespace Maxfire.Web.Mvc
{
	public class EnumerationModelBinder<TEnumeration> : SimpleModelBinder<TEnumeration>
		where TEnumeration : Enumeration<TEnumeration>
	{
		protected override TEnumeration BindModelCore(ValueProviderResult valueProviderResult, ModelBindingContext bindingContext)
		{
			TEnumeration value = null;
			try
			{
				value = Enumeration.FromName<TEnumeration>(valueProviderResult.AttemptedValue);
			}
			catch (Exception ex)
			{
				bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);
				bindingContext.ModelState.AddModelError(bindingContext.ModelName, ex);
			}

			return value;
		}
	}
}
