namespace Maxfire.Web.Mvc.Validators
{
	public class LabeledValidateRangeAttribute : BaseValidationAttribute
	{
		// Todo: Add more ctors
		// Todo: Maybe, Try use Range<T>
		// Todo: Maybe, See DataAnnotations
		public LabeledValidateRangeAttribute(int min, int max) 
			: base(() => new LabeledRangeValidator(min, max))
		{
		}
	}

	// Todo: Try use Range<T>
	public class LabeledRangeValidator : BaseValidator
	{
		private readonly int _min;
		private readonly int _max;

		// Todo: Add more ctors
		public LabeledRangeValidator(int min, int max)
		{
			_min = min;
			_max = max;
		}

		protected override bool IsValidNonEmptyInput(string fieldValue)
		{
			bool valid = false;

			if (Conventions.ValidateInt32(fieldValue))
			{
				int intValue = Conventions.ParseInt32(fieldValue);
				valid = intValue >= _min && intValue <= _max;
			}

			return valid;
		}

		protected override string BuildErrorMessage()
		{
			return "Feltet '{0}' tilhører ikke intervallet af mulige værdier " + _min + ".." + _max + ".";
		}
	}
}