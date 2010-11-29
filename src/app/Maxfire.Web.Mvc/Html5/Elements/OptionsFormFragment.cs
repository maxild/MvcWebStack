using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Maxfire.Core.Extensions;
using Maxfire.Core.Reflection;

namespace Maxfire.Web.Mvc.Html5.Elements
{
	// TODOs:
	// Q: What core type should GetOptionsFor/SetOptionsFor use? IEnumerable<TextValuePair>, because SelectedValues cannot be defined
	// Q: What core type should GetOptionsFor/SetOptionsFor use? IEnumerable<SelectListItem>, because SelectedValues can be defined

	public abstract class OptionsFormFragment<T> : FormFragment<T> where T : OptionsFormFragment<T>
	{
		private IEnumerable<SelectListItem> _options;
		private IEnumerable _selectedValues;
		
		protected OptionsFormFragment(string tagName, string name, IModelMetadataAccessor accessor)
			: base(tagName, name, accessor)
		{
		}

		/// <summary>
		/// Get access to the methods used to configure the options (name, text, selected values).
		/// </summary>
		public OptionsBuilder<T> Options
		{
			get { return new OptionsBuilder<T>(self); }
		}

		protected IEnumerable<SelectListItem> GetOptions()
		{
			return _options ?? Enumerable.Empty<SelectListItem>();
		}

		protected T SetOptions(IEnumerable<SelectListItem> options)
		{
			// TODO: Hvad med selected values?
			// TODO: Sondring mellem når Builder kalder og klient kalder
			_options = options;
			return self;
		}

		protected IEnumerable<object> Selected()
		{
			return _selectedValues != null ? _selectedValues.Cast<object>() : Enumerable.Empty<object>();
		}

		protected T Selected(IEnumerable selectedValues)
		{
			_selectedValues = selectedValues;
			return self;
		}

		public class OptionsBuilder<TOptionsFormFragment> where TOptionsFormFragment : OptionsFormFragment<TOptionsFormFragment>
		{
			private readonly TOptionsFormFragment _optionsFormFragment;

			public OptionsBuilder(TOptionsFormFragment optionsFormFragment)
			{
				_optionsFormFragment = optionsFormFragment;
			}

			public TOptionsFormFragment FromEnumValues(Type enumType)
			{
				return FromEnumHelper(enumType, x => Convert.ToInt32(x).ToString());
			}

			public TOptionsFormFragment FromEnumValues<TEnum>()
			{
				return FromEnumHelper<TEnum>(x => Convert.ToInt32(x).ToString());
			}

			public TOptionsFormFragment FromEnumTexts(Type enumType)
			{
				return FromEnumHelper(enumType, x => x.ToString());
			}

			public TOptionsFormFragment FromEnumTexts<TEnum>()
			{
				return FromEnumHelper<TEnum>(x => x.ToString());
			}

			private TOptionsFormFragment FromEnumHelper<TEnum>(Func<TEnum, string> valueSelector)
			{
				var enumType = typeof(TEnum);
				if (!enumType.IsEnum)
				{
					throw new ArgumentException("The generic type argument must be an enum.");
				}
				var values = Enum.GetValues(enumType).Cast<TEnum>();
				return ToOptions(values, x => x.GetDisplayNameOfEnum(), valueSelector);
			}

			private TOptionsFormFragment FromEnumHelper(Type enumType, Func<object, string> valueSelector)
			{
				if (!enumType.IsEnum)
				{
					throw new ArgumentException("The generic type argument must be an enum.");
				}
				var values = Enum.GetValues(enumType).Cast<object>();
				return ToOptions(values, x => x.GetDisplayNameOfEnum(enumType), valueSelector);
			}

			public TOptionsFormFragment FromCollection<TItem>(IEnumerable<TItem> options, Func<TItem, string> textSelector, Func<TItem, object> valueSelector)
			{
				return ToOptions(options, textSelector, option => valueSelector(option).ToString());
			}

