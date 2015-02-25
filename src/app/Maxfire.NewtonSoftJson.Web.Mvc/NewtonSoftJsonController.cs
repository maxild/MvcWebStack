using System.Text;
using System.Web.Mvc;
using Maxfire.Web.Mvc;
using Newtonsoft.Json;

namespace Maxfire.NewtonSoftJson.Web.Mvc
{
	public class NewtonSoftJsonController : OpinionatedController
	{
		protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
		{
			return Json(data, contentType, contentEncoding, behavior, Formatting.None);
		}

		protected virtual JsonNetResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior, Formatting formatting)
		{
			return JsonUtil.JsonNetResult(data, contentType, contentEncoding, behavior, formatting);
		}

		public override ActionResult RedirectAjaxRequest(string url)
		{
			var data = JsonUtil.Message(new { redirectUrl = url });
			return JsonUtil.JsonNetResult(data);
		}
	}
}