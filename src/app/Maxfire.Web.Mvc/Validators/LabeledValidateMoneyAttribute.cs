namespace Maxfire.Web.Mvc.Validators
{
	public class LabeledValidateMoneyAttribute : BaseValidationAttribute
	{
		private const string DEFAULT_ERROR_MESSAGE = "Feltet '{0}' indeholder ikke et validt decimal tal.";

		public LabeledValidateMoneyAttribute() 
			: base(() => new LabeledMoneyValidator(), DEFAULT_ERROR_MESSAGE)
		{
		}

		public class LabeledMoneyValidator : BaseValidator
		{
			public override bool IsValidCore(string fieldValue)
			{
				return Conventions.ValidateMoney(fieldValue);
			}
		}
	}
}