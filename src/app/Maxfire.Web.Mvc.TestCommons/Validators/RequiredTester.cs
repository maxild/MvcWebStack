using Maxfire.Web.Mvc.Validators;
using Xunit;

namespace Maxfire.Web.Mvc.TestCommons.Validators
{
	public class RequiredTester : ValidatorSpec
	{
		class Foo
		{
			[LabeledValidateNonEmpty]
			public string Bar { get; set; }
		}

		[Fact]
		public void Valid()
		{
			Validating(new Foo { Bar = "hello" })
				.OnProperty(x => x.Bar)
				.ShouldReturnTrue();
		}

		[Fact]
		public void InvalidIfNull()
		{
			Validating(new Foo { Bar = null })
				.OnProperty(x => x.Bar)
				.ShouldReturnFalse()
				.WithErrorMessage("Feltet 'Bar' er et krav.");
		}

		[Fact]
		public void InvalidIfEmpty()
		{
			Validating(new Foo { Bar = "" })
				.OnProperty(x => x.Bar)
				.ShouldReturnFalse()
				.WithErrorMessage("Feltet 'Bar' er et krav.");
		}

		[Fact]
		public void InvalidIfWhitespace()
		{
			Validating(new Foo { Bar = "\t\n\v\f\r    \x0085\x00a0\u2028\u2029" })
				.OnProperty(x => x.Bar)
				.ShouldReturnFalse()
				.WithErrorMessage("Feltet 'Bar' er et krav.");
		}
	}
}