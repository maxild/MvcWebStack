using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Maxfire.Core;
using Maxfire.Core.Extensions;
using Maxfire.Core.Reflection;

namespace Maxfire.Web.Mvc
{
	public static class OptionsAdapter
	{
		public static IEnumerable<TextValuePair> FromEnumeration<TEnumeration>()
			where TEnumeration : Enumeration
		{
			return FromEnumeration(Enumeration.GetAll<TEnumeration>());
		}

		public static IEnumerable<TextValuePair> FromEnumeration<TEnumeration>(params TEnumeration[] items)
			where TEnumeration : Enumeration
		{
			return new OptionsAdapter<TEnumeration>(items, item => item.Text, item => item.Name);
		}

		public static IEnumerable<TextValuePair> FromEnumeration<TEnumeration>(IEnumerable<TEnumeration> items)
			where TEnumeration : Enumeration
		{
			return new OptionsAdapter<TEnumeration>(items, item => item.Text, item => item.Name);
		}

		public static IEnumerable<TextValuePair> FromEnumValues(Type enumType)
		{
			return FromEnumHelper(enumType, item => Convert.ToInt32(item).ToString());
		}

		public static IEnumerable<TextValuePair> FromEnumValues<TEnum>()
		{
			return FromEnumHelper<TEnum>(item => Convert.ToInt32(item).ToString());
		}

		public static IEnumerable<TextValuePair> FromEnumTexts(Type enumType)
		{
			return FromEnumHelper(enumType, item => item.ToString());
		}

		public static IEnumerable<TextValuePair> FromEnumTexts<TEnum>(Func<TEnum, bool> predicate = null)
		{
			return FromEnumHelper<TEnum>(item => item.ToString(), predicate);
		}

		private static IEnumerable<TextValuePair> FromEnumHelper<TEnum>(Func<TEnum, string> valueSelector, Func<TEnum, bool> predicate = null)
		{
			var enumType = typeof (TEnum);
			if (!enumType.IsEnum)
			{
				throw new ArgumentException("The generic type argument must be an enum.");
			}
			var values = Enum.GetValues(enumType).Cast<TEnum>();
			return new OptionsAdapter<TEnum>(values, item => item.GetDisplayNameOfEnum(), valueSelector, predicate);
		}

		private static IEnumerable<TextValuePair> FromEnumHelper(Type enumType, Func<object, string> valueSelector)
		{
			if (!enumType.IsEnum)
			{
				throw new ArgumentException("The generic type argument must be an enum.");
			}
			var values = Enum.GetValues(enumType).Cast<object>();
			return new OptionsAdapter<object>(values, item => item.GetDisplayNameOfEnum(enumType), valueSelector);
		}

		public static IEnumerable<TextValuePair> FromCollection<T>(IEnumerable<T> items, Func<T, string> textSelector, Func<T, object > valueSelector)
		{
			return new OptionsAdapter<T>(items, textSelector, item => valueSelector(item).ToNullSafeString(CultureInfo.CurrentCulture));
		}

		public static IEnumerable<TextValuePair> FromCollection<T>(IEnumerable<T> items, Func<T, string> textSelector, Func<T, string> valueSelector)
		{
			return new OptionsAdapter<T>(items, textSelector, valueSelector);
		}

		public static IEnumerable<TextValuePair> FromCollection<T>(IEnumerable<T> items)
		{
			return new OptionsAdapter<T>(items, item => item.ToNullSafeString(CultureInfo.CurrentCulture), item => item.ToNullSafeString(CultureInfo.CurrentCulture));
		}

		public static IEnumerable<TextValuePair> FromSelectListItems(IEnumerable<SelectListItem> items)
		{
			return new OptionsAdapter<SelectListItem>(items, item => item.Text, item => item.Value);
		}

		public static IEnumerable<TextValuePair> FromDictionary<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> items)
		{
			return new OptionsAdapter<KeyValuePair<TKey, TValue>>(items, 
				kvp => kvp.Key.ToNullSafeString(CultureInfo.CurrentCulture), 
				kvp => kvp.Value.ToNullSafeString(CultureInfo.CurrentCulture));
		}

		public static IEnumerable<TextValuePair> FromDictionary(object items)
		{
			return FromDictionary(items.ToPropertyStringValuePairs());
		}

		public static IEnumerable<TextValuePair> FromDictionary(params Func<string, string>[] items)
		{
			return new OptionsAdapter<Func<string, string>>(items, 
				func => func.Method.GetParameters()[0].Name, 
				func => func(null));
		}

		public static IEnumerable<TextValuePair> Boolean()
		{
			yield return new TextValuePair("Ja", "true");
			yield return new TextValuePair("Nej", "false");
		}

		public static IEnumerable<TextValuePair> Months()
		{
			yield return new TextValuePair("Januar", "1");
			yield return new TextValuePair("Februar", "2");
			yield return new TextValuePair("Marts", "3");
			yield return new TextValuePair("April", "4");
			yield return new TextValuePair("Maj", "5");
			yield return new TextValuePair("Juni", "6");
			yield return new TextValuePair("Juli", "7");
			yield return new TextValuePair("August", "8");
			yield return new TextValuePair("September", "9");
			yield return new TextValuePair("Oktober", "10");
			yield return new TextValuePair("November", "11");
			yield return new TextValuePair("December", "12");
		}

		public static IEnumerable<TextValuePair> WithFirstOptionText(this IEnumerable<TextValuePair> options, string firstOptionText)
		{
			return firstOptionTextIterator(firstOptionText).Concat(options);

		}

		private static IEnumerable<TextValuePair> firstOptionTextIterator(string firstOptionText)
		{
			yield return new TextValuePair(firstOptionText ?? string.Empty, string.Empty);
		}
	}

	public class OptionsAdapter<TItem> : IEnumerable<TextValuePair>
	{
		private readonly IEnumerable<TItem> _items;
		private readonly Func<TItem, string> _textSelector;
		private readonly Func<TItem, string> _valueSelector;
		private readonly Func<TItem, bool> _predicate;

		public OptionsAdapter(IEnumerable<TItem> items, Func<TItem, string> textSelector, Func<TItem, string> valueSelector, Func<TItem, bool> predicate = null)
		{
			if (textSelector == null) throw new ArgumentNullException("textSelector");
			if (valueSelector == null) throw new ArgumentNullException("valueSelector");
			_items = items;
			_textSelector = textSelector;
			_valueSelector = valueSelector;
			_predicate = predicate;
		}

		public IEnumerator<TextValuePair> GetEnumerator()
		{
			return GetTextValuePairs().GetEnumerator();
		}

		public IEnumerable<TextValuePair> GetTextValuePairs()
		{
			if (_items == null)
			{
				return Enumerable.Empty<TextValuePair>();
			}
			IEnumerable<TextValuePair> textValuePairs = _predicate != null ? 
				_items
					.Where(_predicate)
					.Select(item => new TextValuePair(_textSelector(item), _valueSelector(item))) :
				_items
					.Select(item => new TextValuePair(_textSelector(item), _valueSelector(item)));
			return textValuePairs;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}