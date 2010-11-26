using Maxfire.Web.Mvc.Html5.Elements;
using Maxfire.Web.Mvc.Html5.HtmlTokens;

namespace Maxfire.Web.Mvc.Html5.Mixins
{
	public interface ISupportsChecked<out T>
	{
		T Checked(bool value);
		bool Checked();
	}

	public class CheckedMixin<T> : FragmentMixin<T>, ISupportsChecked<T> where T : CheckedMixin<T>, IHtmlFragment<T>
	{
		public CheckedMixin(IHtmlFragment<T> fragment) : base(fragment)
		{
		}

		/// <summary>
		/// Set the checked attribute.
		/// </summary>
		/// <param name="value">Whether the checkbox should be checked.</param>
		public T Checked(bool value)
		{
			if (value)
			{
				elem.Attr(HtmlAttribute.Checked, HtmlAttribute.Checked);
			}
			else
			{
				elem.RemoveAttr(HtmlAttribute.Checked);
			}
			return self;
		}

		/// <summary>
		/// Determine if checkbox is checked.
		/// </summary>
		/// <remarks>Equivalent to ValueAs&lt;bool&gt;</remarks>
		public bool Checked()
		{
			return elem.HasAttr(HtmlAttribute.Checked);
		}
	}
}