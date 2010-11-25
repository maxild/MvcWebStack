using Maxfire.Web.Mvc.Html5.HtmlTokens;

namespace Maxfire.Web.Mvc.Html5.Elements
{
	/// <summary>
	/// Base class for a single element.
	/// </summary>
	public abstract class Element<T> : Fragment<T> where T : Element<T>
	{
		protected Element(string tagName) : base(tagName)
		{
		}
		
		public override string ToHtmlString()
		{
			RemoveClass().AddClass(Attr(HtmlAttribute.Class));
			return base.ToHtmlString();
		}
	}
}