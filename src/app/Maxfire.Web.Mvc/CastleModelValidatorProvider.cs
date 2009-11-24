using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Castle.Components.Validator;

namespace Maxfire.Web.Mvc
{
	public class CastleModelValidatorProvider
	{
		
	}

	public class CastleModelValidator : ModelValidator
	{
		private readonly IValidatorRunner _validatorRunner;

		public CastleModelValidator(ModelMetadata metadata, ControllerContext controllerContext, IValidatorRunner validatorRunner) : base(metadata, controllerContext)
		{
			_validatorRunner = validatorRunner;
		}

		public override IEnumerable<ModelValidationResult> Validate(object container)
		{
			if (!_validatorRunner.IsValid(container))
			{
				var errorSummary = _validatorRunner.GetErrorSummary(container);
				foreach (string propertyName in errorSummary.InvalidProperties)
				{
					foreach (var errorMessage in errorSummary.GetErrorsForProperty(propertyName))
					{
						yield return new ModelValidationResult
						             	{
						             		MemberName = propertyName,
						             		Message = errorMessage
						             	};
					}
				}
			}
		}

		//public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
		//{
		//    // Todo: Instead of returning empty rule set, do something
		//    return base.GetClientValidationRules();
		//}

		public IDictionary<string, string[]> GetValidationErrorsFor<TInputModel>(TInputModel input)
		{
			Dictionary<string, string[]> result = new Dictionary<string, string[]>();
			if (!_validatorRunner.IsValid(input))
			{
				ErrorSummary summary = _validatorRunner.GetErrorSummary(input);
				foreach (string propertyName in summary.InvalidProperties)
				{
					string[] errorsForProperty = summary.GetErrorsForProperty(propertyName);

					List<string> errorMessages = new List<string>();
					foreach (var errorMessage in errorsForProperty)
					{
						errorMessages.Add(errorMessage);
					}

					result.Add(propertyName, errorMessages.ToArray());
				}
			}
			return result;
		}
	}

	public interface IModelValidator
	{
		IDictionary<string, string[]> GetValidationErrorsFor<TInputModel>(TInputModel input);
	}
}