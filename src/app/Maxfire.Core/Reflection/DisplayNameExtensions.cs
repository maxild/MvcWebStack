using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Maxfire.Core.Reflection
{
	public static class DisplayNameExtensions
	{
		public static string GetDisplayName(this MemberInfo property)
		{
			var displayNameAttribute = property.GetCustomAttribute<DisplayNameAttribute>();
			string displayName = displayNameAttribute != null ? displayNameAttribute.DisplayName : property.Name;
			return displayName;
		}

		public static string GetDisplayName(this PropertyDescriptor descriptor)
		{
			var displayNameAttribute = descriptor.Attributes.OfType<DisplayNameAttribute>().FirstOrDefault();
			string displayName = displayNameAttribute != null ? displayNameAttribute.DisplayName : descriptor.Name;
			return displayName;
		}

		public static string GetDisplayName<TModel>(this Expression<Func<TModel, object>> propertyExpression)
		{
			return ExpressionHelper.GetProperty(propertyExpression).GetDisplayName();
		}

		public static string GetDisplayNameOfEnum<TEnum>(this TEnum value)
		{
			Type enumType = typeof(TEnum);
			if (!enumType.IsEnum)
			{
				throw new ArgumentException("The generic type argument must be an enum.");
			}
			return getDisplayNameOfEnum(value);
		}

		public static string GetDisplayName(this Enum value)
		{
			return getDisplayNameOfEnum(value);
		}

		private static string getDisplayNameOfEnum(object value)
		{
			var displayName = value.ToString();

			var field = value.GetType().GetField(displayName, BindingFlags.Static | BindingFlags.GetField | BindingFlags.Public);
			if (field == null)
			{
				return null;
			}

			var displayNameAttribute = field.GetCustomAttribute<DisplayNameAttribute>();
			if (displayNameAttribute != null)
			{
				displayName = displayNameAttribute.DisplayName;
			}

			return displayName;
		}
	}
}