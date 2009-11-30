using System.Linq.Expressions;
using Maxfire.Web.Mvc.FluentHtml.Extensions;
using Maxfire.Web.Mvc.FluentHtml.Html;

namespace Maxfire.Web.Mvc.FluentHtml.Elements
{
	public class RadioButton : Input<RadioButton>
	{
		private string _format;
		
		public RadioButton(string name, MemberExpression forMember)
			: base(HtmlInputType.Radio, name, forMember)
		{
		}

		/// <summary>
		/// Set the checked attribute.
		/// </summary>
		/// <param name="value">Whether the radio button should be checked.</param>
		public RadioButton Checked(bool value)
		{
			if (value)
			{
				SetAttr(HtmlAttribute.Checked, HtmlAttribute.Checked);
			}
			else
			{
				RemoveAttr(HtmlAttribute.Checked);
			}
			return this;
		}

		/// <summary>
		/// Specify a format string for the HTML output.
		/// </summary>
		/// <param name="format">A format string.</param>
		public virtual RadioButton Format(string format)
		{
			_format = format;
			return this;
		}

		protected override void InferIdFromName()
		{
			if (!HasId())
			{
				SetId(string.Format("{0}{1}",
				                    GetName(),
				                    _value == null
				                    	? null
				                    	: string.Format("_{0}", _value)).FormatAsHtmlId());
			}
		}

		public override string ToString()
		{
			return _format == null
			       	? base.ToString()
			       	: string.Format(_format, base.ToString());
		}
	}
}