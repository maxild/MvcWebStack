using System;
using Maxfire.Core;
using Maxfire.Core.Extensions;

namespace Maxfire.Castle.Validation.Validators
{
	public class LabeledValidateEnumerationAttribute : BaseValidationAttribute
	{
		private const string DEFAULT_ERROR_MESSAGE = "Feltet '{0}' indeholder ikke en valid værdi.";

		public LabeledValidateEnumerationAttribute(Type enumerationType)
			: base(() => new EnumerationValidator(enumerationType), DEFAULT_ERROR_MESSAGE)
		{
		}
	}

	public class EnumerationValidator : BaseValidator
	{
		private readonly Type _enumerationType;

		public EnumerationValidator(Type enumerationType)
		{
			if (enumerationType.IsNotAssignableTo(typeof(Enumeration)) || enumerationType.IsAbstract)
			{
				throw new ArgumentException("The argument is not a concrete Enumeration derived type.", "enumerationType");
			}
			_enumerationType = enumerationType;
		}

		protected override bool IsValidNonEmptyInput(string fieldValue)
		{
			return Conventions.ValidateEnumerationOfType(_enumerationType, fieldValue);
		}
	}
}