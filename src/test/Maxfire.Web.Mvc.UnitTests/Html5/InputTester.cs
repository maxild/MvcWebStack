using Maxfire.TestCommons.AssertExtensions;
using Maxfire.Web.Mvc.Html5.Elements;
using Xunit;

namespace Maxfire.Web.Mvc.UnitTests.Html5
{
	public class InputTester
	{
		public InputTester()
		{
			AccessorWithNeitherAttemptedOrModel = new ModelMetadataAccessorFor<string>(null, () => null);
			AccessorWithBothAttemptedAndModel = new ModelMetadataAccessorFor<string>("attempted", () => "model");
			AccessorWithOnlyAttempted = new ModelMetadataAccessorFor<string>("attempted", () => null);
			AccessorWithOnlyModel = new ModelMetadataAccessorFor<string>(null, () => "model");
		}

		private ModelMetadataAccessorFor<string> AccessorWithNeitherAttemptedOrModel { get; set; }
		private ModelMetadataAccessorFor<string> AccessorWithBothAttemptedAndModel { get; set; }
		private ModelMetadataAccessorFor<string> AccessorWithOnlyAttempted { get; set; }
		private ModelMetadataAccessorFor<string> AccessorWithOnlyModel { get; set; }

		[Fact]
		public void ValueShouldAlwaysEqualExplicitValueWhenExplicitValueHaveBeenDefined()
		{
			var sut = new Input("text", "name", AccessorWithBothAttemptedAndModel).Value("explicit");
			sut.ApplyModelState();
			sut.Value().ShouldEqual("explicit");

			sut = new Input("text", "name", AccessorWithNeitherAttemptedOrModel).Value("explicit");
			sut.ApplyModelState();
			sut.Value().ShouldEqual("explicit");

			sut = new Input("text", "name", AccessorWithOnlyAttempted).Value("explicit");
			sut.ApplyModelState();
			sut.Value().ShouldEqual("explicit");

			sut = new Input("text", "name", AccessorWithOnlyModel).Value("explicit");
			sut.ApplyModelState();
			sut.Value().ShouldEqual("explicit");
		}

		[Fact]
		public void ValueShouldEqualAttemptedValueWhenNoExplicitValueHaveBeenDefined()
		{
			var sut = new Input("text", "name", AccessorWithBothAttemptedAndModel);
			sut.ApplyModelState();
			sut.Value().ShouldEqual("attempted");

			sut = new Input("text", "name", AccessorWithOnlyAttempted);
			sut.ApplyModelState();
			sut.Value().ShouldEqual("attempted");
		}

		[Fact]
		public void ValueShouldEqualModelValueWhenNeitherExplicitValueOrAttemptedValueHaveBeenDefined()
		{
			var sut = new Input("text", "name", AccessorWithOnlyModel);
			sut.ApplyModelState();
			sut.Value().ShouldEqual("model");
		}

		[Fact]
		public void ValueShouldBeNullWhenNeitherModelValueOrAttemptedModelOrExplicitValueHaveBeenDefined()
		{
			var sut = new Input("text", "name", AccessorWithNeitherAttemptedOrModel);
			sut.ApplyModelState();
			sut.Value().ShouldBeNull();
		}
	}
}