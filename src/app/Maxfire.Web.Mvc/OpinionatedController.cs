using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
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

		public INameValueSerializer NameValueSerializer
		{
			get { return _nameValueSerializer; }
		}

		private ViewDataWrapper<IEnumerable<SelectListItem>> _optionsWrapper;
		private ViewDataWrapper<IEnumerable<SelectListItem>> OptionsWrapper
		{
			get { return _optionsWrapper ?? (_optionsWrapper = new ViewDataWrapper<IEnumerable<SelectListItem>>(ViewData)); }
		}

		protected void SetOptionsFor<TViewModel>(Expression<Func<TViewModel, object>> expression, IEnumerable<SelectListItem> options) 
			where TViewModel : class
		{
			OptionsWrapper.SetDataFor(expression, options);
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
			VirtualPathData vpd = RouteTable.Routes.GetVirtualPath(ControllerContext.RequestContext, routeValues);

			if (vpd != null)
			{
				return vpd.VirtualPath;
			}

			return null;
		}

		public string ApplicationPath
		{
			get { return ControllerContext.RequestContext.HttpContext.Request.ApplicationPath; }
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