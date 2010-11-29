using Maxfire.Web.Mvc.Html5.HtmlTokens;

namespace Maxfire.Web.Mvc.Html5.Elements
{
	public class Option : Fragment<Option>
	{
		public Option() : base(HtmlElement.Option)
		{
		}

		protected override string ToTagString()
		{
			RemoveClass().AddClass(Attr(HtmlAttribute.Class));
			return base.ToTagString();
		}
	}
}