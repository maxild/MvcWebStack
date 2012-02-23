using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Maxfire.NewtonSoftJson.Web.Mvc
{
	public static class JsonUtil
	{
		private static IDictionary<string, string[]> _empty;
		public static IDictionary<string, string[]> EmptyErrors
		{
			get
			{
				if (_empty == null)
				{
					lock (typeof(JsonMessage<object>))
					{
						if (_empty == null)
						{
							_empty = new Dictionary<string, string[]>();
						}
					}
				}
				return _empty;
			}
		}

		public static JsonMessage<object> FailureMessage(IDictionary<string, string[]> validationErrors)
		{
			return new JsonMessage<object> { ValidationErrors = validationErrors };
		}

		public static JsonMessage<TMessage> Message<TMessage>(TMessage message) where TMessage : class
		{
			return new JsonMessage<TMessage> { Message = message, ValidationErrors = EmptyErrors };
		}

		public static JsonNetResult JsonNetResult(object data)
		{
			return JsonNetResult(data, null, null, JsonRequestBehavior.DenyGet, Formatting.None);
		}

		public static JsonNetResult JsonNetResult(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior, Formatting formatting)
		{
			return new JsonNetResult
			       	{
			       		Data = data,
			       		ContentType = contentType,
			       		ContentEncoding = contentEncoding,
						JsonRequestBehavior = behavior,
			       		Formatting = formatting,
			       		SerializerSettings = new JsonSerializerSettings { MappingResolver = new CamelCaseMappingResolver() }
			       	};
		}
	}

	/// <summary>
	/// Generic JSON result message.
	/// </summary>
	public class JsonMessage<TMessage> where TMessage : class
	{
		public IDictionary<string, string[]> ValidationErrors { get; set; }

		public bool Succeeded
		{
			get { return (ValidationErrors == null || ValidationErrors.Count == 0) && Message != null; }
		}

		public TMessage Message { get; set; }
	}
}