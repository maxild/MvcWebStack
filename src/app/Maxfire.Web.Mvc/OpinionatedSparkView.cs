using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;
using Spark.Web.Mvc;
using Maxfire.Core.Reflection;
using Maxfire.Web.Mvc.FluentHtml.Behaviors;

namespace Maxfire.Web.Mvc
{
	public abstract class OpinionatedSparkView<TViewModel> : SparkView, IOpinionatedView<TViewModel>, ITempDataContainer
		where TViewModel : class
	{
		private readonly List<IBehaviorMarker> _behaviors = new List<IBehaviorMarker>();

		protected OpinionatedSparkView()
		{
			_behaviors.Add(new ValidationBehavior(() => ViewModelState));
			_behaviors.Add(new LabelMemberBehavior());
			_behaviors.Add(new InputTypeBehavior());
		}

		public IEnumerable<IBehaviorMarker> Behaviors
		{
			get { return _behaviors; }
		}

		public ModelStateDictionary ViewModelState
		{
			get { return ViewData.ModelState; }
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

		public virtual void Render(string html)
		{
			ViewContext.HttpContext.Response.Write(html);
		}

		private ViewDataDictionary<TViewModel> _viewData;

		public new ViewDataDictionary<TViewModel> ViewData
		{
			get
			{
				if (_viewData == null)
					SetViewData(new ViewDataDictionary<TViewModel>());
				return _viewData;
			}
			set { SetViewData(value); }
		}

		public TViewModel ViewModel
		{
			get { return ViewData.Model; }
		}

		protected override void SetViewData(ViewDataDictionary viewData)
		{
			_viewData = new ViewDataDictionary<TViewModel>(viewData);
			base.SetViewData(_viewData);
		}

		public string HtmlNamePrefix { get; set; }

		/// <summary>
		/// Get display text for a property of an embedded type in the view model (where typeof(T) != typeof(TViewModel))
		/// </summary>
		protected string LabelTextFor<T>(Expression<Func<T, object>> expression) where T : class
		{
			return expression.GetDisplayName();
		}
	}
}