using System;
using System.Globalization;
using System.Linq;
using Maxfire.Core.Extensions;
using Maxfire.Prelude;

namespace Maxfire.Web.Mvc
{
	public class MappingConventions
	{
		public MappingConventions()
		{
			SetDefaults();
		}

		public void SetDefaults()
		{
			const NumberStyles DECIMAL_STYLES = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign;

			const NumberStyles MONEY_STYLES =
				NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign | NumberStyles.AllowLeadingWhite |
				NumberStyles.AllowTrailingWhite | NumberStyles.AllowThousands;

			const DateTimeStyles DATE_STYLES = DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite;

			decimal decimalValue;
			DateTime dateValue;
			short shortValue;
			int intValue;
			long longValue;

			// See also: http://msdn.microsoft.com/en-us/library/dwhawy9k(VS.95).aspx
			// If the precision specifier is omitted or zero, the type of the number determines the
			// default precision, as indicated by the following list:
			//
			// Byte or SByte: 3
			// Int16 or UInt16: 5
			// Int32 or UInt32: 10
			// Int64: 19
			// UInt64: 20
			// Single: 7
			// Double: 15
			// Decimal: 29
			//

			ValidateMachineDecimal = (s => decimal.TryParse(s.ToTrimmedNullSafeString(), DECIMAL_STYLES, CultureInfo.InvariantCulture, out decimalValue));
			ParseMachineDecimal = (s => decimal.Parse(s.ToTrimmedNullSafeString(), DECIMAL_STYLES, CultureInfo.InvariantCulture));

			// .....The exception to the preceding rule is if the number is a Decimal and
			// the precision specifier is omitted. In that case, fixed-point notation is
			// always used and trailing zeroes are preserved.
			// The “G” format with a number means to format that many significant digits.
			// Because 29 is the most significant digits that a Decimal can have, this will
			// effectively truncate the trailing zeros without rounding.
			FormatMachineDecimal = (value => value.ToString("G29", CultureInfo.InvariantCulture));

			// Human readable
			ValidateDecimal = (s => decimal.TryParse(s.ToTrimmedNullSafeString(), DECIMAL_STYLES, CultureInfo.CurrentCulture, out decimalValue));
			ParseDecimal = (s => decimal.Parse(s.ToTrimmedNullSafeString(), DECIMAL_STYLES, CultureInfo.CurrentCulture));
			FormatDecimal = (value => value.ToString("G29", CultureInfo.CurrentCulture));
			FormatDecimalAsPctSats = (value => String.Format("{0:G29} pct.", value * 100.0m));

			ValidateMoney = (s => decimal.TryParse(s.ToTrimmedNullSafeString(), MONEY_STYLES, CultureInfo.CurrentCulture, out decimalValue));
			ParseMoney = (s => decimal.Parse(s.ToTrimmedNullSafeString(), MONEY_STYLES, CultureInfo.CurrentCulture));

			ValidateDate = (s =>
				DateTime.TryParseExact(s.ToTrimmedNullSafeString(), "dd-MM-yyyy", CultureInfo.CurrentCulture, DATE_STYLES, out dateValue) &&
				dateValue.IsDate());
			ParseDate = (s => DateTime.ParseExact(s.ToTrimmedNullSafeString(), "dd-MM-yyyy", CultureInfo.CurrentCulture, DATE_STYLES));
			FormatDate = (value => value.ToString("dd-MM-yyyy"));

			ValidateBoolean = (s => "true".Equals(s, StringComparison.Ordinal) || "false".Equals(s, StringComparison.Ordinal));
			ParseBoolean = (s =>
			{
				if (!ValidateBoolean(s)) throw new FormatException("Boolean value must be formatted as 'true' or 'false'.");
				return "true".Equals(s, StringComparison.InvariantCultureIgnoreCase);
			});
			FormatBoolean = (value => value ? "true" : "false");

			TryValidateInt16 = (s =>
			{
				short? val = null;
				if (short.TryParse(s.ToTrimmedNullSafeString(), out shortValue))
				{
					val = shortValue;
				}
				return val;
			});
			ValidateInt16 = (s => short.TryParse(s.ToTrimmedNullSafeString(), out shortValue));
			ParseInt16 = (s => short.Parse(s.ToTrimmedNullSafeString()));
			FormatInt16 = (value => value.ToString());

			TryValidateInt32 = (s =>
			{
				int? val = null;
				if (int.TryParse(s.ToTrimmedNullSafeString(), out intValue))
				{
					val = intValue;
				}
				return val;
			});
			ValidateInt32 = (s => int.TryParse(s.ToTrimmedNullSafeString(), out intValue));
			ParseInt32 = (s => int.Parse(s.ToTrimmedNullSafeString()));
			FormatInt32 = (value => value.ToString());

			TryValidateInt64 = (s =>
			{
				long? val = null;
				if (long.TryParse(s.ToTrimmedNullSafeString(), out longValue))
				{
					val = longValue;
				}
				return val;
			});
			ValidateInt64 = (s => long.TryParse(s.ToTrimmedNullSafeString(), out longValue));
			ParseInt64 = (s => long.Parse(s.ToTrimmedNullSafeString()));
			FormatInt64 = (value => value.ToString());

			ValidateNullableInt32 = (s => s.IsTrimmedEmpty() || int.TryParse(s.ToTrimmedNullSafeString(), out intValue));
			ParseNullableInt32 = (s => s.IsTrimmedEmpty() ? (int?)null : int.Parse(s.ToTrimmedNullSafeString()));
			FormatNullableInt32 = (value => value.ToString());
		}

