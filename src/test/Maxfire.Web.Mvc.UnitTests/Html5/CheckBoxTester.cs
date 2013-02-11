using Maxfire.TestCommons.AssertExtensions;
using Maxfire.Web.Mvc.Html5.Elements;
using Xunit;

namespace Maxfire.Web.Mvc.UnitTests.Html5
{
	public class CheckBoxTester
	{
		[Fact]
		public void ValueShouldAlwaysEqualExplicitValueWhenExplicitValueHaveBeenDefined()
		{
			var sut = new CheckBox("name", new ModelMetadataAccessorFor<bool?>("false", () => false)).Checked(true);
			sut.ApplyModelState();
			sut.Checked().ShouldBeTrue();

			sut = new CheckBox("name", new ModelMetadataAccessorFor<bool?>(null, () => null)).Checked(true);
			sut.ApplyModelState();
			sut.Checked().ShouldBeTrue();

			sut = new CheckBox("name", new ModelMetadataAccessorFor<bool?>("false", () => null)).Checked(true);
			sut.ApplyModelState();
			sut.Checked().ShouldBeTrue();

			sut = new CheckBox("name", new ModelMetadataAccessorFor<bool?>(null, () => false)).Checked(true);
			sut.ApplyModelState();
			sut.Checked().ShouldBeTrue();
		}

		[Fact]
		public void ValueShouldEqualAttemptedValueWhenNoExplicitValueHaveBeenDefined()
		{
			var sut = new CheckBox("name", new ModelMetadataAccessorFor<bool?>("true", () => false));
			sut.ApplyModelState();
			sut.Checked().ShouldBeTrue();

			sut = new CheckBox("name", new ModelMetadataAccessorFor<bool?>("true", () => null));
			sut.ApplyModelState();
			sut.Checked().ShouldBeTrue();
		}

		[Fact]
		public void ValueShouldEqualModelValueWhenNeitherExplicitValueOrAttemptedValueHaveBeenDefined()
		{
			var sut = new CheckBox("name", new ModelMetadataAccessorFor<bool?>(null, () => true));
			sut.ApplyModelState();
			sut.Checked().ShouldBeTrue();
		}

		[Fact]
		public void ValueShouldBeFalseWhenNeitherAttemptedValueOrModelValueOrExplicitValueHaveBeenDefined()
		{
			var sut = new CheckBox("name", new ModelMetadataAccessorFor<bool?>(null, () => null));
			sut.ApplyModelState();
			sut.Checked().ShouldBeFalse();
		}
	}
}