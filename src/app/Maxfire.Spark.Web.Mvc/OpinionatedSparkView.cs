using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Maxfire.Web.Mvc;
using Maxfire.Web.Mvc.FluentHtml.Behaviors;
using Spark.Web.Mvc;

namespace Maxfire.Spark.Web.Mvc
{
	public abstract class OpinionatedSparkView : SparkView, IUrlResponseWriter
	{
		private OpinionatedSparkHtmlHelper _htmlHelper;
		public new OpinionatedSparkHtmlHelper Html
		{
			get { return _htmlHelper ?? (_htmlHelper = new OpinionatedSparkHtmlHelper(ViewContext, this, NameValueSerializer)); }
		}

		public string GetVirtualPath(RouteValueDictionary routeValues)
		{
			return UrlHelperUtil.GetVirtualPath(RouteTable.Routes, ViewContext.RequestContext, routeValues);
		}

		// Because we have to resolve to property injection in views, and therefore NameValueSerializer has a setter
		private INameValueSerializer _nameValueSerializer;
		public INameValueSerializer NameValueSerializer
		{
			get { return _nameValueSerializer ?? (_nameValueSerializer = new DefaultNameValueSerializer()); }
			set { _nameValueSerializer = value; }
		}

		public virtual void Render(string html)
		{
			ViewContext.HttpContext.Response.Write(html);
		}

		public bool IsAjaxRequest
		{
			get { return Request != null && Request.IsAjaxRequest(); }
		}
	}

	public abstract class OpinionatedSparkView<TViewModel> : OpinionatedSparkView, IOpinionatedView<TViewModel>
		where TViewModel : class
	{
		private readonly List<IBehaviorMarker> _behaviors = new List<IBehaviorMarker>();

		protected OpinionatedSparkView()
		{
			_behaviors.Add(new ValidationBehavior(() => ViewModelState));
			_behaviors.Add(new LabelMemberBehavior());
			_behaviors.Add(new InputTypeBehavior());
		}

		private ViewDataDictionary<TViewModel> _viewData;
		public new ViewDataDictionary<TViewModel> ViewData
		{
			get
			{
				if (_viewData == null)
				{
					SetViewData(new ViewDataDictionary<TViewModel>());
				}
				return _viewData;
			}
			set { SetViewData(value); }
		}

		protected override void SetViewData(ViewDataDictionary viewData)
		{
			_viewData = new ViewDataDictionary<TViewModel>(viewData);
			base.SetViewData(_viewData);
		}

		public TViewModel Model
		{
			get { return ViewData.Model; }
		}

		private OpinionatedSparkHtmlHelper<TViewModel> _htmlHelper;
		public new OpinionatedSparkHtmlHelper<TViewModel> Html
		{
			get { return _htmlHelper ?? (_htmlHelper = new OpinionatedSparkHtmlHelper<TViewModel>(ViewContext, this, NameValueSerializer)); }
		}

		public IEnumerable<IBehaviorMarker> Behaviors
		{
			get { return _behaviors; }
		}

		public ModelStateDictionary ViewModelState
		{
			get { return ViewData.ModelState; }
		}

		public TViewModel ViewModel
		{
			get { return ViewData.Model; }
		}

		public string HtmlNamePrefix { get; set; }
	}
}