using System;
using System.Collections.Generic;
using Maxfire.TestCommons.AssertExtensions;
using Maxfire.Web.Mvc.Html5.Elements;
using Maxfire.Web.Mvc.UnitTests.Html5.AssertionExtensions;
using Xunit;

namespace Maxfire.Web.Mvc.UnitTests.Html5
{
	public class FragmentTester
	{
		public class TestableFragment : Fragment<TestableFragment>
		{
			public TestableFragment(string elementName) : base(elementName)
			{
			}
		}

		[Fact]
		public void TagName()
		{
			var element = new TestableFragment("input");
			element.VerifyThatElement().HasName("input");
		}

		[Fact]
		public void TagNameNullThrows()
		{
			Assert.Throws<ArgumentException>(() => new TestableFragment(null));
		}

		[Fact]
		public void TagNameEmptyThrows()
		{
			Assert.Throws<ArgumentException>(() => new TestableFragment(string.Empty));
		}

		[Fact]
		public void Render_EmptySelfClosingTagByDefault()
		{
			var element = new TestableFragment("input");
			element.ToString().ShouldEqual("<input />");
		}

		[Fact]
		public void SingleAttr()
		{
			new TestableFragment("input").Attr("id", "firstName").Attr("id").ShouldEqual("firstName");
			new TestableFragment("input").Attr(new { id = "firstName"}).Attr("id").ShouldEqual("firstName");
			new TestableFragment("input").Attr(new Dictionary<string, object> {{ "id", "firstName"}}).Attr("id").ShouldEqual("firstName");
		}

		[Fact]
		public void SingleAttrRemovalOfNonExistent()
		{
			new TestableFragment("input").RemoveAttr("name").HasAttr("Name").ShouldBeFalse();
		}

		[Fact]
		public void SingleAttrRemoval()
		{
			new TestableFragment("input").Attr("name", "firstName").RemoveAttr("name").HasAttr("Name").ShouldBeFalse();
		}

		[Fact]
		public void RemoveAttr_NonExistent_DoesntThrow()
		{
			new TestableFragment("input").RemoveAttr("id");
		}

		[Fact]
		public void Attr_NonExistent_ReturnsNull()
		{
			new TestableFragment("input").Attr("id").ShouldBeNull();
		}

		[Fact] 
		public void Render_Attr()
		{
			var element = new TestableFragment("input").Attr("id", "firstName");
			element.VerifyThatElement().HasName("input").HasAttribute("id", "firstName");
		}

		[Fact]
		public void AddSingleClass()
		{
			new TestableFragment("input").AddClass("myclass").HasClass("myclass").ShouldBeTrue();
		}

		[Fact]
		public void Render_SingleClass()
		{
			new TestableFragment("input").AddClass("myclass")
				.ToHtmlString().ShouldEqual(@"<input class=""myclass"" />");
		}

		[Fact]
		public void Render_SingleClass_MultipleTimes()
		{
			var sut = new TestableFragment("input").AddClass("myclass");
			sut.ToHtmlString().ShouldEqual(@"<input class=""myclass"" />");
			sut.ToHtmlString().ShouldEqual(@"<input class=""myclass"" />");
		}

		[Fact]
		public void Render_MultipleClasses()
		{
			new TestableFragment("input").AddClass("myclass anotherclass", "yetanotherclass")
				.ToHtmlString().ShouldEqual(@"<input class=""anotherclass myclass yetanotherclass"" />");
		}

		[Fact]
		public void Render_MultipleClasses_MultipleTimes()
		{
			var sut = new TestableFragment("input").AddClass("myclass anotherclass", "yetanotherclass");
			sut.ToHtmlString().ShouldEqual(@"<input class=""anotherclass myclass yetanotherclass"" />");
			sut.ToHtmlString().ShouldEqual(@"<input class=""anotherclass myclass yetanotherclass"" />");
		}

		[Fact]
		public void AddMultipleClassUsingOneArg()
		{
			var element = new TestableFragment("input").AddClass("myclass anotherclass");
			element.HasClass("myclass").ShouldBeTrue();
			element.HasClass("anotherclass").ShouldBeTrue();
		}

		[Fact]
		public void AddMultipleClassUsingMultipleArgs()
		{
			var element = new TestableFragment("input").AddClass("myclass", "anotherclass");
			element.HasClass("myclass").ShouldBeTrue();
			element.HasClass("anotherclass").ShouldBeTrue();
		}

		[Fact]
		public void RemoveClass_NonExistent_DoesntThrow()
		{
			new TestableFragment("input").RemoveClass("someclass");
		}

		[Fact]
		public void ToogleClass_NonExistent()
		{
			new TestableFragment("input").AddClass("someclass").ToggleClass("someclass").HasClass("someclass").ShouldBeFalse();
		}

		[Fact]
		public void ToogleClass_Existent()
		{
			new TestableFragment("input").ToggleClass("someclass").HasClass("someclass").ShouldBeTrue();
		}

		[Fact]
		public void ToggleClass_Switch()
		{
			new TestableFragment("input").AddClass("someclass").ToggleClass("someclass", true).HasClass("someclass").ShouldBeTrue();
			new TestableFragment("input").AddClass("someclass").ToggleClass("someclass", false).HasClass("someclass").ShouldBeFalse();
			new TestableFragment("input").ToggleClass("someclass", true).HasClass("someclass").ShouldBeTrue();
			new TestableFragment("input").ToggleClass("someclass", false).HasClass("someclass").ShouldBeFalse();
		}

		[Fact]
		public void Attr_MergeClassNames()
		{
			var element = new TestableFragment("input").AddClass("someclass").Attr(new {@class = "anotherclass someclass xfiles"});
			element.HasClass("anotherclass xfiles someclass").ShouldBeTrue();
			element.HasAttr("class").ShouldBeTrue();
		}
	}
}