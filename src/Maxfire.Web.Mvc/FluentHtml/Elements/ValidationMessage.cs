using System.Linq.Expressions;

namespace Maxfire.Web.Mvc.FluentHtml.Elements
{
	/// <summary>
	/// Generate a validation message (text inside a span element).
	/// </summary>
	public class ValidationMessage : LiteralBase<ValidationMessage>
	{
		public ValidationMessage(MemberExpression forMember)
			: base(forMember) { }

		public override string ToString()
		{
			if (_rawValue == null)
			{
				return null;
			}
			AddCssClass("field-validation-error");
			return base.ToString();
		}
	}
}