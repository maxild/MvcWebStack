using System.Collections.Generic;
using System.Web.Mvc;

namespace Maxfire.Spark.Web.Mvc
{
	// TODO: Skal fjernes da den benytter HtmlHelper<TModel> og ikke OpinionatedHtmlHelper<TModel>
	public abstract class OpinionatedListSparkView<TViewModelElement> :
		OpinionatedSparkView<IEnumerable<TViewModelElement>>
	{
		private HtmlHelper<TViewModelElement> _htmlHelper;

		public new HtmlHelper<TViewModelElement> Html
		{
			get { return _htmlHelper ?? (_htmlHelper = new HtmlHelper<TViewModelElement>(ViewContext, this)); }
		}
	}
}