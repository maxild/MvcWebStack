using System;
using System.Collections.Generic;
using System.Linq;
using Maxfire.Core;
using Maxfire.Core.Extensions;
using Maxfire.Core.Reflection;

namespace Maxfire.Web.Mvc
{
	public static class OptionsAdapter
	{
		public static IEnumerable<ITextValuePair> FromEnumeration<T>()
			where T : Enumeration
		{
			return FromEnumeration(Enumeration.GetAll<T>());
		}

		public static IEnumerable<ITextValuePair> FromEnumeration<T>(params T[] options)
			where T : Enumeration
		{
			return new OptionsTextValueAdapter<T>(options, x => x.Text, x => x.Name);
		}

		public static IEnumerable<ITextValuePair> FromEnumeration<T>(IEnumerable<T> options)
			where T : Enumeration
		{
			return new OptionsTextValueAdapter<T>(options, x => x.Text, x => x.Name);
		}

		public static IEnumerable<ITextValuePair> FromEnum<TEnum>()
		{
			Type enumType = typeof (TEnum);
			if (!enumType.IsEnum)
			{
				throw new ArgumentException("The generic type argument must be an enum.");
			}
			var values = (TEnum[]) Enum.GetValues(enumType);
			return new OptionsTextValueAdapter<TEnum>(values, x => x.GetDisplayNameOfEnum(), x => Convert.ToInt32(x).ToString());
		}

		public static IEnumerable<ITextValuePair> FromCollection<T>(IEnumerable<T> options, Func<T, string> textSelector, Func<T, string> valueSelector)
		{
			textSelector.ThrowIfNull("textSelector");
			valueSelector.ThrowIfNull("valueSelector");

			return new OptionsTextValueAdapter<T>(options, textSelector, valueSelector);
		}

		public static IEnumerable<ITextValuePair> FromCollection<T>(IEnumerable<T> options)
		{
			return new OptionsTextValueAdapter<T>(options, x => x.ToString(), x => x.ToString());
		}

		public static IEnumerable<ITextValuePair> FromDictionary<TKey, TValue>(IDictionary<TKey, TValue> options)
		{
			// Option.Text = kvp.Value, Option.Value = kvp.Key
			return new OptionsTextValueAdapter<KeyValuePair<TKey, TValue>>(options, 
			                                                               kvp => kvp.Value.ToString(),
			                                                               kvp => kvp.Key.ToString());
		}

		public static IEnumerable<ITextValuePair> Boolean()
		{
			yield return new TextValuePair("Nej", "false");
			yield return new TextValuePair("Ja", "true");
		}

		public static IEnumerable<ITextValuePair> WithFirstOptionText(this IEnumerable<ITextValuePair> options, string firstOptionText)
		{
			return firstOptionTextIterator(firstOptionText).Concat(options);

		}

		private static IEnumerable<ITextValuePair> firstOptionTextIterator(string firstOptionText)
		{
			yield return new TextValuePair(firstOptionText ?? string.Empty, string.Empty);
		}
	}
}