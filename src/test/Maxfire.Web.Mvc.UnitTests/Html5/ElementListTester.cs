using System.Collections.Generic;
using System.Web.Mvc;
using Maxfire.TestCommons.AssertExtensions;
using Maxfire.Web.Mvc.Html5.Elements;
using Xunit;

namespace Maxfire.Web.Mvc.UnitTests.Html5
{
	public class ElementListTester
	{
		class OptionElementList : ElementList<OptionElementList>
		{
			private readonly IEnumerator<SelectListItem> _optionsIterator;
			public OptionElementList(string tagName, IEnumerable<SelectListItem> options) : base("option")
			{
				_optionsIterator = options.GetEnumerator();
				RenderAs(TagRenderMode.Normal);
			}

			protected override bool PreRender()
			{
				if (!_optionsIterator.MoveNext())
				{
					return false;
				}
				PreRender(_optionsIterator.Current);
				return true;
			}

			private void PreRender(SelectListItem option)
			{
				Attr("value", option.Value);
				InnerText(option.Text);
				if (option.Selected)
				{
					Attr("selected", "selected");
				}
			}
		}

		[Fact]
		public void Render()
		{
			var options = new OptionElementList("option", new SelectListItem []
			                    {
			                        new SelectListItem { Text = "text0", Value="value0" },
			                        new SelectListItem { Text = "text1", Value="value1", Selected = true}
			                    });
			options.ToHtmlString().ShouldEqual(@"<option value=""value0"">text0</option><option selected=""selected"" value=""value1"">text1</option>");
		}
	}
}