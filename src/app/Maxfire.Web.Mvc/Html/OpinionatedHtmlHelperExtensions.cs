using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using Maxfire.Core.Extensions;
using Maxfire.Web.Mvc.Html.Extensions;

namespace Maxfire.Web.Mvc.Html
{
	// TODOs:
	// 1) model values should be fetched first from modelstate and then by model/lambda if not present in modelstate
	// 2) behaviour should define CSS class for input errors
	public static class OpinionatedHtmlHelperExtensions
	{
		public static MvcHtmlString ActionLink<TController>(this HtmlHelper helper,
			Expression<Action<TController>> action, string linkText, IDictionary<string, object> htmlAttributes) where TController : Controller
		{
			var controller = helper.ViewContext.Controller as OpinionatedController;
			IQueryStringSerializer queryStringSerializer = null;
			if (controller != null)
			{
				queryStringSerializer = controller.QueryStringSerializer;
			}
			var routeValues = RouteValuesHelper.GetRouteValuesFromExpression(action, queryStringSerializer);
			return helper.RouteLink(linkText, routeValues, htmlAttributes);
		}

		public static string DisplayNameFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression)
			where TModel : class
		{
			var modelMetadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
			return modelMetadata.DisplayName ?? modelMetadata.PropertyName ?? expression.GetHtmlFieldNameFor().Split('.').Last();
		}

