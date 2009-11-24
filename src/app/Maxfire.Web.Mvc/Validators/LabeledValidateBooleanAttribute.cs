namespace Maxfire.Web.Mvc.Validators
{
	public class LabeledValidateBooleanAttribute : BaseValidationAttribute
	{
		private const string DEFAULT_ERROR_MESSAGE = "Feltet '{0}' indeholder ikke en valid boolean.";

		public LabeledValidateBooleanAttribute()
			: base(() => new LabeledBooleanValidator(), DEFAULT_ERROR_MESSAGE)
		{
		}

		public class LabeledBooleanValidator : BaseValidator
		{
			public override bool IsValidCore(string fieldValue)
			{
				return Conventions.ValidateBoolean(fieldValue);
			}
		}
	}
}