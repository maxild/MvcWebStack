using System;
using System.Web;
using System.Web.Routing;
using Maxfire.Web.Mvc.FluentHtml.Html;

namespace Maxfire.Web.Mvc.FluentHtml.Elements
{
	public class Link : Element<Link>
	{
		private readonly UrlBuilder _urlBuilder;
		
		public Link(UrlBuilder urlBuilder) : base("a", null)
		{
			if (urlBuilder == null)
				throw new ArgumentNullException("urlBuilder");

			_urlBuilder = urlBuilder;
		}

		public Link Text(string text)
		{
			SetInnerHtml(String.IsNullOrEmpty(text) ? String.Empty : HttpUtility.HtmlEncode(text));
			return this;
		}

		public Link RouteId(object id)
		{
			_urlBuilder.Id(id);
			return this;
		}

		public Link RouteValue(string name, object value)
		{
			_urlBuilder.RouteValue(name, value);
			return this;
		}

		public Link RouteValues(object model)
		{
			_urlBuilder.RouteValues(model);
			return this;
		}

		public Link RouteValues(RouteValueDictionary values)
		{
			_urlBuilder.RouteValues(values);
			return this;
		}

		public override string ToString()
		{
			string url = _urlBuilder.ToString();
			SetAttr(HtmlAttribute.Href, url);
			return base.ToString();
		}
	}
}