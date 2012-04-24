﻿namespace Maxfire.Castle.Validation.Validators
{
	public class LabeledValidateMoneyAttribute : BaseValidationAttribute
	{
		private const string DEFAULT_ERROR_MESSAGE = "Feltet '{0}' indeholder ikke et validt decimal tal.";

		public LabeledValidateMoneyAttribute() 
			: base(() => new LabeledMoneyValidator(), DEFAULT_ERROR_MESSAGE)
		{
		}
	}

	public class LabeledMoneyValidator : BaseValidator
	{
		protected override bool IsValidNonEmptyInput(string fieldValue)
		{
			return Conventions.ValidateMoney(fieldValue);
		}
	}
}