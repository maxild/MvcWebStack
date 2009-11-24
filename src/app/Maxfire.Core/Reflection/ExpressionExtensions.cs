using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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

		public static string GetDisplayName(this MemberInfo property)
		{
			var displayNameAttribute = property.GetCustomAttribute<DisplayNameAttribute>();
			string displayName = displayNameAttribute != null ? displayNameAttribute.DisplayName: property.Name;
			return displayName;
		}

		public static string GetDisplayName(this PropertyDescriptor descriptor)
		{
			var displayNameAttribute = descriptor.Attributes.OfType<DisplayNameAttribute>().FirstOrDefault();
			string displayName = displayNameAttribute != null ? displayNameAttribute.DisplayName: descriptor.Name;
			return displayName;
		}
	}
}