using System;
using Castle.Components.Validator;

namespace Maxfire.Web.Mvc.Validators
{
	public abstract class BaseValidationAttribute : AbstractValidationAttribute
	{
		private const string DEFAULT_ERROR_MESSAGE = "Feltet '{0}' er ikke validt.";

		private readonly Func<IValidator> _thunk;

		protected BaseValidationAttribute(Func<IValidator> thunk)
			: this(thunk, DEFAULT_ERROR_MESSAGE)
		{
		}

		protected BaseValidationAttribute(Func<IValidator> thunk, string defaultErrorMessage)
		{
			_thunk = thunk;
			ErrorMessage = defaultErrorMessage;
		}

		/// <summary>
		/// User can set this like [Required(ErrorMessage="...")] to override the default message
		/// </summary>
		public new string ErrorMessage { get; set; }
		
		public override IValidator Build()
		{
			IValidator validator = _thunk();
			validator.RunWhen = RunWhen.Everytime;
			validator.ExecutionOrder = ExecutionOrder;
			validator.ErrorMessage = ErrorMessage;
			return validator;
		}
	}
}