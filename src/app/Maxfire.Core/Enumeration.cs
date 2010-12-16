using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Maxfire.Core.Extensions;

namespace Maxfire.Core
{
	[Serializable]
	public abstract class Enumeration : IFormattable
	{
		public static IEnumerable<TEnumeration> GetAll<TEnumeration>() 
			where TEnumeration : Enumeration
		{
			return getFields(typeof(TEnumeration))
				.Select(field => field.GetValue(null))
				.OfType<TEnumeration>();
		}

		public static IEnumerable GetAll(Type enumerationType)
		{
			return getFields(enumerationType)
				.Select(field => field.GetValue(null))
				.Where(fieldValue => fieldValue != null);
		}

		public static IEnumerable<string> GetAllNames<TEnumeration>() 
			where TEnumeration : Enumeration
		{
			return GetAllNames(typeof(TEnumeration));
		}

		public static IEnumerable<string> GetAllNames(Type enumerationType)
		{
			return getFields(enumerationType)
				.Select(field => field.GetValue(null))
				.OfType<Enumeration>()
				.Select(fieldValue => fieldValue.Name);
		}

		public static TEnumeration FromValueOrDefault<TEnumeration>(long value)
			where TEnumeration : Enumeration
		{
			return FromValueOrDefault<TEnumeration>((int)value);
		}

		public static TEnumeration FromValueOrDefault<TEnumeration>(int value)
			where TEnumeration : Enumeration
		{
			TEnumeration matchingItem = parse<TEnumeration, int>(value, "value", item => item.Value == value, false);
			return matchingItem;
		}

		public static TEnumeration FromValue<TEnumeration>(long value)
			where TEnumeration : Enumeration
		{
			return FromValue<TEnumeration>((int)value);
		}

		public static TEnumeration FromValue<TEnumeration>(int value)
			where TEnumeration : Enumeration
		{
			TEnumeration matchingItem = parse<TEnumeration, int>(value, "value", item => item.Value == value);
			return matchingItem;
		}

		public static TEnumeration FromNameOrDefault<TEnumeration>(string name)
			where TEnumeration : Enumeration
		{
			TEnumeration matchingItem = parse<TEnumeration, string>(name, "name", item => equalName(item.Name, name), false);
			return matchingItem;
		}

		public static Enumeration FromNameOrDefault(Type enumerationType, string name)
		{
			Enumeration matchingItem = parse(enumerationType, name, "name", item => equalName(item.Name, name), false);
			return matchingItem;
		}

		public static TEnumeration FromName<TEnumeration>(string name)
			where TEnumeration : Enumeration
		{
			TEnumeration matchingItem = parse<TEnumeration, string>(name, "name", item => equalName(item.Name, name));
			return matchingItem;
		}

		private static bool equalName(string name1, string name2)
		{
			return String.Compare(name1, name2, StringComparison.InvariantCultureIgnoreCase) == 0;
		}

		private static IEnumerable<FieldInfo> getFields(Type type)
		{
			return type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
		}

		private static TEnumeration parse<TEnumeration, TValue>(TValue value, string description, Func<TEnumeration, bool> predicate, bool enforceCheck = true)
			where TEnumeration : Enumeration
		{
			TEnumeration matchingItem = GetAll<TEnumeration>().FirstOrDefault(predicate);

			if (enforceCheck && matchingItem == null)
			{
				string message = string.Format("'{0}' is not a valid {1} for '{2}'.", value, description, typeof(TEnumeration));
				throw new ArgumentException(message);
			}

			return matchingItem;
		}

		private static Enumeration parse<TValue>(Type enumerationType, TValue value, string description, Func<Enumeration, bool> predicate, bool enforceCheck)
		{
			Enumeration matchingItem = GetAll(enumerationType)
				.Cast<Enumeration>()
				.FirstOrDefault(predicate);

			if (enforceCheck && matchingItem == null)
			{
				string message = string.Format("'{0}' is not a valid {1} for '{2}'.", value, description, enumerationType);
				throw new ArgumentException(message);
			}

			return matchingItem;
		}

		private readonly int _id;
		private readonly string _name;
		private readonly string _text;

		protected Enumeration(int value, string name, string text)
		{
			if (name.IsEmpty())
				throw new ArgumentNullException("name");
			
			_id = value;
			_name = name;
			_text = text ?? string.Empty;
		}

		protected Enumeration()
		{
		}

		public override bool Equals(object obj)
		{
			var other = obj as Enumeration;

			if (other == null)
			{
				return false;
			}

			return (GetType().Equals(other.GetType()) && Value.Equals(other.Value));
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		public int Value
		{
			get { return _id; }
		}

		public string Name
		{
			get { return _name; }
		}

		public string Text
		{
			get { return _text; }
		}

		
		public override string ToString()
		{
			return ToString("G", null);
		}

		public string ToString(string format, IFormatProvider formatProvider)
		{
			if (formatProvider != null)
			{
				var fmt = formatProvider.GetFormat(GetType()) as ICustomFormatter;
				if (fmt != null)
				{
					return fmt.Format(format, this, formatProvider);
				}
			}

			format = format ?? "G";

			switch (format.ToUpperInvariant())
			{
				case "V":
					return Value.ToString();
				case "T":
					return Text;
				case "G":
					return Name;
				default:
					throw new FormatException(String.Format("Unsupported format '{0}'", format));
			}
		}
	}

