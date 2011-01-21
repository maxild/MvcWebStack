using System.Web.Mvc;
using Maxfire.Web.Mvc;

namespace Maxfire.Spark.Web.Mvc
{
	public class OpinionatedSparkHtmlHelper : OpinionatedHtmlHelper
	{
		private readonly OpinionatedSparkView _view;

		public OpinionatedSparkHtmlHelper(ViewContext viewContext, OpinionatedSparkView view, INameValueSerializer nameValueSerializer) 
			: base(viewContext, view, nameValueSerializer)
		{
			_view = view;
		}

		public override string SiteRoot
		{
			get { return _view.SiteRoot; }
		}

		public override string SiteResource(string path)
		{
			return _view.SiteResource(path);
		}
	}

	public class OpinionatedSparkHtmlHelper<TViewModel> : OpinionatedHtmlHelper<TViewModel>
		where TViewModel : class
	{
		private readonly OpinionatedSparkView _view;

		public OpinionatedSparkHtmlHelper(ViewContext viewContext, OpinionatedSparkView<TViewModel> view, INameValueSerializer nameValueSerializer)
			: base(viewContext, view, nameValueSerializer)
		{
			_view = view;
		}

		public override string SiteRoot
		{
			get { return _view.SiteRoot; }
		}

		public override string SiteResource(string path)
		{
			return _view.SiteResource(path);
		}
	}
}