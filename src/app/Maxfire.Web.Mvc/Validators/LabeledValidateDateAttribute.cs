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
			public override bool IsValidCore(string fieldValue)
			{
				return Conventions.ValidateDate(fieldValue);
			}
		}
	}
}