using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Maxfire.Core.Reflection
{
	public static class AttributeExtensions
	{
		public static bool HasCustomAttribute<T>(this ICustomAttributeProvider member) where T : Attribute
		{
			return member.GetCustomAttributes(typeof(T), false).Length == 1;
		}

		public static bool HasCustomAttribute<T>(this Type type)
		{
			return type.HasCustomAttribute(typeof(T));
		}

		public static bool HasCustomAttribute(this Type type, Type attributeType)
		{
			return TypeDescriptor.GetAttributes(type)[attributeType] != null;
		}

		public static T GetCustomAttribute<T>(this ICustomAttributeProvider member) where T : Attribute
		{
			return member.GetCustomAttribute<T>(false);
		}

		public static T GetCustomAttribute<T>(this ICustomAttributeProvider member, bool inherit) where T : Attribute
		{
			return member.GetCustomAttributes(typeof(T), inherit).Cast<T>().FirstOrDefault();
		}
	}
}