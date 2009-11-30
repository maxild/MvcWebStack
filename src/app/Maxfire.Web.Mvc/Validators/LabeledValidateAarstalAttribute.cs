namespace Maxfire.Web.Mvc.Validators
{
	public class LabeledValidateAarstalAttribute : LabeledRegexValidatorAttribute
	{
		private const string DEFAULT_ERROR_MESSAGE = "Feltet '{0}' indeholder ikke et validt årstal. Benyt venligst fire heltal.";

		public LabeledValidateAarstalAttribute()
			: base("^[0-9]{4}$", DEFAULT_ERROR_MESSAGE)
		{
		}
	}
}