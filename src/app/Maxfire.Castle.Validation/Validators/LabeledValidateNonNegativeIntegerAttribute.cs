namespace Maxfire.Castle.Validation.Validators
{
	public class LabeledValidateNonNegativeIntegerAttribute : LabeledValidateIntegerAttribute
	{
		private const string DEFAULT_ERROR_MESSAGE = "Feltet '{0}' indeholder ikke et ikke-negativt validt heltal.";

		public LabeledValidateNonNegativeIntegerAttribute()
			: base(() => new LabeledNonNegativeIntegerValidator(), DEFAULT_ERROR_MESSAGE)
		{
		}
	}

	public class LabeledNonNegativeIntegerValidator : LabeledIntegerValidator
	{
		protected override bool IsValidValue(long value)
		{
			return (value >= 0);
		}
	}
}