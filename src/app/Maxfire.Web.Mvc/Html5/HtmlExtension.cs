using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Web.Mvc;
using Maxfire.Web.Mvc.Html5.Elements;

namespace Maxfire.Web.Mvc.Html5
{
	public static class HtmlExtension
	{
		public static Input InputFor<TModel, TValue>(this IModelNameValueAccessor<TModel> accessor,
			string type, Expression<Func<TModel, TValue>> expression, object value = null) where TModel : class
		{
			string name = accessor.GetModelNameFor(expression);
			string val = accessor.GetValue(value, name);

			return new Input(type, name, accessor.GetModelMetadata(name))
				.Value(val);
		}

		private static string GetValue<TModel>(this IModelNameValueAccessor<TModel> accessor, object explicitValue, string name)
		{
			string valueParameter = null;
			if (explicitValue != null)
			{
				valueParameter = Convert.ToString(explicitValue, CultureInfo.CurrentCulture);
			}

			// Determine value by the frameworks convention
			return accessor.GetModelStateAttemptedValue<string>(name) ?? // Attempted value recorded in ModelState["name"].Value.AttemptedValue
				   valueParameter ??                                     // Explicitly provided value in the call to the view helper in the page
			       accessor.GetAttemptedModelValue(name);                // Value from model (we do not use ViewData.Eval("name") here)
		}

		private static TDestinationType GetModelStateAttemptedValue<TDestinationType>(this IModelStateContainer container, string key)
		{
			return (TDestinationType) container.GetModelStateAttemptedValue(key, typeof (TDestinationType));
		}

		private static object GetModelStateAttemptedValue(this IModelStateContainer container, string key, Type destinationType)
		{
			ModelState modelState;
			if (container.ModelState.TryGetValue(key, out modelState))
			{
				if (modelState.Value != null)
				{
					return modelState.Value.ConvertTo(destinationType, culture: null);
				}
			}
			return null;
		}
	}
}