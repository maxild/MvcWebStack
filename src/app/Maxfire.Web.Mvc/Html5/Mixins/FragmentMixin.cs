using Maxfire.Web.Mvc.Html5.Elements;

namespace Maxfire.Web.Mvc.Html5.Mixins
{
	/// <summary>
	/// Base class for mixin that uses the fragment API.
	/// </summary>
	public abstract class FragmentMixin<T> : Mixin<T> where T : class, IHtmlFragment<T>
	{
		private readonly IHtmlFragment<T> _fragment;

		protected FragmentMixin(IHtmlFragment<T> fragment)
		{
			_fragment = fragment;
		}

		protected IHtmlFragment<T> elem { get { return _fragment; } }
	}
}