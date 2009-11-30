using System.Linq.Expressions;
using Maxfire.Web.Mvc.FluentHtml.Html;

namespace Maxfire.Web.Mvc.FluentHtml.Elements
{
	/// <summary>
	/// Base class for a literal (text inside a span element).
	/// </summary>
	public abstract class LiteralBase<T> : Element<T> where T : LiteralBase<T>
	{
		private string _format;
		protected object _rawValue;

		protected LiteralBase(MemberExpression forMember) :
			base(HtmlTag.Span, forMember)
		{
		}

		protected LiteralBase() :
			base(HtmlTag.Span)
		{
		}

		/// <summary>
		/// Set the inner text of the span element.
		/// </summary>
		/// <param name="value">The value of the inner text.</param>
		public virtual T Value(object value)
		{
			_rawValue = value;
			return (T)this;
		}

		/// <summary>
		/// Specify a format string to be applied to the value.  The format string can be either a
		/// specification (e.g., '$#,##0.00') or a placeholder (e.g., '{0:$#,##0.00}').
		/// </summary>
		/// <param name="value">A format string.</param>
		public virtual T Format(string value)
		{
			_format = value;
			return (T)this;
		}

		public override string ToString()
		{
			SetInnerText(FormatValue(_rawValue));
			return base.ToString();
		}

		protected virtual string FormatValue(object value)
		{
			return string.IsNullOrEmpty(_format)
			       	? value == null
			       	  	? null
			       	  	: value.ToString()
			       	: (_format.StartsWith("{0") && _format.EndsWith("}"))
			       	  	? string.Format(_format, value)
			       	  	: string.Format("{0:" + _format + "}", value);
		}
	}
}