using Maxfire.Web.Mvc.Html5.Elements;
using Maxfire.Web.Mvc.Html5.HtmlTokens;

namespace Maxfire.Web.Mvc.Html5.Mixins
{
	public interface ISupportsSelected<out T>
	{
		T Selected(bool value);
		bool Selected();
	}

	public class SelectedMixin<T> : FragmentMixin<T>, ISupportsSelected<T> where T : class, IHtmlFragment<T>
	{
		public SelectedMixin(IHtmlFragment<T> fragment) : base(fragment)
		{
		}

		/// <summary>
		/// Set the selected attribute.
		/// </summary>
		/// <param name="value">Whether the option should be selected.</param>
		public T Selected(bool value)
		{
			if (value)
			{
				elem.Attr(HtmlAttribute.Selected, HtmlAttribute.Selected);
			}
			else
			{
				elem.RemoveAttr(HtmlAttribute.Selected);
			}
			return self;
		}

		/// <summary>
		/// Determine if option is selected.
		/// </summary>
		public bool Selected()
		{
			return elem.HasAttr(HtmlAttribute.Selected);
		}
	}
}