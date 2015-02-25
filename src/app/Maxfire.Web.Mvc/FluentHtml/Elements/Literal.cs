using System.Linq.Expressions;

namespace Maxfire.Web.Mvc.FluentHtml.Elements
{
	/// <summary>
	/// Generate a literal (text inside a span element).
	/// </summary>
	public class Literal : LiteralBase<Literal>
	{
		/// <summary>
		/// Generates a literal element (span).
		/// </summary>
		/// <param name="forMember">Expression indicating the view model member assocaited with the element</param>
		public Literal(MemberExpression forMember) :
			base(forMember)
		{
		}

		public Literal()
		{
		}
	}
}