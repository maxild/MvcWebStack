using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Maxfire.Core.Extensions;
using Maxfire.Web.Mvc.Html.Extensions;

namespace Maxfire.Web.Mvc.Html
{
	// ValueProviderResult:
	//
	//   RawValue = string[] or string/object value
	//   AttemptedValue = Value send by browser (user agent)
	//   Culture = InvariantCulture (query or routedata values) or CurrentCulture (form posted values)
	// Value Provider Conventions
	// i)   DictionaryValueProvider<TValue>: RawValue = kvp.Value, AttemptedValue = Convert.ToString(kvp.Value, culture)
	// ii)  FormCollection<string[]>: RawValue = NameValueCollection.GetValues(key), AttemptedValue = NameValueCollection.this[key]
	// iii) NameValueCollectionValueProvider<string[]>: See FormCollection
	// iv)  
	// OBS: Form and QueryString relies on NameValueCollectionValueProvider, NameValueCollection.this[key] delegates to GetAsOneString
	//      that creates comma-separated string (multi-select, multi checkbox...see HTML spec)
	// OBS: RouteDataValueProvider relies on DictionaryValueProvider
	// OBS: For
	//
	// Model Binder Conventions
	// a) Everytime a model binder class valueProvider.GetValue(key) to get a non-null ValueProviderResult
	//    that result should be saved in model state: ModelState.SetModelValue(key, valueProviderResult)
	//
	// Input Control Conventions
	// The convention is that input controls should populate their values using the following
	// algorithm/steps:
	// a) Previously attempted value recorded in ModelState["name"].Value.AttemptedValue
	// b) Explicitly provided value in the page/template (e.g. Html.TextBox("name", "some value"))
	// c) ViewData by calling ViewData.Eval("name"). This way ViewData["name"] will take precedence over
	//    ViewData.Model.name.
	// The reason for this convention is that the re-rendered view should preserve invalid user input. This
	// way a 'hello world' value for a System.Int32 model value will by itself reappear in the re-rendered
	// form (assuming the modelbinder calls SetModelValue appropriately)
	//
	//     string attemptedValue = (string)htmlHelper.GetModelStateValue(fullName, typeof(string));
	//     tagBuilder.MergeAttribute("value", attemptedValue ?? ((useViewData) ? htmlHelper.EvalString(fullName) : valueParameter), isExplicitValue);
	//
	// Requirements:
	// 1) model values should be fetched first from modelstate and then by model/lambda if not present in modelstate (see above)
	// 2) ApplyBehaviours (behaviour support) should define CSS class for input errors. ToString should have no side-effects
	// 3) HTML 5 webforms 2.0 support
	// 4) Must use metadata (especially TemplateInfo: FormattedModelValue, HtmlFieldPrefix)
	// 5) INameValueSerializer resolution through NameValueSerializerProvider architecture (ModelBinderProvider...IoC)
	// 6) Make base class for all input helpers
	// 7) Support for bindings.xml in spark...dictionaries
	// 8) jQuery like API for defining DOM of form/html helpers (especially class values)
	// 9) Id explicitly specified in template takes precedence over auto-generated one.
	// 10) Validation css classes should be created (cross cutting helper concern)
	// 11) Render an additional <input type="hidden".../> for checkboxes.
	// 12) tagBuilder.MergeAttributes(htmlHelper.GetUnobtrusiveValidationAttributes(name, metadata));
	
	// Performance notes:
	// 1a) In MVC core: CachedExpressionCompiler.Process(expression)(Model) <---- Don't know how much this optimization is buying us???
	// 1b) In Maxfire.FluentHtml: expression.Compile()(Model)
	// 
	// 2a) In MVC core: ExpressionHelper.GetExpressionText(expression) ---> name
	// 2b) In Maxfire.FluentHtml: expression.GetNameFor() ---> name

	// From System.Web.Mvc...
	//public enum InputType
	//{
	//    CheckBox,
	//    Hidden,
	//    Password,
	//    Radio,
	//    Text
	//}
	//public static string GetInputTypeString(InputType inputType) {
	//    switch (inputType) {
	//        case InputType.CheckBox:
	//            return "checkbox";
	//        case InputType.Hidden:
	//            return "hidden";
	//        case InputType.Password:
	//            return "password";
	//        case InputType.Radio:
	//            return "radio";
	//        case InputType.Text:
	//            return "text";
	//        default:
	//            return "text";
	//    }
	//}

	public static class HtmlInputType
	{
		public const string Text = "text";
		public const string Checkbox = "checkbox";
		public const string Hidden = "hidden";
		public const string Password = "password";
		public const string Submit = "submit";
		public const string File = "file";
		public const string Radio = "radio";
		public const string Button = "button";
		public const string Reset = "reset";
	}
	
	public static class OpinionatedHtmlHelperExtensions
	{
		public static MvcHtmlString InputFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper,
			string inputType, Expression<Action<TModel, TValue>> expression, string value)
		{
			// todo
			return null;
		}

