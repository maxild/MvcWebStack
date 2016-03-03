using System.Linq.Expressions;
using Maxfire.Web.Mvc.FluentHtml.Behaviors;
using Maxfire.Web.Mvc.FluentHtml.Html;

namespace Maxfire.Web.Mvc.FluentHtml.Elements
{
	/// <summary>
	/// Base class for text input elements.
	/// </summary>
	/// <typeparam name="T">The derived type</typeparam>
	public abstract class TextInput<T> : Input<T>, ISupportsMaxLength 
		where T : TextInput<T>
	{
		protected TextInput(string type, string name, MemberExpression forMember)
			: base(type, name, forMember)
		{
		}

		/// <summary>
		/// Set the 'maxlength' attribute. 
		/// </summary>
		/// <param name="value">Value for the maxlength attribute.</param>
		public T MaxLength(int value)
		{
			((ISupportsMaxLength)this).SetMaxLength(value);
			return (T)this;
		}

		void ISupportsMaxLength.SetMaxLength(int value)
		{
			SetAttr(HtmlAttribute.MaxLength, value);
		}
	}
}