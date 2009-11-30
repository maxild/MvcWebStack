using System;
using System.Linq;
using System.Web.Mvc;

namespace Maxfire.Web.Mvc
{
	public static class ActionFilterExtensions
	{
		public static ModelStateDictionary GetModelState(this ActionExecutingContext filterContext)
		{
			return filterContext.Controller.ViewData.ModelState;
		}

		public static string GetBindingPrefixOfParameterWithType(this ActionExecutingContext filterContext, Type parameterType)
		{
			ParameterDescriptor parameterDescriptor = filterContext.ActionDescriptor.GetParameters()
				.FirstOrDefault(x => x.ParameterType == parameterType);

			return parameterDescriptor != null ? parameterDescriptor.BindingInfo.Prefix : String.Empty;
		}

		public static string GetBindingPrefixOfParameterWithType<TParameter>(this ActionExecutingContext filterContext)
		{
			return filterContext.GetBindingPrefixOfParameterWithType(typeof (TParameter));
		}

		public static object GetValueOfParameterWithType(this ActionExecutingContext filterContext, Type parameterType)
		{
			object parameterValue = filterContext.ActionParameters
				.Select(kvp => kvp.Value)
				.FirstOrDefault(value => value != null && value.GetType() == parameterType);

			if (parameterValue == null)
			{
				throw new NullReferenceException("The action parameter was null. Check the binding prefix.");
			}

			return parameterValue;
		}

		public static TParameter GetValueOfParameterWithType<TParameter>(this ActionExecutingContext filterContext)
		{
			return (TParameter) filterContext.GetValueOfParameterWithType(typeof (TParameter));
		}
	}
}