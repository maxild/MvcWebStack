using System.Web.Mvc;
using Maxfire.Core.Extensions;
using Maxfire.TestCommons.AssertExtensions;
using Maxfire.Web.Mvc.Html5;
using Maxfire.Web.Mvc.Html5.Elements;
using Maxfire.Web.Mvc.UnitTests.Html5.AssertionExtensions;
using Xunit;

namespace Maxfire.Web.Mvc.UnitTests.Html5
{
	public class RadioButtonListTester
	{
		public RadioButtonListTester()
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
						var sut = new RadioButtonList("name", accessor).SelectedValue("explicit");
						sut.ApplyModelState();
						sut.SelectedValue().ShouldEqual("explicit");
					});
		}

		[Fact]
		public void ValueShouldEqualAttemptedValueWhenNoExplicitValueHaveBeenDefined()
		{
			var sut = new RadioButtonList("name", AccessorWithBothAttemptedAndModel);
			sut.ApplyModelState();
			sut.SelectedValue().ShouldEqual("attempted");

			sut = new RadioButtonList("name", AccessorWithOnlyAttempted);
			sut.ApplyModelState();
			sut.SelectedValue().ShouldEqual("attempted");
		}

		[Fact]
		public void ValueShouldEqualModelValueWhenNeitherExplicitValueOrAttemptedValueHaveBeenDefined()
		{
			var sut = new RadioButtonList("name", AccessorWithOnlyModel);
			sut.ApplyModelState();
			sut.SelectedValue().ShouldEqual("model");
		}

		[Fact]
		public void ValueShouldBeNullWhenNeitherModelValueOrAttemptedModelOrExplicitValueHaveBeenDefined()
		{
			var sut = new RadioButtonList("name", AccessorWithNeitherAttemptedOrModel);
			sut.ApplyModelState();
			sut.SelectedValue().ShouldBeNull();
		}

		[Fact]
		public void Render()
		{
			var options = new RadioButtonList("country", null).Options.FromSelectListItems(new[]
				{
					new SelectListItem { Text = "text0", Value="value0" },
					new SelectListItem { Text = "text1", Value="value1", Selected = true}
				});
			options.VerifyThatElementList().HasCount(2)
			       .ElementAt(0).HasName("input")
			       .DoesntHaveAttribute("id")
			       .HasAttribute("name", "country")
			       .HasAttribute("type", "radio")
			       .HasAttribute("value", "value0")
			       .HasInnerText("text0")
			       .ElementAt(1).HasName("input")
			       .DoesntHaveAttribute("id")
			       .HasAttribute("name", "country")
			       .HasAttribute("type", "radio")
			       .HasAttribute("value", "value1")
			       .HasInnerText("text1");
		}
	}
}