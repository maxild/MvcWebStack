using System;
using System.ComponentModel;
using System.Globalization;

namespace Maxfire.Core
{
	public class EnumerationConverter<TEnumeration> : TypeConverter where TEnumeration : Enumeration
	{
		private readonly Func<TEnumeration, string> _convertToString;

		public EnumerationConverter()
		{
		}

		protected EnumerationConverter(Func<TEnumeration, string> converter)
		{
			_convertToString = converter;
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return (sourceType == typeof(string) || base.CanConvertFrom(context, sourceType));
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return (destinationType == typeof(string) || base.CanConvertTo(context, destinationType));
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			string s = value as string;
			if (s != null)
			{
				int val;
				TEnumeration result = int.TryParse(s, out val) ?
					Enumeration.FromValue<TEnumeration>(val) :
					Enumeration.FromName<TEnumeration>(s);
				return result;
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (value == null)
				throw new ArgumentNullException("value");
			if (destinationType == typeof(string))
			{
				TEnumeration enumeration = value as TEnumeration;
				if (enumeration == null)
				{
					throw new ArgumentException(String.Format("The value to convert to a string is not of type '{0}'.",
					                                          typeof(TEnumeration).Name), "value");
				}
				return _convertToString != null ? _convertToString(enumeration) : enumeration.Name;
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}

	public class EnumerationToValueConverter<TEnumeration> : EnumerationConverter<TEnumeration> 
		where TEnumeration : Enumeration
	{
		public EnumerationToValueConverter() : base(e => e.Value.ToString())
		{
		}
	}
}