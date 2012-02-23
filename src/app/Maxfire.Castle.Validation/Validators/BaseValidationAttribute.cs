using System;
using Castle.Components.Validator;
using Maxfire.Core.Extensions;

namespace Maxfire.Castle.Validation.Validators
{
	public abstract class BaseValidationAttribute : AbstractValidationAttribute
	{
		private const string DEFAULT_ERROR_MESSAGE = "Feltet '{0}' er ikke validt.";

		private readonly Func<BaseValidator> _thunk;

		protected BaseValidationAttribute(Func<BaseValidator> thunk)
			: this(thunk, DEFAULT_ERROR_MESSAGE)
		{
		}

		protected BaseValidationAttribute(Func<BaseValidator> thunk, string defaultErrorMessage)
		{
			_thunk = thunk;
			DefaultErrorMessage = defaultErrorMessage.IsNotEmpty() ? defaultErrorMessage : DEFAULT_ERROR_MESSAGE;
		}

		/// <summary>
		/// User can set this like [Required(ErrorMessage="...")] to override the default message
		/// </summary>
		public new string ErrorMessage { get; set; }
		
		public string DefaultErrorMessage { get; set; }

		public override IValidator Build()
		{
			var validator = _thunk();
			validator.RunWhen = RunWhen.Everytime;
			validator.ExecutionOrder = ExecutionOrder;
			validator.ErrorMessage = ErrorMessage;
			validator.DefaultErrorMessage = DefaultErrorMessage;
			return validator;
		}
	}
}