using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Maxfire.Core.Extensions;
using EnumDisplayNameAttribute = Maxfire.Core.ComponentModel.DisplayNameAttribute;

namespace Maxfire.Core.Reflection
{
	public static class DisplayNameExtensions
	{
		public static string GetDisplayName(this MemberInfo property)
		{
			var displayNameAttribute = property.GetCustomAttribute<DisplayNameAttribute>();
			if (displayNameAttribute != null)
			{
				return displayNameAttribute.DisplayName;
			}
			
			var enumDisplayNameAttribute = property.GetCustomAttribute<EnumDisplayNameAttribute>();
			if (enumDisplayNameAttribute != null)
			{
				return enumDisplayNameAttribute.DisplayName;
			}
			
			return property.Name;
		}

		public static string GetDisplayName(this PropertyDescriptor descriptor)
		{
			var displayNameAttribute = descriptor.Attributes.OfType<DisplayNameAttribute>().FirstOrDefault();
			if (displayNameAttribute != null)
			{
				return displayNameAttribute.DisplayName;
			}

			var enumDisplayNameAttribute = descriptor.Attributes.OfType<EnumDisplayNameAttribute>().FirstOrDefault();
			if (enumDisplayNameAttribute != null)
			{
				return enumDisplayNameAttribute.DisplayName;
			}

			return descriptor.Name;
		}

		public static string GetDisplayName<TModel>(this Expression<Func<TModel, object>> propertyExpression)
		{
			return ExpressionHelper.GetProperty(propertyExpression).GetDisplayName();
		}

		public static string GetDisplayName<TModel, TProperty>(this Expression<Func<TModel, TProperty>> propertyExpression)
		{
			return ExpressionHelper.GetProperty(propertyExpression).GetDisplayName();
		}

		public static string GetDisplayNameOfEnum(this object value, Type enumType)
		{
			if (enumType.IsGenericType && enumType.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				enumType = enumType.GetGenericArguments()[0];
			}
			if (!enumType.IsEnum)
			{
				throw new ArgumentException("The generic type argument must be an enum.");
			}
			return getDisplayNameOfEnum(value);
		}

		public static string GetDisplayNameOfEnum<TEnum>(this TEnum value)
		{
			Type enumType = typeof(TEnum);
			if (enumType.IsGenericType && enumType.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				enumType = enumType.GetGenericArguments()[0];
			}
			if (!enumType.IsEnum)
			{
				throw new ArgumentException("The generic type argument must be an enum.");
			}
			return getDisplayNameOfEnum(value);
		}

		private static string getDisplayNameOfEnum(object value)
		{
			if (value == null)
			{
				return string.Empty;
			}

			var displayName = value.ToString();
			if (displayName.IsEmpty())
			{
				return string.Empty;
			}

			var field = value.GetType().GetField(displayName, BindingFlags.Static | BindingFlags.GetField | BindingFlags.Public);
			if (field == null)
			{
				return string.Empty;
			}

			var displayNameAttribute = field.GetCustomAttribute<EnumDisplayNameAttribute>();
			return displayNameAttribute == null ? displayName : displayNameAttribute.DisplayName;
		}
	}
}