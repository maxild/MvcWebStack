using System.Web;
using System.Web.Mvc;
using Maxfire.Core.Extensions;
using Newtonsoft.Json;

namespace Maxfire.NewtonSoftJson.Web.Mvc
{
	public class JsonNetResult : JsonResult
	{
		public JsonSerializerSettings SerializerSettings { get; set; }
		
		public Formatting Formatting { get; set; }

		public JsonNetResult()
		{
			SerializerSettings = new JsonSerializerSettings();
			Formatting = Formatting.None;
		}

		public override void ExecuteResult(ControllerContext context)
		{
			context.ThrowIfNull("context");

			HttpResponseBase response = context.HttpContext.Response;

			response.ContentType = ContentType.IsNotEmpty() ? ContentType : "application/json";

			if (ContentEncoding != null)
			{
				response.ContentEncoding = ContentEncoding;
			}

			if (Data != null)
			{
				JsonTextWriter writer = new JsonTextWriter(response.Output) { Formatting = Formatting };
				JsonSerializer serializer = JsonSerializer.Create(SerializerSettings);
				serializer.Serialize(writer, Data);
				writer.Flush();
			}
		}
	}
}