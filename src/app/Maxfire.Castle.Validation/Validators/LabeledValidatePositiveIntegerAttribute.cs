﻿namespace Maxfire.Castle.Validation.Validators
{
	public class LabeledValidatePositiveIntegerAttribute : LabeledValidateIntegerAttribute
	{
		private const string DEFAULT_ERROR_MESSAGE = "Feltet '{0}' indeholder ikke et positivt validt heltal.";

		public LabeledValidatePositiveIntegerAttribute() 
			: base(() => new LabeledPositiveIntegerValidator(), DEFAULT_ERROR_MESSAGE)
		{
		}
	}

	public class LabeledPositiveIntegerValidator : LabeledIntegerValidator
	{
		protected override bool IsValidValue(long value)
		{
			return (value > 0);
		}
	}
}