			public TOptionsFormFragment FromCollection<TItem>(IEnumerable<TItem> options, Func<TItem, string> textSelector, Func<TItem, string> valueSelector)
			{
				return ToOptions(options, textSelector, valueSelector);
			}

			public TOptionsFormFragment FromCollection<TItem>(IEnumerable<TItem> options)
			{
				return ToOptions(options, x => x.ToString(), x => x.ToString());
			}

			public TOptionsFormFragment FromSelectListItems(IEnumerable<SelectListItem> options)
			{
				return ToOptions(options);
			}

			public TOptionsFormFragment FromTextValuePairs<TItem>(IEnumerable<TItem> options)
			{
				return ToOptions(new SelectList(options, "Value", "Text"));
			}

			public TOptionsFormFragment FromDictionary<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> options)
			{
				return ToOptions(options, kvp => kvp.Key.ToString(), kvp => kvp.Value.ToString());
			}

			public TOptionsFormFragment FromDictionary(object options)
			{
				return FromDictionary(options.ToPropertyStringValuePairs());
			}

			public TOptionsFormFragment FromDictionary(params Func<string, string>[] options)
			{
				return ToOptions(options, func => func.Method.GetParameters()[0].Name, func => func(null));
			}

			public TOptionsFormFragment Boolean()
			{
				return ToOptions(boolean());
			}

			private static IEnumerable<SelectListItem> boolean()
			{
				yield return new SelectListItem { Text = "Ja", Value = "true" };
				yield return new SelectListItem { Text = "Nej", Value = "false" };
			}

			public TOptionsFormFragment Months()
			{
				return ToOptions(months());
			}
 
			private static IEnumerable<SelectListItem> months()
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

			private TOptionsFormFragment ToOptions<TItem>(IEnumerable<TItem> options, Func<TItem, string> textSelector, Func<TItem, string> valueSelector)
			{
				return _optionsFormFragment.SetOptions(new OptionsAdapter<TItem>(options, textSelector, valueSelector,
																	   () => _optionsFormFragment.Selected()));
			}

			private TOptionsFormFragment ToOptions(IEnumerable<SelectListItem> options)
			{
				// TODO: Hvad med IsSelected???
				return _optionsFormFragment.SetOptions(options);
			}
		}

		public class OptionsAdapter<TItem> : IEnumerable<SelectListItem>
		{
			private readonly IEnumerable<TItem> _options;
			private readonly Func<IEnumerable> _selectedValuesThunk;
			private readonly Func<TItem, string> _textSelector;
			private readonly Func<TItem, string> _valueSelector;

			public OptionsAdapter(IEnumerable<TItem> options, Func<TItem, string> textSelector, Func<TItem, string> valueSelector, Func<IEnumerable> selectedValuesThunk)
			{
				if (textSelector == null) throw new ArgumentNullException("textSelector");
				if (valueSelector == null) throw new ArgumentNullException("valueSelector");
				_options = options;
				_textSelector = textSelector;
				_valueSelector = valueSelector;
				_selectedValuesThunk = selectedValuesThunk;
			}

			public IEnumerator<SelectListItem> GetEnumerator()
			{
				if (_options == null)
				{
					return Enumerable.Empty<SelectListItem>().GetEnumerator();
				}
				IEnumerable<object> selectedValues = GetSelectedValues();
				var listItems = from option in _options
								let valueText = _valueSelector(option)
								select new SelectListItem
								{
									Value = valueText,
									Text = _textSelector(option),
									Selected = IsSelectedValue(selectedValues, valueText)
								};
				return listItems.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			IEnumerable<object> GetSelectedValues()
			{
				if (_selectedValuesThunk == null)
				{
					return Enumerable.Empty<object>();
				}
				IEnumerable selectedValues = _selectedValuesThunk();
				return selectedValues == null ? Enumerable.Empty<object>() : selectedValues.Cast<object>();
			}

			static bool IsSelectedValue(IEnumerable<object> selectedValues, string value)
			{
				return selectedValues.Any(selectedValue => value.Equals(Convert.ToString(selectedValue, CultureInfo.InvariantCulture), StringComparison.Ordinal));
			}
		}
	}
}