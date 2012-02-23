using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Castle.Components.Validator;
using Maxfire.Core.Extensions;
using Maxfire.Web.Mvc;

namespace Maxfire.Castle.Validation.Validators
{
	public abstract class BaseValidator : AbstractValidator
	{
		public override void Initialize(IValidatorRegistry validationRegistry, System.Reflection.PropertyInfo property)
		{
			base.Initialize(validationRegistry, property);

			// Subclasses should be able to override ErrorMessage by overriding BuildErrorMessage() 
			// unless the user has provided a non-empty ErrorMessage
			if (ErrorMessage.IsEmpty())
			{
				string buildErrorMessage = BuildErrorMessage();
				ErrorMessage = buildErrorMessage.IsNotEmpty() ? buildErrorMessage : DefaultErrorMessage;
			}
		}

		public override bool IsValid(object instance, object fieldValue)
		{
			var typedCollection = fieldValue as IEnumerable<string>;
			if (typedCollection != null)
			{
				return typedCollection.All(item => IsValidNonEmptyInput(item.ToTrimmedNullSafeString()));
			}

			if (fieldValue.IsTrimmedEmpty())
			{
				return true;
			}

			return IsValidNonEmptyInput(fieldValue.ToTrimmedNullSafeString());
		}

		protected abstract bool IsValidNonEmptyInput(string fieldValue);

		protected MappingConventions Conventions
		{
			get { return new MappingConventions(); }
		}

		public override bool SupportsBrowserValidation
		{
			get { return false; }
		}

		public string DefaultErrorMessage { get; set; }

		protected override string BuildErrorMessage()
		{
			return null;
		}

		// Todo: Have the performer/runner use this method
		// Note: derived classes should override this method to inject min, max, length and other values helping the user.
		public virtual string FormatErrorMessage(string displayName)
		{
			return string.Format(CultureInfo.CurrentCulture, ErrorMessage, displayName);
		}
	}
}