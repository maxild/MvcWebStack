using System;

namespace Maxfire.Web.Mvc.Validators
{
	public class LabeledValidateEnumAttribute : BaseValidationAttribute
	{
		private const string DEFAULT_ERROR_MESSAGE = "Feltet '{0}' indeholder ikke en valid værdi.";

		public LabeledValidateEnumAttribute(Type enumType) 
			: base(() => new LabeledEnumValidator(enumType), DEFAULT_ERROR_MESSAGE)
		{
		}
	}

	public class LabeledEnumValidator : BaseValidator
	{
		private readonly Type _enumType;

		public LabeledEnumValidator(Type enumType)
		{
			_enumType = enumType;
		}

		protected override bool IsValidNonEmptyInput(string fieldValue)
		{
			return Conventions.ValidateEnumOfType(_enumType, fieldValue);
		}
	}
}