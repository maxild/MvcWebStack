using System.Linq.Expressions;
using Maxfire.Web.Mvc.FluentHtml.Html;

namespace Maxfire.Web.Mvc.FluentHtml.Elements
{
	/// <summary>
	/// A set of radio buttons.
	/// </summary>
	public class RadioSet : RadioSetBase<RadioSet>
	{
		public RadioSet(string name, MemberExpression forMember)
			: base(HtmlTag.Div, name, forMember)
		{
		}
	}
}