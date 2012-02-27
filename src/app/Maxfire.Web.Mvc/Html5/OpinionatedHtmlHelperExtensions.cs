using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
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
			string name = accessor.GetModelNameFor(expression);
			object value = explicitValue ?? accessor.GetAttemptedModelValue(name);
			return new Input(type, name, accessor).Value(value).Attr(attributes);
		}

		// TODO: Create FragmentList<T> such that a list of fragments can be treated as one fragment, but rendered in sequence
		public static MvcHtmlString MultipleInputFor<TModel, TValue>(this IModelMetadataAccessor<TModel> accessor,
			string type, Expression<Func<TModel, TValue>> expression, IEnumerable<KeyValuePair<string, object>> attributes)
		{
			string name = accessor.GetModelNameFor(expression);
			IEnumerable<KeyValuePair<string, object>> values = accessor.GetAttemptedModelValues(name);
			var htmlString = values.Aggregate(new StringBuilder(), (sb, kvp) => 
				sb.Append(new Input(type, kvp.Key, accessor).Value(kvp.Value).Attr(attributes))).ToString();
			return MvcHtmlString.Create(htmlString);
		}

		public static RadioButtonList RadioButtonListFor<TModel, TProperty>(this IModelMetadataAccessor<TModel> accessor,
			Expression<Func<TModel, TProperty>> expression, 
			IEnumerable<TextValuePair> options, 
			object explicitValue,
			IEnumerable<KeyValuePair<string, object>> radioAttributes,
			IEnumerable<KeyValuePair<string, object>> labelAttributes)
		{
			string name = accessor.GetModelNameFor(expression);
			object value = explicitValue ?? accessor.GetAttemptedModelValue(name);
			var textValuePairs =  options ?? accessor.GetOptions(name);
			// TODO: Each radio control should have an auto label with for pointing to id (see below)
			// TODO: Each radio control should have (sanitized) unique id
			// TODO: labelAttributes not used at all
			return new RadioButtonList(name, accessor)
				.SelectedValue(value)
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
			object value = explicitValue ?? accessor.GetAttemptedModelValue(name);
			var textValuePairs = options ?? accessor.GetOptions(name);

			return new Select(name, accessor)
				.SelectedValue(value)
				.Options.FromTextValuePairs(textValuePairs)
				.Attr(attributes);
		}
	}
}