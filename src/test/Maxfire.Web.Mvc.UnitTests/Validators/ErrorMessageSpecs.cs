using Maxfire.TestCommons.AssertExtensions;
using Maxfire.Web.Mvc.Validators;
using Xunit;

namespace Maxfire.Web.Mvc.UnitTests.Validators
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
				public override bool IsValidCore(string fieldValue)
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
				public override bool IsValidCore(string fieldValue)
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