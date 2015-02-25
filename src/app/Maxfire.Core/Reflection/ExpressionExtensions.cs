using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Maxfire.Core.Reflection
{
	public static class ExpressionExtensions
	{
		public static string GetNameFor<T, TValue>(this Expression<Func<T, TValue>> expression)
		{
			return ExpressionHelper.GetExpressionText(expression);
		}

		public static TValue GetValueFrom<T, TValue>(this Expression<Func<T, TValue>> expression, T viewModel) where T : class
		{
			try
			{
				return viewModel == null
				       	? default(TValue)
				       	: expression.Compile().Invoke(viewModel);
			}
			catch (Exception)
			{
				return default(TValue);
			}
		}

		public static bool IsArrayGetMethod(this MethodCallExpression methodExpression)
		{
			if (methodExpression == null || methodExpression.Arguments.Count < 2)
			{
				return false;
			}

			return (methodExpression.Method.DeclaringType != null && methodExpression.Method.DeclaringType.IsArray) &&
			       methodExpression.Method.Name.Equals("Get");
		}

		public static bool IsIndexerProperty(this MethodCallExpression methodExpression)
		{
			if (methodExpression == null || methodExpression.Arguments.Count == 0)
			{
				return false;
			}
			// Does any property getters of the declaring type annotated with DefaultMemberAttribute correspond to this method
			return methodExpression.Method.DeclaringType != null && methodExpression.Method.DeclaringType
			                                                                 .GetDefaultMembers()
			                                                                 .OfType<PropertyInfo>()
			                                                                 .Any(p => p.GetGetMethod() == methodExpression.Method);
		}
	}
}