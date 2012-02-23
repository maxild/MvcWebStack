namespace Maxfire.Castle.Validation.Validators
{
	public class LabeledValidatePostNrAttribute : LabeledRegexValidatorAttribute
	{
		private const string DEFAULT_ERROR_MESSAGE = "Feltet '{0}' indeholder ikke et validt postnummer. Benyt venligst fire heltal.";

		public LabeledValidatePostNrAttribute()
			: base("^[0-9]{4}$", DEFAULT_ERROR_MESSAGE)
		{
		}
	}
}