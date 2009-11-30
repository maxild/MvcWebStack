namespace Maxfire.Web.Mvc.Validators
{
	public class LabeledValidateDecimalAttribute : BaseValidationAttribute
	{
		private const string DEFAULT_ERROR_MESSAGE = "Feltet '{0}' indeholder ikke et validt decimal tal.";

		public LabeledValidateDecimalAttribute() 
			: base(() => new LabeledDecimalValidator(), DEFAULT_ERROR_MESSAGE)
		{
		}
		
		public class LabeledDecimalValidator : BaseValidator
		{
			protected override bool IsValidNonEmptyInput(string fieldValue)
			{
				return Conventions.ValidateMachineDecimal(fieldValue);
			}
		}
	}
}