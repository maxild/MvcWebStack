using System.Web.Mvc;
using Maxfire.Core.Extensions;
using Maxfire.Web.Mvc.FluentHtml.Extensions;
using Maxfire.Web.Mvc.FluentHtml.Html;

namespace Maxfire.Web.Mvc.FluentHtml.Elements
{
	/// <summary>
	/// Generate a select option.
	/// </summary>
	public class Option
	{
		private const string NBSP = "&nbsp;";
		private readonly TagBuilder builder = new TagBuilder(HtmlTag.Option);

		/// <summary>
		/// Set the value of the 'value' attribute.
		/// </summary>
		/// <param name="value">The value.</param>
		public Option Value(string value)
		{
			if (value.IsNotEmpty())
			{
				builder.AddAttribute(HtmlAttribute.Value, value);
			}
			else
			{
				builder.AddAttribute(HtmlAttribute.Value, string.Empty);
			}
			return this;
		}

		/// <summary>
		/// Set the inner text to the html encoded value.
		/// </summary>
		/// <param name="value">The value of the inner text.</param>
		public virtual Option Text(string value)
		{
			if (value.IsNotEmpty())
			{
				builder.SetInnerText(value);
			}
			else
			{
				builder.InnerHtml = NBSP;
			}
			return this;
		}

		/// <summary>
		/// Set the selected attribute.
		/// </summary>
		/// <param name="value">Whether the option should be selected.</param>
		public virtual Option Selected(bool value)
		{
			if (value)
			{
				builder.MergeAttribute(HtmlAttribute.Selected, HtmlAttribute.Selected, true);
			}
			else
			{
				builder.Attributes.Remove(HtmlAttribute.Selected);
			}
			return this;
		}

		/// <summary>
		/// Set as disabled or enabled.
		/// </summary>
		/// <param name="value">Whether the option is disabled.</param>
		public virtual Option Disabled(bool value)
		{
			if (value)
			{
				builder.MergeAttribute(HtmlAttribute.Disabled, HtmlAttribute.Disabled, true);
			}
			else
			{
				builder.Attributes.Remove(HtmlAttribute.Disabled);
			}
			return this;
		}

		public override string ToString()
		{
			return builder.ToString();
		}
	}
}