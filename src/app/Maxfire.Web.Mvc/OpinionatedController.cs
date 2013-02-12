using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;
using Maxfire.Core;

namespace Maxfire.Web.Mvc
{
	public abstract class OpinionatedController : Controller, ITempDataContainer, IUrlHelper
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

		public new object TempData
		{
			get { throw new InvalidOperationException("By **convention** we don't use the tempdata dictionary."); }
		}

		TempDataDictionary ITempDataContainer.TempData
		{
			get { return base.TempData; }
		}

		// Note: Could be an extension method to avoid big base class problem
		protected void FlashNotice(string message, params object[] args)
		{
			(this as ITempDataContainer).TempData["flash-notice"] = string.Format(message, args);
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
			get { return Request != null ? Request.IsAjaxRequest() : false; }
		}

		public virtual ActionResult RedirectAjaxRequest(string url)
		{
			// Derived class can override. Null signals do nothing.
			return null;
		}
	}
}