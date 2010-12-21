using Maxfire.Web.Mvc.Html5.HtmlTokens;

namespace Maxfire.Web.Mvc.Html5.Elements
{
	public abstract class Element<T> : Fragment<T> where T : Element<T>
	{
		protected Element(string elementName) : base(elementName)
		{
		}

		protected override string ToTagString()
		{
			RemoveClass().AddClass(Attr(HtmlAttribute.Class));
			return base.ToTagString();
		}
	}
}