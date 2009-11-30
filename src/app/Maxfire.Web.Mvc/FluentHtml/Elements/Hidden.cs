using System.Linq.Expressions;
using Maxfire.Web.Mvc.FluentHtml.Html;

namespace Maxfire.Web.Mvc.FluentHtml.Elements
{
	/// <summary>
	/// Generates an HTML input element of type 'hidden.'
	/// </summary>
	public class Hidden : TextInput<Hidden>
	{
		public Hidden(string name) : this(name, null)
		{
		}

		/// <summary>
		/// Generates an HTML input element of type 'hidden.'
		/// </summary>
		/// <param name="name">Value of the 'name' attribute of the element.  Also used to derive the 'id' attribute.</param>
		/// <param name="forMember">Expression indicating the model member assocaited with the element.</param>
		public Hidden(string name, MemberExpression forMember)
			: base(HtmlInputType.Hidden, name, forMember)
		{
		}
	}
}