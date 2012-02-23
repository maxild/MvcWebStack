using System;
using Maxfire.Core.Extensions;

namespace Maxfire.Castle.Validation.Validators
{
	public class LabeledValidateNonEmptyAttribute : BaseValidationAttribute
	{
		private const string DEFAULT_ERROR_MESSAGE = "Feltet '{0}' er et krav.";

		public LabeledValidateNonEmptyAttribute() 
			: base(() => new LabeledNonEmptyValidator(), DEFAULT_ERROR_MESSAGE)
		{
		}
	}

	public class LabeledNonEmptyValidator : BaseValidator
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