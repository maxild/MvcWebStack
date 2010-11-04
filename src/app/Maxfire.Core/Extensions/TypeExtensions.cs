using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Maxfire.Core.Extensions
{
	public static class TypeExtensions
	{
		public static object GetDefaultValue(this Type type)
		{
			return (type.AllowsNullValue()) ? null : Activator.CreateInstance(type);
		}

		public static bool IsNullableValueType(this Type type)
		{
			return Nullable.GetUnderlyingType(type) != null;
		}

		public static bool AllowsNullValue(this Type type)
		{
			return (!type.IsValueType || IsNullableValueType(type));
		}

		public static Type MatchesGenericInterface(this Type type, Type genericInterfaceType)
		{
			Func<Type, bool> matchesInterface = t => t.IsGenericType && t.GetGenericTypeDefinition() == genericInterfaceType;
			return (matchesInterface(type)) ? type : type.GetInterfaces().FirstOrDefault(matchesInterface);
		}

		public static bool IsSimpleType(this Type propertyType)
		{
			if (propertyType.IsArray)
			{
				return false;
			}

			bool isSimple = propertyType.IsPrimitive ||
			                propertyType.IsEnum ||
			                propertyType == typeof(String) ||
			                propertyType == typeof(Guid) ||
			                propertyType == typeof(DateTime) ||
			                propertyType == typeof(Decimal);

			if (isSimple)
			{
				return true;
			}

			TypeConverter typeConverter = TypeDescriptor.GetConverter(propertyType);

			return typeConverter.CanConvertFrom(typeof(String));
		}

		public static object ConvertSimpleType(CultureInfo culture, object value, Type destinationType)
		{
			if (value == null || destinationType.IsInstanceOfType(value))
			{
				return value;
			}

			TypeConverter converter = TypeDescriptor.GetConverter(destinationType);
			bool canConvertFrom = converter.CanConvertFrom(value.GetType());
			if (!canConvertFrom)
			{
				converter = TypeDescriptor.GetConverter(value.GetType());
				if (!converter.CanConvertTo(destinationType))
				{
					string message = String.Format("No converter exists that can convert from type '{0}' to type '{1}'.",
					                               value.GetType().FullName, destinationType.FullName);
					throw new InvalidOperationException(message);
				}
			}

			try
			{
				object convertedValue = canConvertFrom ?
				                                       	converter.ConvertFrom(null, culture, value) :
				                                       	                                            	converter.ConvertTo(null, culture, value, destinationType);
				return convertedValue;
			}
			catch (Exception ex)
			{
				string message = String.Format("The converter threw an exception when converting from type '{0}' to type '{1}'.",
				                               value.GetType().FullName, destinationType.FullName);
				throw new InvalidOperationException(message, ex);
			}
		}
	}
}