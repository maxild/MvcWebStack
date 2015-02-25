using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;
using Maxfire.Core;

namespace Maxfire.Web.Mvc
{
	public abstract class OpinionatedController : Controller, IUrlHelper
	{
		// Because we do not want to require all derived controllers to implement a non-default 
		// property we rely on property injection, and therefore NameValueSerializer has a setter
		private INameValueSerializer _nameValueSerializer;
		public INameValueSerializer NameValueSerializer
		{
			get { return _nameValueSerializer ?? (_nameValueSerializer = new DefaultNameValueSerializer()); }
			set { _nameValueSerializer = value; }
		}

		private ViewDataWrapper<IEnumerable<TextValuePair>> _optionsWrapper;
		private ViewDataWrapper<IEnumerable<TextValuePair>> OptionsWrapper
		{
			get { return _optionsWrapper ?? (_optionsWrapper = ViewDataWrapper.NewOptionsWrapper(ViewData)); }
		}

		protected void SetOptionsFor<TViewModel>(Expression<Func<TViewModel, object>> expression, IEnumerable<TextValuePair> options) 
			where TViewModel : class
		{
			OptionsWrapper.SetDataFor(expression, options);
		}

		protected void SetOptionsFor<TViewModel>(Expression<Func<TViewModel, object>> expression, string prefix, IEnumerable<TextValuePair> options)
			where TViewModel : class
		{
			OptionsWrapper.SetDataFor(expression, prefix, options);
		}

		private ViewDataWrapper<string> _labelTextWrapper;
		private ViewDataWrapper<string> LabelTextWrapper
		{
			get { return _labelTextWrapper ?? (_labelTextWrapper = ViewDataWrapper.NewLabelTextWrapper(ViewData)); }
		}

		protected void SetLabelTextFor<TViewModel>(Expression<Func<TViewModel, object>> expression, string labelText)
			where TViewModel : class
		{
			LabelTextWrapper.SetDataFor(expression, labelText);
		}

		public virtual string SiteRoot
		{
			get { return UrlHelperUtil.GetSiteRoot(ControllerContext.RequestContext); }
		}

		public virtual string SiteResource(string path)
		{
			return UrlHelper.GenerateContentUrl(path, ControllerContext.HttpContext);
		}

		public string GetVirtualPath(RouteValueDictionary routeValues)
		{
			return UrlHelperUtil.GetVirtualPath(RouteTable.Routes, ControllerContext.RequestContext, routeValues);
		}

		public bool IsAjaxRequest
		{
			get { return Request != null && Request.IsAjaxRequest(); }
		}

		public virtual ActionResult RedirectAjaxRequest(string url)
		{
			// Derived class can override. Null signals do nothing.
			return null;
		}

		private NamedValue<ModelStateDictionary> _modelStateToTempDateHelper;
		private NamedValue<ModelStateDictionary> ModelStateInTempData
		{
			get { return _modelStateToTempDateHelper ?? (_modelStateToTempDateHelper = ModelStateTempDataTransfer.GetNamedValue(() => TempData)); }
		}

		public void ExportModelStateToTempData()
		{
			// Only export model state once
			if (ModelStateInTempData.Value == null)
			{
				ModelStateInTempData.Value = ModelState;
			}
		}
	}
}