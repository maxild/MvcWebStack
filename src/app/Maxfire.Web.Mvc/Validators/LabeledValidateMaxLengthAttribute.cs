namespace Maxfire.Web.Mvc.Validators
{
	public class LabeledValidateMaxLengthAttribute : BaseValidationAttribute
	{
		public LabeledValidateMaxLengthAttribute(int length) 
			: base(() => new LabeledMaxLengthValidator(length))
		{
		}

		class LabeledMaxLengthValidator : BaseValidator
		{
			private readonly int _length;

			public LabeledMaxLengthValidator(int length)
			{
				_length = length;
			}

			protected override bool IsValidNonEmptyInput(string fieldValue)
			{
				return fieldValue.Length <= _length;
			}

			protected override string BuildErrorMessage()
			{
				return "Feltet '{0}' kan maksimalt indeholde "+ _length +" karakterer.";
			}
		}
	}
}