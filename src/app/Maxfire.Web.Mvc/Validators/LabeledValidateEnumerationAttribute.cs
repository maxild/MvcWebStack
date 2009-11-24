using System;
using Maxfire.Core;

namespace Maxfire.Web.Mvc.Validators
{
	public class LabeledValidateEnumerationAttribute : BaseValidationAttribute
	{
		private const string DEFAULT_ERROR_MESSAGE = "Feltet '{0}' indeholder ikke en valid værdi.";

		public LabeledValidateEnumerationAttribute(Type enumerationType)
			: base(() => new EnumerationValidator(enumerationType), DEFAULT_ERROR_MESSAGE)
		{
		}

		class EnumerationValidator : BaseValidator
		{
			private readonly Type _enumerationType;

			public EnumerationValidator(Type enumerationType)
			{
				if (enumerationType.IsAssignableFrom(typeof(Enumeration)) || enumerationType.IsAbstract)
				{
					throw new ArgumentException("The argument is not a concrete Enumeration derived type.", "enumerationType");
				}
				_enumerationType = enumerationType;
			}

			public override bool IsValidCore(string fieldValue)
			{
				return Conventions.ValidateEnumerationOfType(_enumerationType, fieldValue);
			}
		}
	}
}