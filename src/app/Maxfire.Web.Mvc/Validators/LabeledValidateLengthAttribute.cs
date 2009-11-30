namespace Maxfire.Web.Mvc.Validators
{
	public class LabeledValidateLengthAttribute : BaseValidationAttribute
	{
		public LabeledValidateLengthAttribute(int length) 
			: base(() => new LabeledLengthValidator(length))
		{
		}
	}

	public class LabeledLengthValidator : BaseValidator
	{
		private readonly int _length;

		public LabeledLengthValidator(int length)
		{
			_length = length;
		}

		protected override bool IsValidNonEmptyInput(string fieldValue)
		{
			int length = fieldValue.Length;
			return (length == _length);
		}

		protected override string BuildErrorMessage()
		{
			return "Feltet '{0}' skal indeholde " + _length + " karakterer.";
		}
	}
}