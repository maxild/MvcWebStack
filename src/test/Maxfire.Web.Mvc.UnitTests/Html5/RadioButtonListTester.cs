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
		public void RenderWithoutWrapping()
		{
			var options = new RadioButtonList("remember", null).Options.FromSelectListItems(new[]
				{
					new SelectListItem { Text = "Yes", Value="yes" },
					new SelectListItem { Text = "No", Value="no", Selected = true}
				}).Attr(new {@class="radio"}).LabelAttr(new {@class="big-label"});

			options.VerifyThatElementList().HasCount(4)
					.ElementAt(0)
						.HasName("input")
						.HasAttribute("id", "remember-yes")
						.HasAttribute("name", "remember")
						.HasAttribute("type", "radio")
						.HasAttribute("value", "yes")
						.HasAttribute("class", "radio")
					.ElementAt(1)
						.HasName("label")
						.HasAttribute("for", "remember-yes")
						.HasAttribute("class", "big-label")
						.HasInnerText("Yes")
					.ElementAt(2)
						.HasName("input")
						.HasAttribute("id", "remember-no")
						.HasAttribute("name", "remember")
						.HasAttribute("type", "radio")
						.HasAttribute("value", "no")
						.HasAttribute("class", "radio")
					.ElementAt(3)
						.HasName("label")
						.HasAttribute("for", "remember-no")
						.HasAttribute("class", "big-label")
						.HasInnerText("No");
		}

		[Fact]
		public void RenderWithWrapping()
		{
			var options = new RadioButtonList("remember", null).Options.FromSelectListItems(new[]
				{
					new SelectListItem { Text = "Yes", Value="yes" },
					new SelectListItem { Text = "No", Value="no", Selected = true}
				})
				.Wrap("<p>{0}</p>");

			options.VerifyThatElementList().HasCount(2)
			       .ElementAt(0).HasName("p")
						.GoToChildNodes().HasCount(2)
							.ElementAt(0)
								.HasName("input")
								.HasAttribute("id", "remember-yes")
								.HasAttribute("name", "remember")
								.HasAttribute("type", "radio")
								.HasAttribute("value", "yes")
							.ElementAt(1)
								.HasName("label")
								.HasAttribute("for", "remember-yes")
								.HasInnerText("Yes")
						.GoToParentNodes()
			       .ElementAt(1).HasName("p")
						.GoToChildNodes().HasCount(2)
							.ElementAt(0)
								.HasName("input")
								.HasAttribute("id", "remember-no")
								.HasAttribute("name", "remember")
								.HasAttribute("type", "radio")
								.HasAttribute("value", "no")
							.ElementAt(1)
								.HasName("label")
								.HasAttribute("for", "remember-no")
								.HasInnerText("No");
		}
	}
}