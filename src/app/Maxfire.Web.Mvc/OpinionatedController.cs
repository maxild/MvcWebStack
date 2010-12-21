using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Maxfire.Core;
using Newtonsoft.Json;

namespace Maxfire.Web.Mvc
{
	public abstract class OpinionatedController : Controller, ITempDataContainer, IUrlHelper
	{
		private readonly INameValueSerializer _nameValueSerializer;
		
		protected OpinionatedController() : this(new DefaultNameValueSerializer())
		{
		}

		protected OpinionatedController(INameValueSerializer nameValueSerializer)
		{
			_nameValueSerializer = nameValueSerializer;
		}

		private ViewDataWrapper<IEnumerable<TextValuePair>> _optionsWrapper;
		private ViewDataWrapper<IEnumerable<TextValuePair>> OptionsWrapper
		{
			get { return _optionsWrapper ?? (_optionsWrapper = new ViewDataWrapper<IEnumerable<TextValuePair>>(ViewData)); }
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
			get { return _labelTextWrapper ?? (_labelTextWrapper = new ViewDataWrapper<string>(ViewData)); }
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

		string IUrlHelper.GetVirtualPath(RouteValueDictionary routeValues)
		{
			return UrlHelperUtil.GetVirtualPath(RouteTable.Routes, ControllerContext.RequestContext, routeValues);
		}

		string IUrlHelper.ApplicationPath
		{
			get { return UrlHelperUtil.GetApplicationPath(ControllerContext.RequestContext); }
		}

		public INameValueSerializer NameValueSerializer
		{
			get { return _nameValueSerializer; }
		}

		protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
		{
			return Json(data, contentType, contentEncoding, behavior, Formatting.None);
		}

		protected virtual JsonNetResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior, Formatting formatting)
		{
			return JsonUtil.JsonNetResult(data, contentType, contentEncoding, behavior, formatting);
		}

		public bool IsAjaxRequest
		{
			get { return Request != null ? Request.IsAjaxRequest() : false; }
		}
	}
}