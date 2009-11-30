using System;
using Maxfire.Web.Mvc.Validators.Extensions;

namespace Maxfire.Web.Mvc.Validators
{
	public class LabeledValidateNonEmptyAttribute : BaseValidationAttribute
	{
		private const string DEFAULT_ERROR_MESSAGE = "Feltet '{0}' er et krav.";

		public LabeledValidateNonEmptyAttribute() 
			: base(() => new LabeledNonEmptyValidator(), DEFAULT_ERROR_MESSAGE)
		{
		}

		class LabeledNonEmptyValidator : BaseValidator
		{
			public override bool IsValid(object instance, object fieldValue)
			{
				var result = fieldValue.IsTrimmedNotEmpty();
				return result;
			}

			protected override bool IsValidNonEmptyInput(string fieldValue)
			{
				throw new NotImplementedException("This method is not called, because of the IsValid override.");
			}
		}
	}
}