		public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes) where TModel : class
		{
			string labelText = htmlHelper.DisplayNameFor(expression);
			return htmlHelper.LabelFor(expression, labelText, htmlAttributes);
		}

		public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TValue>> expression, string labelText, IDictionary<string, object> htmlAttributes) where TModel : class
		{
			return String.IsNullOrEmpty(labelText) ?
				MvcHtmlString.Empty :
				MvcHtmlString.Create(LabelHelper(expression.GetHtmlFieldIdFor(htmlHelper), labelText, htmlAttributes));
		}

		public static MvcHtmlString LegendFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes) where TModel : class
		{
			string labelText = htmlHelper.DisplayNameFor(expression);
			
			return String.IsNullOrEmpty(labelText) ? 
				MvcHtmlString.Empty : 
				MvcHtmlString.Create(LegendHelper(labelText, htmlAttributes));
		}

		public static MvcHtmlString DateBoxFor<TModel>(this HtmlHelper<TModel> htmlHelper,
				Expression<Func<TModel, DateTime>> expression, IDictionary<string, object> htmlAttributes) where TModel : class
		{
			return htmlHelper.TextBoxHelper(expression, date => date.ToShortDateString(), htmlAttributes);
		}

		public static MvcHtmlString DateBoxFor<TModel>(this HtmlHelper<TModel> htmlHelper,
				Expression<Func<TModel, DateTime?>> expression, IDictionary<string, object> htmlAttributes) where TModel : class
		{
			return htmlHelper.TextBoxHelper(expression, date => date.HasValue ? date.Value.ToShortDateString() : string.Empty, htmlAttributes);
		}

		public static MvcHtmlString DateSelectsFor<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, DateTime>> expression, 
			string labelText,
			IDictionary<string, object> htmlSelectElementAttributes, IDictionary<string, object> htmlLabelElementAttributes
			) where TModel : class
		{
			var modelValue = (DateTime?)ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData).Model;
			string fullHtmlFieldName = expression.GetHtmlFieldNameFor(htmlHelper);

			var sb = new StringBuilder();
			
			string dayName = fullHtmlFieldName + ".Day";
			string dayId = Html401IdUtil.CreateSanitizedId(dayName);
			string dayText = labelText + " (dag)";
			object dayValue = htmlHelper.GetModelStateValue(dayName, typeof(int)); 
			if (dayValue == null && modelValue != null)
			{
				dayValue = modelValue.Value.Day;
			}
			var dayOptions = OptionsAdapter2.FromCollection(1.UpTo(31));

			sb.Append(LabelHelper(dayId, dayText, htmlLabelElementAttributes));
			sb.Append(htmlHelper.SelectHelper(dayName, dayId, dayOptions, null, dayValue, htmlSelectElementAttributes));

			string monthName = fullHtmlFieldName + ".Month";
			string monthId = Html401IdUtil.CreateSanitizedId(monthName);
			string monthText = labelText + " (måned)";
			object monthValue = htmlHelper.GetModelStateValue(monthName, typeof(int));
			if (monthValue == null && modelValue != null)
			{
				monthValue = modelValue.Value.Month;
			}
			var monthOptions = OptionsAdapter2.Months();
			
			sb.Append(LabelHelper(monthId, monthText, htmlLabelElementAttributes));
			sb.Append(htmlHelper.SelectHelper(monthName, monthId, monthOptions, null, monthValue, htmlSelectElementAttributes));


			string yearName = fullHtmlFieldName + ".Year";
			string yearId = Html401IdUtil.CreateSanitizedId(yearName);
			string yearText = labelText + " (år)";
			object yearValue = htmlHelper.GetModelStateValue(yearName, typeof(int));
			if (yearValue == null && modelValue != null)
			{
				yearValue = modelValue.Value.Year;
			}
			var yearOptions = OptionsAdapter2.FromCollection(1900.UpTo(2000));
			
			sb.Append(LabelHelper(yearId, yearText, htmlLabelElementAttributes));
			sb.Append(htmlHelper.SelectHelper(yearName, yearId, yearOptions, null, yearValue, htmlSelectElementAttributes));

			return MvcHtmlString.Create(sb.ToString());
		}

		// Note: Even though we use SelectListItem the Selected property isn't used at all, because the model is compared to the values to find any selected options
		// Todo: Maybe use my own ITextValuePair abstraction
		public static MvcHtmlString RadioButtonListFor<TModel, TProperty>(this OpinionatedHtmlHelper<TModel> htmlHelper, 
			Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> options,
			IDictionary<string, object> htmlInputElementAttributes, IDictionary<string, object> htmlLabelElementAttributes) where TModel : class
		{
			var sb = new StringBuilder();
			object modelValue = htmlHelper.GetModelValueFor(expression);
			string name = expression.GetHtmlFieldNameFor(htmlHelper);
			
			options = options ?? htmlHelper.GetOptionsFor(expression);

			if (options != null)
			{
				foreach (var option in options)
				{
					string sanitizedId = Html401IdUtil.CreateSanitizedId(name + "_" + option.Value);
					sb.Append(htmlHelper.RadioButtonHelper(modelValue, sanitizedId, name, option.Value, htmlInputElementAttributes));
					sb.Append(LabelHelper(sanitizedId, option.Text, htmlLabelElementAttributes));
				}
			}

			return MvcHtmlString.Create(sb.ToString());
		}

		// Note: MVC 2 RTM source code for DropDownListFor does have a BUG: 
		//   Any SelectListItem in the list of options that has the Selected property set to true 
		//   does not have any bearing over the <option> element rendered with the selected attribute 
		//   applied for the item. 
		public static MvcHtmlString SelectFor<TModel, TProperty>(this OpinionatedHtmlHelper<TModel> htmlHelper, 
			Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> options, 
			string optionLabel, IDictionary<string, object> htmlAttributes) where TModel : class
		{
			object modelValue = htmlHelper.GetModelValueFor(expression);
			string name = expression.GetHtmlFieldNameFor(htmlHelper);
			string sanitizedId = Html401IdUtil.CreateSanitizedId(name);

			options = options ?? htmlHelper.GetOptionsFor(expression);

			return MvcHtmlString.Create(SelectHelper(htmlHelper, name, sanitizedId, options, optionLabel, modelValue, htmlAttributes));
		}

		private static string SelectHelper<TModel>(this HtmlHelper<TModel> htmlHelper, 
			string name, string id, IEnumerable<SelectListItem> options, 
			string optionLabel, object modelValue, IDictionary<string, object> htmlAttributes) where TModel : class
		{
			var sb = new StringBuilder();
			
			if (optionLabel != null)
			{
				sb.AppendLine(OptionHelper(new SelectListItem { Text = optionLabel, Value = String.Empty, Selected = false }));
			}

			if (options != null)
			{
				foreach (var option in options)
				{
					sb.AppendLine(OptionHelper(option, modelValue));
				}
			}

			var tagBuilder = new TagBuilder("select")
			                 	{
			                 		InnerHtml = sb.ToString()
			                 	};
			tagBuilder.MergeAttributes(htmlAttributes);
			tagBuilder.MergeAttribute("name", name, true);
			tagBuilder.MergeAttribute("id", id, true);
			
			ModelState modelState;
			if (htmlHelper.ViewData.ModelState.TryGetValue(name, out modelState))
			{
				if (modelState.Errors.Count > 0)
				{
					tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
				}
			}

			return tagBuilder.ToString(TagRenderMode.Normal);
		}

		private static MvcHtmlString TextBoxHelper<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
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
			var tag = new TagBuilder("label") { InnerHtml = HttpUtility.HtmlEncode(labelText) };
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

		private static string OptionHelper(SelectListItem item, object modelValue = null)
		{
			var tagBuilder = new TagBuilder("option") { InnerHtml = HttpUtility.HtmlEncode(item.Text) };
			if (item.Value != null) tagBuilder.Attributes["value"] = item.Value;
			if (GetIsSelected(item.Value, modelValue)) tagBuilder.Attributes["selected"] = "selected";
			return tagBuilder.ToString(TagRenderMode.Normal);
		}

		private static MvcHtmlString RadioButtonHelper(this HtmlHelper htmlHelper,
			object modelValue, string id, string name, string value, IDictionary<string, object> htmlAttributes)
		{
			bool isChecked = GetIsSelected(value, modelValue);
			return htmlHelper.RadioButton(name, value, isChecked, htmlAttributes.GetIdExtendedHtmlAttributes(id));
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

		private static object GetModelValueFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression) where TModel : class
		{
			string name = expression.GetHtmlFieldNameFor(htmlHelper);
			object modelValue = htmlHelper.GetModelStateValue(name, typeof(TProperty));
			if (modelValue == null)
			{
				modelValue = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData).Model;
			}
			return modelValue;
		}

		private static object GetModelStateValue(this HtmlHelper htmlHelper, string key, Type destinationType)
		{
			ModelState modelState;
			if (htmlHelper.ViewData.ModelState.TryGetValue(key, out modelState))
			{
				if (modelState.Value != null)
				{
					return modelState.Value.ConvertTo(destinationType, null /* culture */);
				}
			}
			return null;
		}

		private static bool GetIsSelected(string value, object modelValue)
		{
			return modelValue != null && string.Equals(modelValue.ToString(), value, StringComparison.OrdinalIgnoreCase);
		}
	}
}