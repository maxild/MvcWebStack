using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using Maxfire.Core.Extensions;

namespace Maxfire.Web.Mvc
{
	public static class RouteValuesHelper
	{
		public static RouteValueDictionary GetRouteValuesFromExpression<TController>(Expression<Action<TController>> action, INameValueSerializer nameValueSerializer)
			where TController : Controller
		{
			return GetRouteValuesFromExpression(action, nameValueSerializer, null, string.Empty);
		}

		public static RouteValueDictionary GetRouteValuesFromExpression<TController>(Expression<Action<TController>> action, INameValueSerializer nameValueSerializer, string controllerName)
			where TController : Controller
		{
			return GetRouteValuesFromExpression(action, nameValueSerializer, controllerName, string.Empty);
		}

		public static RouteValueDictionary GetRouteValuesFromExpression<TController>(Expression<Action<TController>> action, INameValueSerializer nameValueSerializer, string controllerName, string prefix) 
			where TController : Controller
		{
			action.ThrowIfNull("action");

			var call = GetRequiredMethodCallExpression(action);

			string controllerNameToUse = GetControllerName(controllerName ?? typeof(TController).Name);
			string actionName = GetActionNameHelper(call);

			var routeValues = new RouteValueDictionary { { "controller", controllerNameToUse }, { "action", actionName } };

			if (nameValueSerializer != null)
			{
				AddParameterValuesFromExpressionToDictionary(routeValues, call, prefix, nameValueSerializer);
			}

			return routeValues;
		}

		public static string GetActionName<TController>(Expression<Action<TController>> action)
			where TController : Controller
		{
			var call = GetRequiredMethodCallExpression(action);
			return GetActionNameHelper(call);
		}

		public static string GetControllerName<TController>()
		{
			return GetControllerName(typeof (TController).Name);
		}

		private static string GetActionNameHelper(MethodCallExpression call)
		{
			return call.Method.Name;
		}

		private static MethodCallExpression GetRequiredMethodCallExpression<TController>(Expression<Action<TController>> action)
		{
			var call = action.Body as MethodCallExpression;
			if (call == null)
			{
				throw new ArgumentException("The LINQ expression must be a method call.");
			}
			return call;
		}

		private static string GetControllerName(string controllerName)
		{
			if (!controllerName.EndsWith("Controller", StringComparison.OrdinalIgnoreCase))
			{
				throw new ArgumentException("The name of the generic TController type argument does not end with 'Controller'.");
			}

			controllerName = controllerName.Substring(0, controllerName.Length - "Controller".Length);
			if (controllerName.Length == 0)
			{
				throw new ArgumentException("Cannot route to the Controller super layer type.");
			}

			return controllerName;
		}

		private static void AddParameterValuesFromExpressionToDictionary(RouteValueDictionary routeValues,
																		 MethodCallExpression call, string prefix, INameValueSerializer nameValueSerializer)
		{
			ParameterInfo[] parameters = call.Method.GetParameters();

			if (parameters.Length > 0)
			{
				for (int i = 0; i < parameters.Length; i++)
				{
					Expression arg = call.Arguments[i];
					object value;
					var ce = arg as ConstantExpression;
					if (ce != null)
					{
						// If argument is a constant expression, just get the value
						value = ce.Value;
					}
					else
					{
						// Otherwise, convert the argument subexpression to type object,
						// make a lambda out of it, compile it, and invoke it to get the value
						Expression<Func<object>> lambdaExpression = Expression.Lambda<Func<object>>(Expression.Convert(arg, typeof(object)));
						Func<object> func = lambdaExpression.Compile();
						value = func();
					}

					if (value == null)
					{
						continue;
					}

					// Simple argumenter uden prefix bliver navngivet ved navnet på parameteren
					string prefixToUse = prefix;
					if (value.GetType().IsSimpleType() && prefix.IsEmpty())
					{
						prefixToUse = parameters[i].Name.ToLowerInvariant();
					}

					var values = nameValueSerializer.GetValues(value, prefixToUse);
					routeValues.Merge(values);
				}
			}
		}
	}
}