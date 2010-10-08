using System.Collections.Generic;
using System.Web.Mvc;

namespace Maxfire.Web.Mvc
{
	public abstract class OpinionatedListSparkView<TViewModelElement> : OpinionatedSparkView<IEnumerable<TViewModelElement>>
	{
		private HtmlHelper<TViewModelElement> _htmlHelper;

		public new HtmlHelper<TViewModelElement> Html
		{
			get { return _htmlHelper ?? (_htmlHelper = new HtmlHelper<TViewModelElement>(ViewContext, this)); }
		}
	}
}