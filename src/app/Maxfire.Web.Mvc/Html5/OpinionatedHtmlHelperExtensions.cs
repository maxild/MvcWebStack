using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Maxfire.Core;
using Maxfire.Web.Mvc.Html5.Elements;

namespace Maxfire.Web.Mvc.Html5
{
	public static class OpinionatedHtmlHelperExtensions
	{
		public static Input InputFor<TModel, TValue>(this IModelMetadataAccessor<TModel> accessor,
			string type, Expression<Func<TModel, TValue>> expression, 
			object explicitValue, IEnumerable<KeyValuePair<string, object>> attributes)
		{
			string name = accessor.GetModelNameFor(expression);
			object value = explicitValue ?? accessor.GetAttemptedModelValue(name);
			return new Input(type, name, accessor).Value(value).Attr(attributes);
		}

		public static RadioButtonList RadioButtonListFor<TModel, TProperty>(this IModelMetadataAccessor<TModel> accessor,
			Expression<Func<TModel, TProperty>> expression, IEnumerable<TextValuePair> options, object explicitValue,
			IEnumerable<KeyValuePair<string, object>> radioAttributes,
			IEnumerable<KeyValuePair<string, object>> labelAttributes)
		{
			string name = accessor.GetModelNameFor(expression);
			object value = explicitValue ?? accessor.GetAttemptedModelValue(name);
			var textValuePairs =  options ?? accessor.GetOptions(name);
			// TODO: Each radio control should have an auto label with for pointing to id (see below)
			// TODO: Each radio control should have (sanitized) unique id
			return new RadioButtonList(name, accessor)
				.SelectedValue(value)
				.Options.FromTextValuePairs(textValuePairs)
				.Attr(radioAttributes);
		}
	}
}