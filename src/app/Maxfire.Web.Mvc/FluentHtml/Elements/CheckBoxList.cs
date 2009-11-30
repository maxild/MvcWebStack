using System.Linq.Expressions;
using Maxfire.Web.Mvc.FluentHtml.Html;

namespace Maxfire.Web.Mvc.FluentHtml.Elements
{
	/// <summary>
	/// A list of checkboxes buttons.
	/// </summary>
	public class CheckBoxList : CheckBoxListBase<CheckBoxList>
	{
		public CheckBoxList(string name, MemberExpression forMember)
			: base(HtmlTag.Div, name, forMember)
		{
		}
	}
}