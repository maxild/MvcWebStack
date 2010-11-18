using System;
using System.Collections.Generic;
using System.Web.Routing;

namespace Maxfire.Web.Mvc
{
	public class UrlBuilder
	{
		private readonly IUrlHelper _urlHelper;
		private readonly RouteValueDictionary _staticReflectionValues;
		private readonly RouteValueDictionary _routeValues;
		private readonly object _controllerName;
		private readonly object _actionName;

		public UrlBuilder(IUrlHelper urlHelper, RouteValueDictionary staticReflectionValues)
		{
			if (urlHelper == null) 
				throw new ArgumentNullException("urlHelper");
			if (staticReflectionValues == null)
				throw new ArgumentNullException("staticReflectionValues");
			if (!staticReflectionValues.ContainsKey("Controller"))
				throw new ArgumentException("The required values have to contain the special 'controller' key.", "staticReflectionValues");
			if (!staticReflectionValues.ContainsKey("Action"))
				throw new ArgumentException("The required values have to contain the special 'action' key.", "staticReflectionValues");

			_controllerName = staticReflectionValues.GetRequiredString("Controller");
			_actionName = staticReflectionValues.GetRequiredString("Action");
			
			_urlHelper = urlHelper;
			_staticReflectionValues = staticReflectionValues;
			_routeValues = new RouteValueDictionary();
		}

		public UrlBuilder Id(object id)
		{
			return RouteValue("id", id);
		}

		public UrlBuilder RouteValue(string name, object value)
		{
			_routeValues[name] = value;
			return this;
		}

		public UrlBuilder RouteValues(object model)
		{
			RouteValueDictionary routeValues = new RouteValueDictionary();
			var values = _urlHelper.NameValueSerializer.GetValues(model, string.Empty);
			routeValues.Merge(values);
			return RouteValues(routeValues);
		}

		public UrlBuilder RouteValues(IDictionary<string, object> routeValues)
		{
			_routeValues.Merge(routeValues);
			return this;
		}

		public override string ToString()
		{
			_staticReflectionValues.Merge(_routeValues);
			_staticReflectionValues["Controller"] = _controllerName;
			_staticReflectionValues["Action"] = _actionName;
			return _urlHelper.GetVirtualPath(_staticReflectionValues);
		}

		public static implicit operator string(UrlBuilder urlBuilder)
		{
			return urlBuilder.ToString();
		}
	}
}