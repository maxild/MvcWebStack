using Maxfire.Web.Mvc.FluentHtml.Extensions;
using Maxfire.Web.Mvc.FluentHtml.Html;

namespace Maxfire.Web.Mvc.FluentHtml.Elements
{
	/// <summary>
	/// Generate an HTML input element of type 'submit.'
	/// </summary>
	public class SubmitButton : Input<SubmitButton>
	{
		/// <summary>
		/// Generate an HTML input element of type 'submit.'
		/// </summary>
		/// <param name="text">Value of the 'value' and 'name' attributes. Also used to derive the 'id' attribute.</param>
		public SubmitButton(string text)
			: base(HtmlInputType.Submit, text.FormatAsHtmlName(), null)
		{
			Value(text);
		}
	}
}