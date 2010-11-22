using System;
using System.Collections;
using System.Collections.Generic;
using Maxfire.TestCommons.AssertExtensions;
using Maxfire.Web.Mvc.Html5.Elements;
using Xunit;

namespace Maxfire.Web.Mvc.UnitTests.Html5
{
	public class ElementTester
	{
		public class TestableElement : Element<TestableElement>
		{
			public TestableElement(string tagName) : base(tagName)
			{
			}
		}

		[Fact]
		public void TagName()
		{
			var element = new TestableElement("input");
			element.TagName.ShouldEqual("input");
		}

		[Fact]
		public void TagNameNullThrows()
		{
			Assert.Throws<ArgumentException>(() => new TestableElement(null));
		}

		[Fact]
		public void TagNameEmptyThrows()
		{
			Assert.Throws<ArgumentException>(() => new TestableElement(string.Empty));
		}

		[Fact]
		public void Render_EmptySelfClosingTag()
		{
			var element = new TestableElement("input");
			element.ToString().ShouldEqual("<input />");
		}

		[Fact]
		public void SingleAttr()
		{
			new TestableElement("input").Attr("id", "firstName").Attr("id").ShouldEqual("firstName");
			new TestableElement("input").Attr(new { id = "firstName"}).Attr("id").ShouldEqual("firstName");
			new TestableElement("input").Attr(new Dictionary<string, object> {{ "id", "firstName"}}).Attr("id").ShouldEqual("firstName");
		}

		[Fact]
		public void SingleAttrRemovalOfNonExistent()
		{
			new TestableElement("input").RemoveAttr("name").HasAttr("Name").ShouldBeFalse();
		}

		[Fact]
		public void SingleAttrRemoval()
		{
			new TestableElement("input").Attr("name", "firstName").RemoveAttr("name").HasAttr("Name").ShouldBeFalse();
		}

		[Fact]
		public void RemoveAttr_NonExistent_DoesntThrow()
		{
			new TestableElement("input").RemoveAttr("id");
		}

		[Fact]
		public void Attr_NonExistent_ReturnsNull()
		{
			new TestableElement("input").Attr("id").ShouldBeNull();
		}

		[Fact] 
		public void Render_Attr()
		{
			var element = new TestableElement("input").Attr("id", "firstName");
			element.ToString().ShouldEqual(@"<input id=""firstName"" />");
		}

		[Fact]
		public void AddSingleClass()
		{
			new TestableElement("input").AddClass("myclass").HasClass("myclass").ShouldBeTrue();
		}

		[Fact]
		public void AddMultipleClassUsingOneArg()
		{
			var element = new TestableElement("input").AddClass("myclass anotherclass");
			element.HasClass("myclass").ShouldBeTrue();
			element.HasClass("anotherclass").ShouldBeTrue();
		}

		[Fact]
		public void AddMultipleClassUsingMultipleArgs()
		{
			var element = new TestableElement("input").AddClass("myclass", "anotherclass");
			element.HasClass("myclass").ShouldBeTrue();
			element.HasClass("anotherclass").ShouldBeTrue();
		}

		[Fact]
		public void RemoveClass_NonExistent_DoesntThrow()
		{
			new TestableElement("input").RemoveClass("someclass");
		}

		[Fact]
		public void ToogleClass_NonExistent()
		{
			new TestableElement("input").AddClass("someclass").ToggleClass("someclass").HasClass("someclass").ShouldBeFalse();
		}

		[Fact]
		public void ToogleClass_Existent()
		{
			new TestableElement("input").ToggleClass("someclass").HasClass("someclass").ShouldBeTrue();
		}

		[Fact]
		public void ToggleClass_Switch()
		{
			new TestableElement("input").AddClass("someclass").ToggleClass("someclass", true).HasClass("someclass").ShouldBeTrue();
			new TestableElement("input").AddClass("someclass").ToggleClass("someclass", false).HasClass("someclass").ShouldBeFalse();
			new TestableElement("input").ToggleClass("someclass", true).HasClass("someclass").ShouldBeTrue();
			new TestableElement("input").ToggleClass("someclass", false).HasClass("someclass").ShouldBeFalse();
		}
	}
}