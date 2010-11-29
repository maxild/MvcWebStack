using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using Maxfire.Core.Extensions;
using Maxfire.Web.Mvc.Html5.Elements;

namespace Maxfire.Web.Mvc.Html5
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
	// OBS: Using ModelMetadata.FromLambdaExpression(expression, ViewData).Model is the public surface to the CachedExpressionCompiler to do fast compiles of expression trees
	// Note: I have made the CachedExpressionCompiler public!!!!
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

	public static class OpinionatedHtmlHelperExtensions
	{
		public static Input InputFor<TModel, TValue>(this IModelMetadataAccessor<TModel> accessor,
			string type, Expression<Func<TModel, TValue>> expression, 
			object explicitValue, IDictionary<string, object> attributes)
		{
			string name = accessor.GetModelNameFor(expression);
			string value = explicitValue.ToNullString(CultureInfo.CurrentCulture) ?? accessor.GetAttemptedModelValue(name);
			return new Input(type, name, accessor).Value(value).Attr(attributes);
		}
	}
}