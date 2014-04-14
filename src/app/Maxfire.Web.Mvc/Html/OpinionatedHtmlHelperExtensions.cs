using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Maxfire.Core;
using Maxfire.Core.Extensions;
using Maxfire.Web.Mvc.Html.Extensions;
using Maxfire.Web.Mvc.Html5;

namespace Maxfire.Web.Mvc.Html
{
	public static class OpinionatedHtmlHelperExtensions
	{
		public static string DisplayNameFor<TModel, TValue>(this OpinionatedHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression)
			where TModel : class
		{
			var modelMetadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
			return modelMetadata.DisplayName ?? modelMetadata.PropertyName ?? expression.GetHtmlFieldNameFor().Split('.').Last();
		}

		public static MvcHtmlString LabelFor<TModel, TValue>(this OpinionatedHtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes) where TModel : class
		{
			string labelText = htmlHelper.GetLabelTextFor(expression);
			return htmlHelper.LabelFor(expression, labelText, htmlAttributes);
		}

		public static MvcHtmlString LabelFor<TModel, TValue>(this OpinionatedHtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TValue>> expression, string labelText, IDictionary<string, object> htmlAttributes) where TModel : class
		{
			return String.IsNullOrEmpty(labelText) ?
				MvcHtmlString.Empty :
				MvcHtmlString.Create(LabelHelper(expression.GetHtmlFieldIdFor(htmlHelper), labelText, htmlAttributes));
		}

		public static MvcHtmlString LegendFor<TModel, TValue>(this OpinionatedHtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes) where TModel : class
		{
			string labelText = htmlHelper.DisplayNameFor(expression);
			
			return String.IsNullOrEmpty(labelText) ? 
				MvcHtmlString.Empty : 
				MvcHtmlString.Create(LegendHelper(labelText, htmlAttributes));
		}

		public static MvcHtmlString DateBoxFor<TModel>(this OpinionatedHtmlHelper<TModel> htmlHelper,
				Expression<Func<TModel, DateTime>> expression, IDictionary<string, object> htmlAttributes) where TModel : class
		{
			return htmlHelper.TextBoxHelper(expression, date => date.ToShortDateString(), htmlAttributes);
		}

		public static MvcHtmlString DateBoxFor<TModel>(this OpinionatedHtmlHelper<TModel> htmlHelper,
				Expression<Func<TModel, DateTime?>> expression, IDictionary<string, object> htmlAttributes) where TModel : class
		{
			return htmlHelper.TextBoxHelper(expression, date => date.HasValue ? date.Value.ToShortDateString() : string.Empty, htmlAttributes);
		}

		public static MvcHtmlString DateSelectsFor<TModel>(this OpinionatedHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, DateTime>> expression, 
			string labelText, string optionLabel,
			IDictionary<string, object> htmlSelectElementAttributes, IDictionary<string, object> htmlLabelElementAttributes
			) where TModel : class
		{
			var modelValue = (DateTime?)ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData).Model;
			string fullHtmlFieldName = expression.GetHtmlFieldNameFor(htmlHelper);
			labelText = labelText ?? htmlHelper.GetLabelTextFor(expression);

			return DateSelectsHelper(htmlHelper, fullHtmlFieldName, labelText, optionLabel, modelValue, htmlLabelElementAttributes, htmlSelectElementAttributes);
		}

		public static MvcHtmlString DateSelectsFor<TModel>(this OpinionatedHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, DateTime?>> expression,
			string labelText, string optionLabel,
			IDictionary<string, object> htmlSelectElementAttributes, IDictionary<string, object> htmlLabelElementAttributes
			) where TModel : class
		{
			var modelValue = (DateTime?)ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData).Model;
			string fullHtmlFieldName = expression.GetHtmlFieldNameFor(htmlHelper);
			labelText = labelText ?? htmlHelper.GetLabelTextFor(expression);

