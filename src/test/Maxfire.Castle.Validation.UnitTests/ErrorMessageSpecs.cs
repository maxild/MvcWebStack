using Maxfire.Castle.Validation.Validators;
using Maxfire.TestCommons.AssertExtensions;
using Xunit;

namespace Maxfire.Castle.Validation.UnitTests
{
	public class ErrorMessageSpecs : ValidatorConstructor
	{
		public class Foo
		{
			[Validate]
			public string DefaultErrorMessage { get; set; }

			[Validate(ErrorMessage = "ErrorMessage")]
			public string ErrorMessage { get; set; }

			[ValidateBuildErrorMessage]
			public string BuildErrorMessage { get; set; }
		}

		public class ValidateAttribute : BaseValidationAttribute
		{
			private const string DEFAULT_ERROR_MESSAGE = "DefaultErrorMessage";

			public ValidateAttribute()
				: base(() => new Validator(), DEFAULT_ERROR_MESSAGE)
			{
			}

			class Validator : BaseValidator
			{
				protected override bool IsValidNonEmptyInput(string fieldValue)
				{
					return false;
				}
			}
		}

		public class ValidateBuildErrorMessageAttribute : BaseValidationAttribute
		{
			public ValidateBuildErrorMessageAttribute()
				: base(() => new ValidatorWithBuildErrorMessage(), "")
			{
			}

			class ValidatorWithBuildErrorMessage : BaseValidator
			{
				protected override bool IsValidNonEmptyInput(string fieldValue)
				{
					return false;
				}

				protected override string BuildErrorMessage()
				{
					return "BuildErrorMessage";
				}
			}
		}

		[Fact]
		public void DefaultErrorMessage()
		{
			CreateValidatorFor<Foo>(x => x.ErrorMessage).ErrorMessage.ShouldEqual("ErrorMessage");
		}

		[Fact]
		public void BuildErrorMessage()
		{
			CreateValidatorFor<Foo>(x => x.BuildErrorMessage).ErrorMessage.ShouldEqual("BuildErrorMessage");
		}

		[Fact]
		public void ErrorMessage()
		{
			CreateValidatorFor<Foo>(x => x.DefaultErrorMessage).ErrorMessage.ShouldEqual("DefaultErrorMessage");
		}
	}
}