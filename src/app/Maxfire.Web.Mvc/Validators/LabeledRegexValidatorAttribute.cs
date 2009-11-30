using System.Text.RegularExpressions;

namespace Maxfire.Web.Mvc.Validators
{
	public class LabeledRegexValidatorAttribute : BaseValidationAttribute
	{
		private const string DEFAULT_ERROR_MESSAGE = "Feltet '{0}' indeholder ikke en valid værdi.";

		protected LabeledRegexValidatorAttribute(string pattern)
			: base(() => new LabeledRegexValidator(pattern), DEFAULT_ERROR_MESSAGE)
		{
		}

		protected LabeledRegexValidatorAttribute(string pattern, string errorMessage)
			: base(() => new LabeledRegexValidator(pattern), errorMessage)
		{
		}

		class LabeledRegexValidator : BaseValidator
		{
			private readonly Regex _regex;
			
			public LabeledRegexValidator(string pattern)
			{
				_regex = new Regex(pattern, RegexOptions.Compiled);
			}

			protected override bool IsValidNonEmptyInput(string fieldValue)
			{
				return _regex.IsMatch(fieldValue);
			}
		}
	}

	public class LabeledValidatePostNrAttribute : LabeledRegexValidatorAttribute
	{
		private const string DEFAULT_ERROR_MESSAGE = "Feltet '{0}' indeholder ikke et validt postnummer. Benyt venligst fire heltal.";

		public LabeledValidatePostNrAttribute()
			: base("^[0-9]{4}$", DEFAULT_ERROR_MESSAGE)
		{
		}
	}

	public class LabeledValidateAarstalAttribute : LabeledRegexValidatorAttribute
	{
		private const string DEFAULT_ERROR_MESSAGE = "Feltet '{0}' indeholder ikke et validt årstal. Benyt venligst fire heltal.";

		public LabeledValidateAarstalAttribute()
			: base("^[0-9]{4}$", DEFAULT_ERROR_MESSAGE)
		{
		}
	}
}