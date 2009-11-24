using System;
using System.ComponentModel;
using System.Web.Mvc;

namespace Maxfire.Web.Mvc
{
	public class ValidationModelBinder : DefaultModelBinder
	{
		protected override IModelBinder GetBinder(Type modelType)
		{
			var binder = Binders.GetBinder(modelType, false);
			return binder ?? this;
		}

		protected override void OnModelUpdated(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			foreach (ModelValidator validator in bindingContext.ModelMetadata.GetValidators(controllerContext))
			{
				foreach (ModelValidationResult validationResult in validator.Validate(bindingContext.Model))
				{
					bindingContext.ModelState.AddModelError(CreateSubPropertyName(bindingContext.ModelName, validationResult.MemberName), validationResult.Message);
				}
			}
		}

		// TODO: Try to use base version that validates => Should imply too many (duplicate) model errors
		protected override void OnPropertyValidated(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, object value)
		{
		}
	}
}