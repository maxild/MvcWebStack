using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Maxfire.Web.Mvc;
using Maxfire.Web.Mvc.FluentHtml.Behaviors;
using Spark.Web.Mvc;

namespace Maxfire.Spark.Web.Mvc
{
	public abstract class OpinionatedSparkView : SparkView, ITempDataContainer, IUrlResponseWriter
	{
		private readonly INameValueSerializer _nameValueSerializer;

		protected OpinionatedSparkView() : this(new DefaultNameValueSerializer())
		{
		}

		protected OpinionatedSparkView(INameValueSerializer nameValueSerializer)
		{
			_nameValueSerializer = nameValueSerializer;
		}

		string IUrlHelper.GetVirtualPath(RouteValueDictionary routeValues)
		{
			VirtualPathData vpd = RouteTable.Routes.GetVirtualPath(ViewContext.RequestContext, routeValues);

			if (vpd != null)
			{
				return vpd.VirtualPath;
			}

			return null;
		}

		public virtual string ApplicationPath
		{
			get { return ViewContext.RequestContext.HttpContext.Request.ApplicationPath; }
		}

		public INameValueSerializer NameValueSerializer
		{
			get { return _nameValueSerializer; }
		}

		public virtual void Render(string html)
		{
			ViewContext.HttpContext.Response.Write(html);
		}
	}

	public abstract class OpinionatedSparkView<TViewModel> : OpinionatedSparkView, IOpinionatedView<TViewModel>
		where TViewModel : class
	{
		private readonly List<IBehaviorMarker> _behaviors = new List<IBehaviorMarker>();
		private OpinionatedHtmlHelper<TViewModel> _htmlHelper;

		private ViewDataDictionary<TViewModel> _viewData;

		protected OpinionatedSparkView()
		{
			_behaviors.Add(new ValidationBehavior(() => ViewModelState));
			_behaviors.Add(new LabelMemberBehavior());
			_behaviors.Add(new InputTypeBehavior());
		}

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

		// TODO: Model vs ViewModel (and MVC 3 dynamic property)
		public TViewModel Model
		{
			get { return ViewData.Model; }
		}

		public new OpinionatedHtmlHelper<TViewModel> Html
		{
			get { return _htmlHelper ?? (_htmlHelper = new OpinionatedHtmlHelper<TViewModel>(ViewContext, this, NameValueSerializer)); }
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

		protected override void SetViewData(ViewDataDictionary viewData)
		{
			_viewData = new ViewDataDictionary<TViewModel>(viewData);
			base.SetViewData(_viewData);
		}
	}
}