			return DateSelectsHelper(htmlHelper, fullHtmlFieldName, labelText, optionLabel, modelValue, htmlLabelElementAttributes, htmlSelectElementAttributes);
		}

		private static MvcHtmlString DateSelectsHelper<TModel>(this OpinionatedHtmlHelper<TModel> htmlHelper, 
			string fullHtmlFieldName, 
			string labelText, 
			string optionLabel, 
			DateTime? modelValue,
			IDictionary<string, object> htmlLabelElementAttributes, 
			IDictionary<string, object> htmlSelectElementAttributes
			) where TModel : class
		{
			var sb = new StringBuilder();

			bool exactMatchError = htmlHelper.ViewData.ModelState.IsInvalid(
				key => string.Equals(key, fullHtmlFieldName, StringComparison.OrdinalIgnoreCase));
			bool anyComponentErrors = htmlHelper.ViewData.ModelState.IsInvalid(
				key => key.StartsWith(fullHtmlFieldName + "."));
			bool modelIsInvalid = exactMatchError && !anyComponentErrors;

			string dayName = fullHtmlFieldName + ".Day";
			string dayId = Html401IdUtil.CreateSanitizedId(dayName);
			string dayText = labelText + " (dag)";
			object dayValue = htmlHelper.GetModelStateValue(dayName, typeof(int)); 
			if (dayValue == null && modelValue != null)
			{
				dayValue = modelValue.Value.Day;
			}
			var dayOptions = htmlHelper.GetOptions(dayName) ?? OptionsAdapter.FromCollection(1.UpTo(31));

			sb.Append(LabelHelper(dayId, dayText, htmlLabelElementAttributes));
// ReSharper disable PossibleMultipleEnumeration
			if (dayOptions.IsSingular())
			{
				TextValuePair singleOption = dayOptions.Single();
				sb.Append(htmlHelper.Hidden(dayName, singleOption.Value));
				sb.Append(singleOption.Text);
				sb.Append(".");
				sb.Append(@" &nbsp;");
			}
			else
			{
				sb.Append(htmlHelper.SelectHelper(dayName, dayId, dayOptions, optionLabel, dayValue, htmlSelectElementAttributes,
				                                  modelIsInvalid));
			}
// ReSharper restore PossibleMultipleEnumeration

			string monthName = fullHtmlFieldName + ".Month";
			string monthId = Html401IdUtil.CreateSanitizedId(monthName);
			string monthText = labelText + " (måned)";
			object monthValue = htmlHelper.GetModelStateValue(monthName, typeof(int));
			if (monthValue == null && modelValue != null)
			{
				monthValue = modelValue.Value.Month;
			}
			var monthOptions = htmlHelper.GetOptions(monthName) ?? OptionsAdapter.Months();
			
			sb.Append(LabelHelper(monthId, monthText, htmlLabelElementAttributes));
			sb.Append(htmlHelper.SelectHelper(monthName, monthId, monthOptions, optionLabel, monthValue, htmlSelectElementAttributes, modelIsInvalid));

			string yearName = fullHtmlFieldName + ".Year";
			string yearId = Html401IdUtil.CreateSanitizedId(yearName);
			string yearText = labelText + " (år)";
			object yearValue = htmlHelper.GetModelStateValue(yearName, typeof(int));
			if (yearValue == null && modelValue != null)
			{
				yearValue = modelValue.Value.Year;
			}
			var yearOptions = htmlHelper.GetOptions(yearName) ?? OptionsAdapter.FromCollection(1950.UpTo(2050));
			
			sb.Append(LabelHelper(yearId, yearText, htmlLabelElementAttributes));
			sb.Append(htmlHelper.SelectHelper(yearName, yearId, yearOptions, optionLabel, yearValue, htmlSelectElementAttributes, modelIsInvalid));

			return MvcHtmlString.Create(sb.ToString());
		}

		// Note: MVC 2 RTM source code for DropDownListFor does have a BUG: 
		//   Any SelectListItem in the list of options that has the Selected property set to true 
		//   does not have any bearing over the <option> element rendered with the selected attribute 
		//   applied for the item. 
		public static MvcHtmlString DropDownListFor<TModel, TProperty>(this OpinionatedHtmlHelper<TModel> htmlHelper, 
			Expression<Func<TModel, TProperty>> expression, IEnumerable<TextValuePair> options, 
			string optionLabel, IDictionary<string, object> htmlAttributes) where TModel : class
		{
			object modelValue = htmlHelper.GetModelValueFor(expression);
			string name = expression.GetHtmlFieldNameFor(htmlHelper);
			string sanitizedId = Html401IdUtil.CreateSanitizedId(name);

			options = options ?? htmlHelper.GetOptionsFor(expression);

			return MvcHtmlString.Create(SelectHelper(htmlHelper, name, sanitizedId, options, optionLabel, modelValue, htmlAttributes));
		}

		private static string SelectHelper<TModel>(this OpinionatedHtmlHelper<TModel> htmlHelper, 
			string name, 
			string id, 
			IEnumerable<TextValuePair> options, 
			string optionLabel, 
			object modelValue, 
			IDictionary<string, object> htmlAttributes, 
			bool modelIsInvalid = false) where TModel : class
		{
			var sb = new StringBuilder();

			var nameValueSerializer = htmlHelper.GetNameValueSerializer();

			if (optionLabel != null)
			{
				sb.AppendLine(OptionHelper(new TextValuePair(optionLabel, String.Empty), nameValueSerializer));
			}

			if (options != null)
			{
				foreach (var option in options)
				{
					sb.AppendLine(OptionHelper(option, nameValueSerializer, modelValue));
				}
			}

			var tagBuilder = new TagBuilder("select")
			                 	{
			                 		InnerHtml = sb.ToString()
			                 	};
			tagBuilder.MergeAttributes(htmlAttributes);
			tagBuilder.MergeAttribute("name", name, true);
			tagBuilder.MergeAttribute("id", id, true);

			bool exactComponentError = htmlHelper.ViewData.ModelState.IsInvalid(key => string.Equals(key, name, StringComparison.OrdinalIgnoreCase));

			if (exactComponentError || modelIsInvalid)
			{
				tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
			}

			return tagBuilder.ToString(TagRenderMode.Normal);
		}

		private static INameValueSerializer GetNameValueSerializer(this HtmlHelper htmlHelper)
		{
			var controller = htmlHelper.ViewContext.Controller as OpinionatedController;
			INameValueSerializer nameValueSerializer = null;
			if (controller != null)
			{
				nameValueSerializer = controller.NameValueSerializer;
			}
			return nameValueSerializer;
		}

		private static MvcHtmlString TextBoxHelper<TModel, TProperty>(this OpinionatedHtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TProperty>> expression, Func<TProperty, string> valueSelector, IDictionary<string, object> htmlAttributes) where TModel : class
		{
			object modelValue = htmlHelper.GetModelValueFor(expression);
			string name = expression.GetHtmlFieldNameFor(htmlHelper);
			string sanitizedId = Html401IdUtil.CreateSanitizedId(name);
			var value = (TProperty)modelValue;
			return htmlHelper.TextBox(name, valueSelector(value), htmlAttributes.GetIdExtendedHtmlAttributes(sanitizedId));
		}

		private static string LabelHelper(string forId, string labelText, IDictionary<string, object> htmlAttributes)
		{
			var tag = new TagBuilder("label") { InnerHtml = labelText };
			tag.MergeAttributes(htmlAttributes);
			tag.Attributes.Add("for", forId);
			return tag.ToString(TagRenderMode.Normal);
		}

		private static string LegendHelper(string labelText, IDictionary<string, object> htmlAttributes)
		{
			var tag = new TagBuilder("legend") { InnerHtml = HttpUtility.HtmlEncode(labelText) };
			tag.MergeAttributes(htmlAttributes);
			return tag.ToString(TagRenderMode.Normal);
		}

		private static string OptionHelper(TextValuePair item, INameValueSerializer nameValueSerializer, object modelValue = null)
		{
			var tagBuilder = new TagBuilder("option") { InnerHtml = HttpUtility.HtmlEncode(item.Text) };
			if (item.Value != null) tagBuilder.Attributes["value"] = item.Value;
			if (GetIsSelected(item.Value, nameValueSerializer, modelValue)) tagBuilder.Attributes["selected"] = "selected";
			return tagBuilder.ToString(TagRenderMode.Normal);
		}

		private static IDictionary<string, object> GetIdExtendedHtmlAttributes(this IDictionary<string, object> htmlAttributes, string id)
		{
			if (!String.IsNullOrEmpty(id))
			{
				// We do not want to mutate htmlAttributes (no side-effects)
				htmlAttributes = htmlAttributes.Concat(new Dictionary<string, object> { { "id", id } })
					.ToDictionary(x => x.Key, x => x.Value);
			}
			return htmlAttributes;
		}

		public static object GetModelValueFor<TModel, TProperty>(this OpinionatedHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression) where TModel : class
		{
			var self = htmlHelper as IModelNameResolver<TModel>;
			string name = self.GetModelNameFor(expression);
			object attemptedvalue = htmlHelper.GetModelStateValue(name, typeof (string));
			return attemptedvalue;
		}

		private static object GetModelStateValue(this HtmlHelper htmlHelper, string key, Type destinationType)
		{
			ModelState modelState;
			if (htmlHelper.ViewData.ModelState.TryGetValue(key, out modelState))
			{
				if (modelState.Value != null)
				{
					// BUG: This should be the _attempted_ value, not the _model_ (bound) value
					// TODO: Problem:
					// Q: Hvorfor skal TModel blandes ind i attemptedValue
					// A: Fordi model-based value initilaization uses INameValueDeserializer (KommuneModelBinder)
					IModelBinder binder = ModelBinders.Binders.GetNonDefaultBinder(destinationType);
					if (binder != null)
					{
						var bindingContext = new ModelBindingContext
						{
							ValueProvider = new ValueProvider(modelState.Value),
							ModelState = htmlHelper.ViewData.ModelState,
							ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, destinationType),
						};
						return binder.BindModel(htmlHelper.ViewContext, bindingContext);
					}
					
					// If dedicated binder hasn't been registered fallback to simple type conversion
					return modelState.Value.ConvertTo(destinationType, null /* culture */);
				}
			}
			return null;
		}

		class ValueProvider : IValueProvider
		{
			private readonly ValueProviderResult _valueProviderResult;

			public ValueProvider(ValueProviderResult valueProviderResult)
			{
				_valueProviderResult = valueProviderResult;
			}

			public bool ContainsPrefix(string prefix)
			{
				if (prefix == null) throw new ArgumentNullException("prefix");
				return prefix.Length == 0;
			}

			public ValueProviderResult GetValue(string key)
			{
				if (key == null) throw new ArgumentNullException("key");
				return key.Length == 0 ? _valueProviderResult : null;
			}
		}

		private static bool GetIsSelected(string value, INameValueSerializer nameValueSerializer, object modelValue)
		{
			return modelValue != null && string.Equals(modelValue.GetValueString(nameValueSerializer), value, StringComparison.OrdinalIgnoreCase);
		}

		private static string GetValueString(this object value, INameValueSerializer nameValueSerializer)
		{
			if (value == null)
			{
				return null;
			}
			if (nameValueSerializer != null)
			{
				value = nameValueSerializer.GetValues(value).Map(x => x.Value).FirstOrDefault();
			}
			return Convert.ToString(value, CultureInfo.InvariantCulture);
		}
	}
}