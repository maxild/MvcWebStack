using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using Maxfire.Web.Mvc.FluentHtml.Html;

namespace Maxfire.Web.Mvc.FluentHtml.Elements
{
	/// <summary>
	/// Base class for select elements.
	/// </summary>
	/// <typeparam name="T">The derived type.</typeparam>
	public abstract class SelectBase<T> : OptionsElementBase<T> where T : SelectBase<T>
	{
		protected string _firstOptionText;

		protected SelectBase(string name, MemberExpression forMember)
			: base(HtmlTag.Select, name, forMember)
		{
		}

		/// <summary>
		/// Set the 'size' attribute.
		/// </summary>
		/// <param name="value">The value of the 'size' attribute.</param>
		/// <returns></returns>
		public virtual T Size(int value)
		{
			Attr(HtmlAttribute.Size, value);
			return (T)this;
		}

		protected override void PreRender()
		{
			SetInnerHtml(renderOptions());
			base.PreRender();
		}

		protected override TagRenderMode TagRenderMode
		{
			get { return TagRenderMode.Normal; }
		}

		private string renderOptions()
		{
			if (_options == null)
			{
				return null;
			}

			var sb = new StringBuilder();

			var hasSelectedValue = _options.Any(option => IsSelectedValue(option.Value));
			if (_firstOptionText != null && !hasSelectedValue)
			{
				sb.Append(GetFirstOption());
			}

			foreach (var option in _options)
			{
				if (option != null)
				{
					sb.Append(GetOption(option));
				}
			}
			
			return sb.ToString();
		}

		// Note: By the HTML spec, the text of the OPTION is used as the value, if the VALUE is empty.
		// See also http://www.w3.org/MarkUp/html-spec/html-spec_8.html#SEC8.1.3.1

		protected virtual Option GetFirstOption()
		{
			// Because no value is selected the 'no value' option must be selected
			return new Option()
				.Text(_firstOptionText)
				.Value(string.Empty)
				.Selected(true);
		}

		protected virtual Option GetOption(ITextValuePair option)
		{
			return new Option()
				.Text(option.Text)
				.Value(option.Value)
				.Selected(IsSelectedValue(option.Value));
		}
	}
}