		public Func<string, bool> ValidateMachineDecimal { get; set; }
		public Func<string, decimal> ParseMachineDecimal { get; set; }
		public Func<decimal, string> FormatMachineDecimal { get; set; }

		public Func<string, bool> ValidateDecimal { get; set; }
		public Func<string, decimal> ParseDecimal { get; set; }
		public Func<decimal, string> FormatDecimal { get; set; }
		public Func<decimal, string> FormatDecimalAsPctSats { get; set; }

		public Func<string, bool> ValidateMoney { get; set; }
		public Func<string, decimal> ParseMoney { get; set; }

		public Func<string, bool> ValidateDate { get; set; }
		public Func<string, DateTime> ParseDate { get; set; }
		public Func<DateTime, string> FormatDate { get; set; }

		public Func<string, bool> ValidateBoolean { get; set; }
		public Func<string, bool> ParseBoolean { get; set; }
		public Func<bool, string> FormatBoolean { get; set; }

		public Func<string, short?> TryValidateInt16 { get; set; }
		public Func<string, bool> ValidateInt16 { get; set; }
		public Func<string, short> ParseInt16 { get; set; }
		public Func<short, string> FormatInt16 { get; set; }

		public Func<string, int?> TryValidateInt32 { get; set; }
		public Func<string, bool> ValidateInt32 { get; set; }
		public Func<string, int> ParseInt32 { get; set; }
		public Func<int, string> FormatInt32 { get; set; }

		public Func<string, long?> TryValidateInt64 { get; set; }
		public Func<string, bool> ValidateInt64 { get; set; }
		public Func<string, long> ParseInt64 { get; set; }
		public Func<long, string> FormatInt64 { get; set; }

		public Func<string, bool> ValidateNullableInt32 { get; set; }
		public Func<string, int?> ParseNullableInt32 { get; set; }
		public Func<int?, string> FormatNullableInt32 { get; set; }

		public virtual string FormatMoney(decimal value)
		{
			return value.ToString("#,0.00", CultureInfo.CurrentCulture);
		}

		public virtual string FormatMoney(double value)
		{
			return value.ToString("#,0.00", CultureInfo.CurrentCulture);
		}

		public virtual string FormatMoneyDkk(decimal value)
		{
			return String.Format("{0:#,0.00} kr.", value);
		}

		public virtual string FormatMoneyDkk(double value)
		{
			return String.Format("{0:#,0.00} kr.", value);
		}

		public virtual bool ValidateEnumOfType<T>(string s)
		{
			return ValidateEnumOfType(typeof(T), s);
		}

		public virtual bool ValidateEnumOfType(Type enumType, string s)
		{
			checkEnumTypeArgument(enumType);
			s = s.ToTrimmedNullSafeString();
			bool enumHasSuchName = Enum.GetNames(enumType).Any(name => name == s.ToTrimmedNullSafeString());
			if (!enumHasSuchName)
			{
				Array values = Enum.GetValues(enumType);
				int value;
				if (int.TryParse(s, out value))
				{
					for (int i = 0; i < values.Length; i++)
					{
						if (value == Convert.ToInt32(values.GetValue(i)))
						{
							return true;
						}
					}
				}
				return false;
			}
			return true;
		}

		public virtual T ParseEnumOfType<T>(string s)
		{
			checkEnumTypeArgument<T>();
			return (T)Enum.Parse(typeof(T), s.ToTrimmedNullSafeString(), true);
		}

		public virtual string FormatEnum<T>(T e)
		{
			checkEnumTypeArgument<T>();
			return e.ToString();
		}

		public virtual string FormatEnumAsValue<T>(T e)
		{
			checkEnumTypeArgument<T>();
			return Convert.ToInt32(e).ToString();
		}

		private static void checkEnumTypeArgument<T>()
		{
			checkEnumTypeArgument(typeof(T));
		}

		private static void checkEnumTypeArgument(Type enumType)
		{
			if (!enumType.IsEnum)
			{
				throw new ArgumentException("The generic type argument is not an enum.");
			}
		}

		public virtual bool ValidateEnumerationOfType<T>(string s) where T : Enumeration<T>
		{
			return (Enumeration.FromNameOrDefault<T>(s.ToTrimmedNullSafeString()) != null);
		}

		public virtual bool ValidateEnumerationOfType(Type enumerationType, string s)
		{
			return (Enumeration.FromNameOrDefault(enumerationType, s.ToTrimmedNullSafeString()) != null);
		}

		public virtual T ParseEnumerationOfType<T>(string s) where T : Enumeration<T>
		{
			return Enumeration.FromName<T>(s.ToTrimmedNullSafeString());
		}

		public virtual string FormatEnumerationOfType<T>(T value) where T : Enumeration
		{
			if (value == null)
			{
				throw new ArgumentNullException(nameof(value));
			}

			return value.Name;
		}

		public virtual string FormatBooleanToJaNej(bool value)
		{
			return value ? "Ja" : "Nej";
		}
	}

	public static class DateTimeExtensions
	{
		public static bool IsDate(this DateTime date)
		{
			return (date.TimeOfDay.TotalMilliseconds == 0);
		}
	}
}
