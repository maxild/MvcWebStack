namespace Maxfire.Web.Mvc.Validators
{
	public class LabeledValidateDateAttribute : BaseValidationAttribute
	{
		private const string DEFAULT_ERROR_MESSAGE = "Feltet '{0}' indeholder ikke en valid dato.";

		public LabeledValidateDateAttribute() 
			: base(() => new LabeledDateValidator(), DEFAULT_ERROR_MESSAGE)
		{
		}
		
		public class LabeledDateValidator : BaseValidator
		{
			protected override bool IsValidNonEmptyInput(string fieldValue)
			{
				return Conventions.ValidateDate(fieldValue);
			}
		}
	}
}