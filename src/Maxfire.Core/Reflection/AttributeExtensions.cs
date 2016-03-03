using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Maxfire.Core.Extensions;

namespace Maxfire.Core.Reflection
{
	public static class AttributeExtensions
	{
		public static bool HasSingleCustomAttribute<T>(this ICustomAttributeProvider member, bool inherit = false) where T : Attribute
		{
			return member.GetCustomAttributes(typeof(T), inherit).Length == 1;
		}

		public static bool HasCustomAttribute<T>(this ICustomAttributeProvider member, bool inherit = false) 
			where T : Attribute
		{
			return member.GetCustomAttributes<T>(inherit).IsNotEmpty();
		}

		public static T GetCustomAttribute<T>(this ICustomAttributeProvider member, bool inherit = false) 
			where T : Attribute
		{
			return member.GetCustomAttributes<T>(inherit).FirstOrDefault();
		}

		public static IEnumerable<T> GetCustomAttributes<T>(this ICustomAttributeProvider member, bool inherit = false) 
			where T : Attribute
		{
			return member.GetCustomAttributes(typeof (T), inherit).Cast<T>();
		}
	}
}