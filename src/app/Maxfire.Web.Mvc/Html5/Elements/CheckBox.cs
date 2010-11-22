using System.Web.Mvc;
using Maxfire.Web.Mvc.Html5.HtmlTokens;

namespace Maxfire.Web.Mvc.Html5.Elements
{
	/// <summary>
	/// Boolean single-valued checkbox (with hidden control of false)
	/// </summary>
	public class CheckBox : InputElement<CheckBox>
	{
		public CheckBox(string name, ModelMetadata modelMetadata)
			: base(HtmlInputType.Checkbox, name, modelMetadata)
		{
		}

		/// <summary>
		/// Set the checked attribute.
		/// </summary>
		/// <param name="value">Whether the checkbox should be checked.</param>
		public CheckBox Checked(bool value)
		{
			if (value)
			{
				Attr(HtmlAttribute.Checked, HtmlAttribute.Checked);
			}
			else
			{
				RemoveAttr(HtmlAttribute.Checked);
			}
			return self;
		}

		/// <summary>
		/// Determine if checkbox is checked.
		/// </summary>
		/// <remarks>Equivalent to ValueAs&lt;bool&gt;</remarks>
		public bool Checked()
		{
			return HasAttr(HtmlAttribute.Checked);
		}

		public override object Value()
		{
			return Checked() ? "true,false" :"false";
		}

		public override CheckBox Value(object value)
		{
			// Setting value other than "true" is not supported...we fail silently using a noop.
			return self;
		}

		public override string ToString()
		{
			string checkbox = base.Value("true").ToString();
			string hidden = new Hidden(Attr(HtmlAttribute.Name), ModelMetadata).Value("false").ToString();
			return string.Concat(checkbox, hidden);
		}
	}
}