	[Serializable]
	public abstract class ComparableEnumeration<TEnumeration> : Enumeration, IEquatable<TEnumeration>, IComparable, IComparable<TEnumeration>
		where TEnumeration : ComparableEnumeration<TEnumeration>
	{
		protected ComparableEnumeration(int value, string name, string text) : base(value, name, text)
		{
		}

		protected ComparableEnumeration()
		{
		}

		public virtual int CompareTo(object other)
		{
			return CompareTo(other as TEnumeration);
		}

		public int CompareTo(TEnumeration other)
		{
			if (other == null || GetType() != other.GetType()) return 1;
			return Value.CompareTo(other.Value);
		}

		public bool Equals(TEnumeration other)
		{
			return base.Equals(other);
		}
	}

	[Serializable]
	public abstract class ConvertibleEnumeration<TEnumeration> : ComparableEnumeration<TEnumeration>, IConvertible
		where TEnumeration : ConvertibleEnumeration<TEnumeration>
	{
		protected ConvertibleEnumeration()
		{
		}

		protected ConvertibleEnumeration(int value, string name, string text)
			: base(value, name, text)
		{
		}

		#region IConvertible Members

		TypeCode IConvertible.GetTypeCode()
		{
			return TypeCode.Int32;
		}

		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			return Convert.ToBoolean(Value, provider);
		}

		char IConvertible.ToChar(IFormatProvider provider)
		{
			return Convert.ToChar(Value, provider);
		}

		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			return Convert.ToSByte(Value, provider);
		}

		byte IConvertible.ToByte(IFormatProvider provider)
		{
			return Convert.ToByte(Value, provider);
		}

		short IConvertible.ToInt16(IFormatProvider provider)
		{
			return Convert.ToInt16(Value, provider);
		}

		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			return Convert.ToUInt16(Value, provider);
		}

		int IConvertible.ToInt32(IFormatProvider provider)
		{
			return Convert.ToInt32(Value, provider);
		}

		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			return Convert.ToUInt32(Value, provider);
		}

		long IConvertible.ToInt64(IFormatProvider provider)
		{
			return Convert.ToInt64(Value, provider);
		}

		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			return Convert.ToUInt64(Value, provider);
		}

		float IConvertible.ToSingle(IFormatProvider provider)
		{
			return Convert.ToSingle(Value, provider);
		}

		double IConvertible.ToDouble(IFormatProvider provider)
		{
			return Convert.ToDouble(Value, provider);
		}

		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			return Convert.ToDecimal(Value, provider);
		}

		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			throw new InvalidCastException(getInvalidCastMessage(typeof(DateTime)));
		}

		string IConvertible.ToString(IFormatProvider provider)
		{
			return ToString();
		}

		object IConvertible.ToType(Type targetType, IFormatProvider provider)
		{
			if (targetType == null)
				throw new ArgumentNullException("targetType");
			if (GetType() == targetType || targetType == typeof(object))
				return this;

			IConvertible convertible = this;

			if (targetType == typeof(bool))
			{
				return convertible.ToBoolean(provider);
			}
			if (targetType == typeof(char))
			{
				return convertible.ToChar(provider);
			}
			if (targetType == typeof(sbyte))
			{
				return convertible.ToSByte(provider);
			}
			if (targetType == typeof(byte))
			{
				return convertible.ToByte(provider);
			}
			if (targetType == typeof(short))
			{
				return convertible.ToInt16(provider);
			}
			if (targetType == typeof(ushort))
			{
				return convertible.ToUInt16(provider);
			}
			if (targetType == typeof(int))
			{
				return convertible.ToInt32(provider);
			}
			if (targetType == typeof(uint))
			{
				return convertible.ToUInt32(provider);
			}
			if (targetType == typeof(long))
			{
				return convertible.ToInt64(provider);
			}
			if (targetType == typeof(ulong))
			{
				return convertible.ToUInt64(provider);
			}
			if (targetType == typeof(float))
			{
				return convertible.ToSingle(provider);
			}
			if (targetType == typeof(double))
			{
				return convertible.ToDouble(provider);
			}
			if (targetType == typeof(decimal))
			{
				return convertible.ToDecimal(provider);
			}
			if (targetType == typeof(DateTime))
			{
				return convertible.ToDateTime(provider);
			}
			if (targetType == typeof(string))
			{
				return convertible.ToString(provider);
			}
			throw new InvalidCastException(getInvalidCastMessage(targetType));
		}

		private string getInvalidCastMessage(Type targetType)
		{
			return String.Format("A value of type '{0}' cannot be converted to a value of type '{1}'.", GetType().FullName, targetType.FullName);
		}

		#endregion
	}
}