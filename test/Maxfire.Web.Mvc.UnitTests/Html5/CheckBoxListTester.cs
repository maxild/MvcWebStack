using System.Collections.Generic;
using System.Linq;
using Maxfire.Prelude.Linq;
using Maxfire.TestCommons.AssertExtensions;
using Maxfire.Web.Mvc.Html5;
using Maxfire.Web.Mvc.Html5.Elements;
using Xunit;

namespace Maxfire.Web.Mvc.UnitTests.Html5
{
	public class CheckBoxListTester
	{
		public CheckBoxListTester()
		{
			AccessorWithNeitherAttemptedOrModel = new ModelMetadataAccessorFor<IEnumerable<string>>(null, () => null);
			AccessorWithBothAttemptedAndModel = new ModelMetadataAccessorFor<IEnumerable<string>>("attempted", () => new[] {"model"});
			AccessorWithOnlyAttempted = new ModelMetadataAccessorFor<IEnumerable<string>>("attempted", () => null);
			AccessorWithOnlyModel = new ModelMetadataAccessorFor<IEnumerable<string>>(null, () => new[] { "model" });
		}

		private ModelMetadataAccessorFor<IEnumerable<string>> AccessorWithNeitherAttemptedOrModel { get; set; }
		private ModelMetadataAccessorFor<IEnumerable<string>> AccessorWithBothAttemptedAndModel { get; set; }
		private ModelMetadataAccessorFor<IEnumerable<string>> AccessorWithOnlyAttempted { get; set; }
		private ModelMetadataAccessorFor<IEnumerable<string>> AccessorWithOnlyModel { get; set; }

		[Fact]
		public void ValueShouldAlwaysEqualExplicitValueWhenExplicitValueHaveBeenDefined()
		{
			new IModelMetadataAccessor<IEnumerable<string>>[]
				{
					AccessorWithBothAttemptedAndModel,
					AccessorWithNeitherAttemptedOrModel,
					AccessorWithOnlyAttempted,
					AccessorWithOnlyModel
				}
				.Each(accessor =>
					{
						var sut = new CheckBoxList("name", accessor).SelectedValues(new [] {"explicit"});
						sut.ApplyModelState();
						sut.SelectedValues().First().ShouldEqual("explicit");
					});
		}

		[Fact]
		public void ValueShouldEqualAttemptedValueWhenNoExplicitValueHaveBeenDefined()
		{
			var sut = new CheckBoxList("name", AccessorWithBothAttemptedAndModel);
			sut.ApplyModelState();
			sut.SelectedValues().First().ShouldEqual("attempted");

			sut = new CheckBoxList("name", AccessorWithOnlyAttempted);
			sut.ApplyModelState();
			sut.SelectedValues().First().ShouldEqual("attempted");
		}

		[Fact]
		public void ValueShouldEqualModelValueWhenNeitherExplicitValueOrAttemptedValueHaveBeenDefined()
		{
			var sut = new CheckBoxList("name", AccessorWithOnlyModel);
			sut.ApplyModelState();
			sut.SelectedValues().First().ShouldEqual("model");
		}

		[Fact]
		public void ValueShouldBeNullWhenNeitherModelValueOrAttemptedModelOrExplicitValueHaveBeenDefined()
		{
			var sut = new CheckBoxList("name", AccessorWithNeitherAttemptedOrModel);
			sut.ApplyModelState();
			sut.SelectedValues().ShouldBeEmpty();
		}
	}
}
