using System;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Newtonsoft.Json;

namespace Maxfire.Web.Mvc
{
	public abstract class OpinionatedController : Controller, ITempDataContainer, IUrlHelper
	{
		private readonly IQueryStringSerializer _queryStringSerializer;

		protected OpinionatedController() : this(new DefaultQueryStringSerializer())
		{
		}

		protected OpinionatedController(IQueryStringSerializer queryStringSerializer)
		{
			_queryStringSerializer = queryStringSerializer;
		}

		public IQueryStringSerializer QueryStringSerializer
		{
			get { return _queryStringSerializer; }
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