		public static MvcHtmlString ActionLink<TController>(this HtmlHelper htmlHelper,
			Expression<Action<TController>> action, string linkText, IDictionary<string, object> htmlAttributes) where TController : Controller
		{
			var nameValueSerializer = htmlHelper.GetNameValueSerializer();
			var routeValues = RouteValuesHelper.GetRouteValuesFromExpression(action, nameValueSerializer);
			return htmlHelper.RouteLink(linkText, routeValues, htmlAttributes);
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
			string labelText, string optionLabel,
			IDictionary<string, object> htmlSelectElementAttributes, IDictionary<string, object> htmlLabelElementAttributes
			) where TModel : class
		{
			var modelValue = (DateTime?)ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData).Model;
			string fullHtmlFieldName = expression.GetHtmlFieldNameFor(htmlHelper);

			return DateSelectsHelper(htmlHelper, fullHtmlFieldName, labelText, optionLabel, modelValue, htmlLabelElementAttributes, htmlSelectElementAttributes);
		}

		public static MvcHtmlString DateSelectsFor<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, DateTime?>> expression, 
			string labelText, string optionLabel,
			IDictionary<string, object> htmlSelectElementAttributes, IDictionary<string, object> htmlLabelElementAttributes
			) where TModel : class
		{
			var modelValue = (DateTime?)ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData).Model;
			string fullHtmlFieldName = expression.GetHtmlFieldNameFor(htmlHelper);

			return DateSelectsHelper(htmlHelper, fullHtmlFieldName, labelText, optionLabel, modelValue, htmlLabelElementAttributes, htmlSelectElementAttributes);
		}

		private static MvcHtmlString DateSelectsHelper<TModel>(this HtmlHelper<TModel> htmlHelper, 
			string fullHtmlFieldName, string labelText, string optionLabel, DateTime? modelValue,
			IDictionary<string, object> htmlLabelElementAttributes, IDictionary<string, object> htmlSelectElementAttributes
			) where TModel : class
		{
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
			sb.Append(htmlHelper.SelectHelper(dayName, dayId, dayOptions, optionLabel, dayValue, htmlSelectElementAttributes));

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
			sb.Append(htmlHelper.SelectHelper(monthName, monthId, monthOptions, optionLabel, monthValue, htmlSelectElementAttributes));


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
			sb.Append(htmlHelper.SelectHelper(yearName, yearId, yearOptions, optionLabel, yearValue, htmlSelectElementAttributes));

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

			var nameValueSerializer = htmlHelper.GetNameValueSerializer();

			if (optionLabel != null)
			{
				sb.AppendLine(OptionHelper(new SelectListItem { Text = optionLabel, Value = String.Empty, Selected = false }, nameValueSerializer));
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

		private static string OptionHelper(SelectListItem item, INameValueSerializer nameValueSerializer, object modelValue = null)
		{
			var tagBuilder = new TagBuilder("option") { InnerHtml = HttpUtility.HtmlEncode(item.Text) };
			if (item.Value != null) tagBuilder.Attributes["value"] = item.Value;
			if (GetIsSelected(item.Value, nameValueSerializer, modelValue)) tagBuilder.Attributes["selected"] = "selected";
			return tagBuilder.ToString(TagRenderMode.Normal);
		}

		private static MvcHtmlString RadioButtonHelper(this HtmlHelper htmlHelper,
			object modelValue, string id, string name, string value, IDictionary<string, object> htmlAttributes)
		{
			var nameValueSerializer = htmlHelper.GetNameValueSerializer();
			bool isChecked = GetIsSelected(value, nameValueSerializer, modelValue);
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
			// BUG: Here INameValueSerializer should be used to convert model ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData).Model to string/object
			string name = expression.GetHtmlFieldNameFor(htmlHelper);
			object attemptedValue = htmlHelper.GetModelStateValue(name, typeof(string)) ??
			                        ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData).Model;
			return attemptedValue;
		}

		private static string GetModelAttemptedValue<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, 
			Expression<Func<TModel, TProperty>> expression, INameValueSerializer nameValueSerializer)
		{
			object model = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData).Model;
			return model.GetValueString(nameValueSerializer);
		}

		private static TDestinationType GetModelStateAttemptedValue<TDestinationType>(this HtmlHelper htmlHelper, string key)
		{
			return (TDestinationType) htmlHelper.GetModelStateValue(key, typeof(TDestinationType));
		}

		// Note: Don't use view data for passing models to views!!!
		private static TDestionationType GetViewDataAttemptedValue<TDestionationType>(this HtmlHelper htmlHelper, string key)
		{
			return (TDestionationType)Convert.ChangeType(htmlHelper.ViewData.Eval(key), typeof(TDestionationType), CultureInfo.CurrentCulture);
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

			public IEnumerable<string> GetKeys()
			{
				yield return string.Empty;
			}
		}

		private static bool GetIsSelected(string value, INameValueSerializer nameValueSerializer, object modelValue)
		{
			return modelValue != null && string.Equals(modelValue.GetValueString(nameValueSerializer), value, StringComparison.OrdinalIgnoreCase);
		}

		private static string GetValueString(this object value, INameValueSerializer nameValueSerializer)
		{
			if (nameValueSerializer != null)
			{
				value = nameValueSerializer.GetValues(value).Map(x => x.Value).FirstOrDefault();
			}
			return Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
		}
	}
}