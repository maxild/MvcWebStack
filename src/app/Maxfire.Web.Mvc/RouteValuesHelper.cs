using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using Maxfire.Core.Extensions;

namespace Maxfire.Web.Mvc
{
	public class RouteValuesHelper
	{
		public static RouteValueDictionary GetRouteValuesFromExpression<TController>(Expression<Action<TController>> action)
			where TController : Controller
		{
			return GetRouteValuesFromExpression(action, null, string.Empty);
		}

		public static RouteValueDictionary GetRouteValuesFromExpression<TController>(Expression<Action<TController>> action, string controllerName)
			where TController : Controller
		{
			return GetRouteValuesFromExpression(action, controllerName, string.Empty);
		}

		public static RouteValueDictionary GetRouteValuesFromExpression<TController>(Expression<Action<TController>> action, string controllerName, string prefix) 
			where TController : Controller
		{
			action.ThrowIfNull("action");

			MethodCallExpression call = action.Body as MethodCallExpression;
			if (call == null)
			{
				throw new ArgumentException("The LINQ expression must be a method call.");
			}

			controllerName = controllerName ?? typeof(TController).Name;
			if (!controllerName.EndsWith("Controller", StringComparison.OrdinalIgnoreCase))
			{
				throw new ArgumentException("The name of the generic TController type argument does not end with 'Controller'.");
			}

			controllerName = controllerName.Substring(0, controllerName.Length - "Controller".Length);
			if (controllerName.Length == 0)
			{
				throw new ArgumentException("Cannot route to the Controller super layer type.", "action");
			}

			var routeValues = new RouteValueDictionary { { "Controller", controllerName }, { "Action", call.Method.Name } };
			addParameterValuesFromExpressionToDictionary(routeValues, call, prefix);
			
			return routeValues;
		}

		private static void addParameterValuesFromExpressionToDictionary(RouteValueDictionary routeValues, 
		                                                                 MethodCallExpression call, string prefix)
		{
			ParameterInfo[] parameters = call.Method.GetParameters();

			if (parameters.Length > 0)
			{
				for (int i = 0; i < parameters.Length; i++)
				{
					Expression arg = call.Arguments[i];
					object value;
					ConstantExpression ce = arg as ConstantExpression;
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
					if (value.GetType().IsSimpleType())
					{
						routeValues.Add(parameters[i].Name, value);
					}
					else
					{
						routeValues.AddValuesWithDotNotation(value, prefix);
					}
				}
			}
		}
	}
}