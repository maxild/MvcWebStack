using Maxfire.Web.Mvc.Html5.HtmlTokens;

namespace Maxfire.Web.Mvc.Html5.Elements
{
	/// <summary>
	/// Base class for a single element.
	/// </summary>
	public class Element<T> : Fragment<T> where T : class, IHtmlFragment<T>
	{
		public Element(string tagName) : base(tagName)
		{
		}
		
		public override string ToHtmlString()
		{
			RemoveClass().AddClass(Attr(HtmlAttribute.Class));
			return base.ToHtmlString();
		}
	}
}