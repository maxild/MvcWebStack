using Maxfire.Core.Extensions;
using Maxfire.TestCommons.AssertExtensions;
using Maxfire.Web.Mvc.Html5;
using Maxfire.Web.Mvc.Html5.Elements;
using Xunit;

namespace Maxfire.Web.Mvc.UnitTests.Html5
{
	public class SelectTester
	{
		public SelectTester()
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
			new IModelMetadataAccessor<string>[]
				{
					AccessorWithBothAttemptedAndModel,
					AccessorWithNeitherAttemptedOrModel,
					AccessorWithOnlyAttempted,
					AccessorWithOnlyModel
				}
				.Each(accessor =>
					{
						var sut = new Select("name", accessor).SelectedValue("explicit");
						sut.ApplyModelState();
						sut.SelectedValue().ShouldEqual("explicit");
					});
		}

		[Fact]
		public void ValueShouldEqualAttemptedValueWhenNoExplicitValueHaveBeenDefined()
		{
			var sut = new Select("name", AccessorWithBothAttemptedAndModel);
			sut.ApplyModelState();
			sut.SelectedValue().ShouldEqual("attempted");

			sut = new Select("name", AccessorWithOnlyAttempted);
			sut.ApplyModelState();
			sut.SelectedValue().ShouldEqual("attempted");
		}

		[Fact]
		public void ValueShouldEqualModelValueWhenNeitherExplicitValueOrAttemptedValueHaveBeenDefined()
		{
			var sut = new Select("name", AccessorWithOnlyModel);
			sut.ApplyModelState();
			sut.SelectedValue().ShouldEqual("model");
		}

		[Fact]
		public void ValueShouldBeNullWhenNeitherModelValueOrAttemptedModelOrExplicitValueHaveBeenDefined()
		{
			var sut = new Select("name", AccessorWithNeitherAttemptedOrModel);
			sut.ApplyModelState();
			sut.SelectedValue().ShouldBeNull();
		}
	}
}