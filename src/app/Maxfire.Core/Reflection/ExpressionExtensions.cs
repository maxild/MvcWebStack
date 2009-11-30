using System;
using System.Linq.Expressions;

namespace Maxfire.Core.Reflection
{
	public static class ExpressionExtensions
	{
		public static string GetNameFor<T, TValue>(this Expression<Func<T, TValue>> expression) where T : class
		{
			return new ExpressionNameVisitor().ToString(expression.Body);
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

		public static bool IsIndexerProperty(this MethodCallExpression expression)
		{
			//TODO: Is there a more certain way to determine if this is an indexed property?
			return expression != null && expression.Method.Name == "get_Item" && expression.Arguments.Count == 1;
		}
	}
}