using System;
using System.Web.Mvc;
using JetBrains.Annotations;

namespace Maxfire.Web.Mvc
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class ValidateInputModelAttribute : ActionFilterAttribute
	{
		private readonly Type _inputModelType;

		public ValidateInputModelAttribute(Type inputModelType)
		{
			_inputModelType = inputModelType;
		}

		[UsedImplicitly]
		public IFormValidator FormValidator { get; set; }

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			if (FormValidator == null)
			{
				throw new InvalidOperationException(
				    $"The FormValidator property was not injected. Please register an implementation for the {typeof(IFormValidator).Name} service in your IoC container.");
			}

			object inputModel = filterContext.GetValueOfParameterWithType(_inputModelType);
			string prefix = filterContext.GetBindingPrefixOfParameterWithType(_inputModelType);

			var validationErrors = FormValidator.GetValidationErrorsFor(inputModel);

			filterContext.GetModelState().AddValidationErrors(validationErrors, prefix);
		}
	}
}
