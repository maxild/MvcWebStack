using Maxfire.Web.Mvc.Html5.Elements;
using Xunit;

namespace Maxfire.Web.Mvc.UnitTests.Html5
{
	public class FormFragmentTester
	{
		//[DebuggerDisplay("{ToTagString()}")]
		class TestableFormFragment : FormFragment<TestableFormFragment>
		{
			public TestableFormFragment(string name) 
				: base("input", name, null /*accessor*/)
			{
			}

			protected override void BindValue(object value)
			{
			}
		}

		[Fact]
		public void LabelBefore()
		{
			//var formElement = new TestableFormFragment("firstName");
			//formElement.LabelBefore("Your Name");
			//formElement.VerifyThatElementList().HasCount(2)
			//    .ElementAt(0)
			//        .HasName("label")
			//        .HasAttribute("for", "firstName")
			//        .HasInnerText("Your Name")
			//    .ElementAt(1)
			//        .HasName("input")
			//        .HasAttribute("name", "firstName")
			//        .HasAttribute("id", "firstName");
		}

		[Fact]
		public void LabelBeforeWithAdditionalAttrbutesOnLabel()
		{
			//var formElement = new TestableFormFragment("firstName");
			//formElement.LabelBefore("Your Name", label => label.AddClass("hidden").Attr("form", "myform"));
			//formElement.VerifyThatElementList().HasCount(2)
			//    .ElementAt(0)
			//        .HasName("label")
			//        .HasAttribute("for", "firstName")
			//        .HasAttribute("class", "hidden")
			//        .HasAttribute("form", "myform")
			//        .HasInnerText("Your Name")
			//    .ElementAt(1)
			//        .HasName("input")
			//        .HasAttribute("name", "firstName")
			//        .HasAttribute("id", "firstName");
		}
	}
}