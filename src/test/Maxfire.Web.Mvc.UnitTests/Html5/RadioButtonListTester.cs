using System.Web.Mvc;
using Maxfire.Web.Mvc.Html5.Elements;
using Maxfire.Web.Mvc.UnitTests.Html5.AssertionExtensions;
using Xunit;

namespace Maxfire.Web.Mvc.UnitTests.Html5
{
	public class RadioButtonListTester
	{
		[Fact]
		public void Render()
		{
			var options = new RadioButtonList("country", null).Options.FromSelectListItems(new []
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