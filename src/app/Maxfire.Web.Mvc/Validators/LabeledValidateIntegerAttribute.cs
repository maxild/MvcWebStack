using System;

namespace Maxfire.Web.Mvc.Validators
{
	public class LabeledValidateIntegerAttribute : BaseValidationAttribute
	{
		private const string DEFAULT_ERROR_MESSAGE = "Feltet '{0}' indeholder ikke et validt heltal.";

		public LabeledValidateIntegerAttribute()
			: this(() => new LabeledIntegerValidator(), DEFAULT_ERROR_MESSAGE)
		{
		}

		protected LabeledValidateIntegerAttribute(Func<BaseValidator> thunk, string errorMessage) : base(thunk, errorMessage)
		{
		}
	}

	public class LabeledIntegerValidator : BaseValidator
	{
		protected override bool IsValidNonEmptyInput(string fieldValue)
		{
			bool valid = false;

			if (Property != null && (Property.PropertyType == typeof(short) || Property.PropertyType == typeof(short?)))
			{
				var val = Conventions.TryValidateInt16(fieldValue);
				if (val != null)
				{
					valid = IsValidValue(val.Value);
				}
			}
			else if (Property != null && (Property.PropertyType == typeof(long) || Property.PropertyType == typeof(long?)))
			{
				var val = Conventions.TryValidateInt64(fieldValue);
				if (val != null)
				{
					valid = IsValidValue(val.Value);
				}
			}
			else
			{
				var val = Conventions.TryValidateInt32(fieldValue);
				if (val != null)
				{
					valid = IsValidValue(val.Value);
				}
			}

			return valid;
		}

		protected virtual bool IsValidValue(long value)
		{
			return true;
		}
	}
}