using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace Maxfire.Core.Reflection
{
	public static class ReflectionHelper
	{
		public static PropertyDescriptor GetProperty<T>(Expression<Func<T, object>> expression)
		{
			string propertyName = ExpressionHelper.GetProperty(expression).Name;
			return GetProperty<T>(propertyName);
		}

		public static PropertyDescriptor GetProperty<T>(string propertyName)
		{
			return TypeDescriptor.GetProperties(typeof(T))
				.Cast<PropertyDescriptor>()
				.Where(p => p.Name == propertyName)
				.Single();
		}
	}
}