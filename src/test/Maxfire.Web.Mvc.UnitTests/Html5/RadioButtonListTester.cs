using System.Web.Mvc;
using Maxfire.TestCommons.AssertExtensions;
using Maxfire.Web.Mvc.Html5.Elements;
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
			options.ToHtmlString().ShouldEqual(@"<input id=""country"" name=""country"" type=""radio"" value=""value0"">text0</input><input checked=""checked"" id=""country"" name=""country"" type=""radio"" value=""value1"">text1</input>");
		}
	}
}