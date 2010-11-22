using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Maxfire.Core.Extensions;
using Maxfire.Core.Reflection;

namespace Maxfire.Web.Mvc
{
	public static class OptionsAdapter2
	{
		public static IEnumerable<SelectListItem> FromEnumValues(Type enumType)
		{
			return FromEnumHelper(enumType, x => Convert.ToInt32(x).ToString());
		}

		public static IEnumerable<SelectListItem> FromEnumValues<TEnum>()
		{
			return FromEnumHelper<TEnum>(x => Convert.ToInt32(x).ToString());
		}

		public static IEnumerable<SelectListItem> FromEnumTexts(Type enumType)
		{
			return FromEnumHelper(enumType, x => x.ToString());
		}

		public static IEnumerable<SelectListItem> FromEnumTexts<TEnum>()
		{
			return FromEnumHelper<TEnum>(x => x.ToString());
		}

		private static IEnumerable<SelectListItem> FromEnumHelper<TEnum>(Func<TEnum, string> valueSelector)
		{
			var enumType = typeof (TEnum);
			if (!enumType.IsEnum)
			{
				throw new ArgumentException("The generic type argument must be an enum.");
			}
			var values = Enum.GetValues(enumType).Cast<TEnum>();
			return new OptionsAdapter2<TEnum>(values, x => x.GetDisplayNameOfEnum(), valueSelector);
		}

		private static IEnumerable<SelectListItem> FromEnumHelper(Type enumType, Func<object, string> valueSelector)
		{
			if (!enumType.IsEnum)
			{
				throw new ArgumentException("The generic type argument must be an enum.");
			}
			var values = Enum.GetValues(enumType).Cast<object>();
			return new OptionsAdapter2<object>(values, x => x.GetDisplayNameOfEnum(enumType), valueSelector);
		}

		public static IEnumerable<SelectListItem> FromCollection<T>(IEnumerable<T> options, Func<T, string> textSelector, Func<T, object > valueSelector)
		{
			return new OptionsAdapter2<T>(options, textSelector, option => valueSelector(option).ToString());
		}

		public static IEnumerable<SelectListItem> FromCollection<T>(IEnumerable<T> options, Func<T, string> textSelector, Func<T, string> valueSelector)
		{
			return new OptionsAdapter2<T>(options, textSelector, valueSelector);
		}

		public static IEnumerable<SelectListItem> FromCollection<T>(IEnumerable<T> options)
		{
			return new OptionsAdapter2<T>(options, x => x.ToString(), x => x.ToString());
		}

		public static IEnumerable<SelectListItem> FromTextValuePairs<T>(IEnumerable<T> options)
		{
			return new SelectList(options, "Value", "Text");
		}

		public static IEnumerable<SelectListItem> FromDictionary<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> options)
		{
			return new OptionsAdapter2<KeyValuePair<TKey, TValue>>(options, 
				kvp => kvp.Key.ToString(), 
				kvp => kvp.Value.ToString());
		}

		public static IEnumerable<SelectListItem> FromDictionary(object options)
		{
			return FromDictionary(options.ToPropertyStringValuePairs());
		}

		public static IEnumerable<SelectListItem> FromDictionary(params Func<string, string>[] options)
		{
			return new OptionsAdapter2<Func<string, string>>(options, 
				func => func.Method.GetParameters()[0].Name, 
				func => func(null));
		}

		public static IEnumerable<SelectListItem> Boolean()
		{
			yield return new SelectListItem { Text = "Ja", Value = "true" };
			yield return new SelectListItem { Text = "Nej", Value = "false" };
		}

		public static IEnumerable<SelectListItem> Months()
		{
			yield return new SelectListItem { Text = "Januar", Value = "1" };
			yield return new SelectListItem { Text = "Februar", Value = "2" };
			yield return new SelectListItem { Text = "Marts", Value = "3" };
			yield return new SelectListItem { Text = "April", Value = "4" };
			yield return new SelectListItem { Text = "Maj", Value = "5" };
			yield return new SelectListItem { Text = "Juni", Value = "6" };
			yield return new SelectListItem { Text = "Juli", Value = "7" };
			yield return new SelectListItem { Text = "August", Value = "8" };
			yield return new SelectListItem { Text = "September", Value = "9" };
			yield return new SelectListItem { Text = "Oktober", Value = "10" };
			yield return new SelectListItem { Text = "November", Value = "11" };
			yield return new SelectListItem { Text = "December", Value = "12" };
		}
	}

	public class OptionsAdapter2<T> : IEnumerable<SelectListItem>
	{
		private readonly IEnumerable<T> _options;
		private readonly string _selectedValue;
		private readonly Func<T, string> _textSelector;
		private readonly Func<T, string> _valueSelector;

		public OptionsAdapter2(IEnumerable<T> options, Func<T, string> textSelector, Func<T, string> valueSelector)
		{
			if (textSelector == null) throw new ArgumentNullException("textSelector");
			if (valueSelector == null) throw new ArgumentNullException("valueSelector");
			_options = options;
			_textSelector = textSelector;
			_valueSelector = valueSelector;
			_selectedValue = null; // selectedValue;
		}

		public IEnumerator<SelectListItem> GetEnumerator()
		{
			if (_options == null)
			{
				return Enumerable.Empty<SelectListItem>().GetEnumerator();
			}

			var listItems = from option in _options
							let valueText = _valueSelector(option)
							select new SelectListItem
							{
								Value = valueText,
								Text = _textSelector(option),
								Selected = _selectedValue != null && _selectedValue.Equals(valueText)
							};

			return listItems.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}