using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;
using Maxfire.Core;
using Maxfire.Web.Mvc.Html5.Elements;
using Maxfire.Web.Mvc.Html5.HtmlTokens;

namespace Maxfire.Web.Mvc.Html5
{
	public static class OpinionatedHtmlHelperExtensions
	{
		public static Anchor ActionLink<TController>(this IUrlHelper urlHelper,
			Expression<Action<TController>> action,
			string linkText,
			IEnumerable<KeyValuePair<string, object>> attributes) where TController : Controller
		{
			string url = urlHelper.UrlFor(action);
			return ActionLinkHelper(url, linkText, attributes);
		}

		public static Anchor ActionLink<TController>(this IUrlHelper urlHelper,
			Expression<Action<TController>> action,
			string linkText,
			RouteValueDictionary routeValues,
			IEnumerable<KeyValuePair<string, object>> attributes) where TController : Controller
		{
			string url = urlHelper.UrlFor(action, routeValues);
			return ActionLinkHelper(url, linkText, attributes);
		}

		private static Anchor ActionLinkHelper(string url, string linkText, IEnumerable<KeyValuePair<string, object>> attributes)
		{
			return new Anchor().InnerHtml(linkText).Attr(attributes).Attr(HtmlAttribute.HRef, url);
		}

		public static Input InputFor<TModel, TValue>(this IModelMetadataAccessor<TModel> accessor,
			string type, Expression<Func<TModel, TValue>> expression, 
			object explicitValue, IEnumerable<KeyValuePair<string, object>> attributes)
		{
			string name = accessor.GetModelNameFor(expression);
			string value = accessor.GetModelValueAsString(name);
			return new Input(type, name, accessor)
				.BindValueAndExplicitValue(value, explicitValue)
				.Attr(attributes);
		}

		public static RadioButtonList RadioButtonListFor<TModel, TProperty>(this IModelMetadataAccessor<TModel> accessor,
			Expression<Func<TModel, TProperty>> expression, 
			IEnumerable<TextValuePair> options, 
			object explicitValue,
			IEnumerable<KeyValuePair<string, object>> radioAttributes,
			IEnumerable<KeyValuePair<string, object>> labelAttributes)
		{
			string name = accessor.GetModelNameFor(expression);
			string value = accessor.GetModelValueAsString(name);
			var textValuePairs = options ?? accessor.GetOptions(name);
			// TODO: Each radio control should have an auto label with for pointing to id (see below)
			// TODO: Each radio control should have (sanitized) unique id
			// TODO: labelAttributes not used at all
			return new RadioButtonList(name, accessor)
				.BindValueAndExplicitValue(value, explicitValue)
				.Options.FromTextValuePairs(textValuePairs)
				.Attr(radioAttributes);
		}

		public static Select SelectFor<TModel, TProperty>(this IModelMetadataAccessor<TModel> accessor,
			Expression<Func<TModel, TProperty>> expression, 
			IEnumerable<TextValuePair> options,
			object explicitValue,
			IEnumerable<KeyValuePair<string, object>> attributes)
		{
			string name = accessor.GetModelNameFor(expression);
			string value = accessor.GetModelValueAsString(name);
			var textValuePairs = options ?? accessor.GetOptions(name);

			return new Select(name, accessor)
				.BindValueAndExplicitValue(value, explicitValue)
				.Options.FromTextValuePairs(textValuePairs)
				.Attr(attributes);
		}
	}
}