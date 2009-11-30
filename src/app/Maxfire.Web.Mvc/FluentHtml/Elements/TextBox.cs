using System.Linq.Expressions;
using Maxfire.Web.Mvc.FluentHtml.Html;

namespace Maxfire.Web.Mvc.FluentHtml.Elements
{
	/// <summary>
	/// Generate an HTML input element of type 'text.'
	/// </summary>
	public class TextBox : TextInput<TextBox>
	{
		/// <summary>
		/// Generate an HTML input element of type 'text.'
		/// </summary>
		/// <param name="name">Value of the 'name' attribute of the element.  Also used to derive the 'id' attribute.</param>
		/// <param name="forMember">Expression indicating the view model member assocaited with the element</param>
		public TextBox(string name, MemberExpression forMember)
			: base(HtmlInputType.Text, name, forMember) { }
	}
}