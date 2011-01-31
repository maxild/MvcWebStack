using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using Maxfire.Core;
using Maxfire.Web.Mvc.Html5.Elements;
using Maxfire.Web.Mvc.Html5.HtmlTokens;

namespace Maxfire.Web.Mvc.Html5
{
	public static class OpinionatedHtmlHelperExtensions
	{
		public static Anchor ActionLink<TController>(this IUrlHelper urlHelper,
			Expression<Action<TController>> action, string linkText,
			IEnumerable<KeyValuePair<string, object>> attributes) where TController : Controller
		{
			string url = urlHelper.UrlFor(action);
			return new Anchor().InnerHtml(linkText).Attr(attributes).Attr(HtmlAttribute.HRef, url);
		}

		public static Input InputFor<TModel, TValue>(this IModelMetadataAccessor<TModel> accessor,
			string type, Expression<Func<TModel, TValue>> expression, 
			object explicitValue, IEnumerable<KeyValuePair<string, object>> attributes)
		{
			string modelName = accessor.GetModelNameFor(expression);
			string fullHtmlFieldName = accessor.GetFullHtmlFieldName(modelName);
			object value = explicitValue ?? accessor.GetAttemptedModelValue(modelName);
			return new Input(type, fullHtmlFieldName, accessor).Value(value).Attr(attributes);
		}

		public static RadioButtonList RadioButtonListFor<TModel, TProperty>(this IModelMetadataAccessor<TModel> accessor,
			Expression<Func<TModel, TProperty>> expression, IEnumerable<TextValuePair> options, object explicitValue,
			IEnumerable<KeyValuePair<string, object>> radioAttributes,
			IEnumerable<KeyValuePair<string, object>> labelAttributes)
		{
			string modelName = accessor.GetModelNameFor(expression);
			string fullHtmlFieldName = accessor.GetFullHtmlFieldName(modelName);
			object value = explicitValue ?? accessor.GetAttemptedModelValue(modelName);
			var textValuePairs =  options ?? accessor.GetOptions(modelName);
			// TODO: Each radio control should have an auto label with for pointing to id (see below)
			// TODO: Each radio control should have (sanitized) unique id
			// TODO: labelAttributes not used at all
			return new RadioButtonList(fullHtmlFieldName, accessor)
				.SelectedValue(value)
				.Options.FromTextValuePairs(textValuePairs)
				.Attr(radioAttributes);
		}
	}
}