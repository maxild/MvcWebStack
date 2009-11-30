using System.Linq.Expressions;

namespace Maxfire.Web.Mvc.FluentHtml
{
	/// <summary>
	/// Interface for elements that are associated with a model member.
	/// </summary>
	public interface IMemberElement : IElement
	{
		/// <summary>
		/// Expression indicating the view model member associated with the element.</param>
		/// </summary>
		MemberExpression ForMember { get; }
	}
}