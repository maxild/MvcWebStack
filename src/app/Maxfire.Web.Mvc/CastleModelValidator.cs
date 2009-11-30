using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Castle.Components.Validator;

namespace Maxfire.Web.Mvc
{
	public class CastleModelValidator : ModelValidator, IFormValidator
	{
		// Todo: Maybe write custom runner => use IValidationRegistry and nothing else (see performer in castle)
		private readonly IValidatorRunner _validatorRunner;

		public CastleModelValidator(ModelMetadata metadata, ControllerContext controllerContext, IValidatorRunner validatorRunner) : base(metadata, controllerContext)
		{
			_validatorRunner = validatorRunner;
		}

		public override IEnumerable<ModelValidationResult> Validate(object container)
		{
			// Todo: Why do we need container argument?
			// Todo: Should we use 'container' or 'Metadata.Model'???
			if (!_validatorRunner.IsValid(Metadata.Model))
			{
				var errorSummary = _validatorRunner.GetErrorSummary(container);
				foreach (string propertyName in errorSummary.InvalidProperties)
				{
					foreach (var errorMessage in errorSummary.GetErrorsForProperty(propertyName))
					{
						var propertyMetadata = PropertyMetadata[propertyName];
						yield return new ModelValidationResult
						             	{
						             		MemberName = propertyName,
											Message = formatErrorMessage(errorMessage, propertyMetadata.GetDisplayName())
						             	};
					}
				}
			}
		}

		private static string formatErrorMessage(string errorMessage, string propertyName)
		{
			return string.Format(CultureInfo.CurrentCulture, errorMessage, propertyName);
		}

		private Dictionary<string, ModelMetadata> _propertyMetadata;
		public IDictionary<string, ModelMetadata> PropertyMetadata
		{
			get
			{
				if (_propertyMetadata == null)
				{
					_propertyMetadata = Metadata.Properties.ToDictionary(m => m.PropertyName, StringComparer.OrdinalIgnoreCase);
				}
				return _propertyMetadata;
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

	public interface IFormValidator
	{
		IDictionary<string, string[]> GetValidationErrorsFor<TInputModel>(TInputModel input);
	}
}