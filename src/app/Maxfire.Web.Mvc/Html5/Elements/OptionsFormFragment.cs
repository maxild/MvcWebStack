using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Maxfire.Core;
using Maxfire.Core.Extensions;
using Maxfire.Core.Reflection;
using OptionsAdapterUtility = Maxfire.Web.Mvc.OptionsAdapter;

namespace Maxfire.Web.Mvc.Html5.Elements
{
	public abstract class OptionsFormFragment<T> : FormFragment<T> where T : OptionsFormFragment<T>
	{
		private IEnumerable<SelectListItem> _options;
		private IEnumerable _selectedValues;
		
		protected OptionsFormFragment(string elementName, string name, IModelMetadataAccessor accessor)
			: base(elementName, name, accessor)
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
				return FromEnumHelper(enumType, item => Convert.ToInt32(item).ToString());
			}

			public TOptionsFormFragment FromEnumValues<TEnum>()
			{
				return FromEnumHelper<TEnum>(item => Convert.ToInt32(item).ToString());
			}

			public TOptionsFormFragment FromEnumTexts(Type enumType)
			{
				return FromEnumHelper(enumType, item => item.ToString());
			}

			public TOptionsFormFragment FromEnumTexts<TEnum>()
			{
				return FromEnumHelper<TEnum>(item => item.ToString());
			}

			private TOptionsFormFragment FromEnumHelper<TEnum>(Func<TEnum, string> valueSelector)
			{
				var enumType = typeof(TEnum);
				if (!enumType.IsEnum)
				{
					throw new ArgumentException("The generic type argument must be an enum.");
				}
				var values = Enum.GetValues(enumType).Cast<TEnum>();
				return ToOptions(values, item => item.GetDisplayNameOfEnum(), valueSelector);
			}

			private TOptionsFormFragment FromEnumHelper(Type enumType, Func<object, string> valueSelector)
			{
				if (!enumType.IsEnum)
				{
					throw new ArgumentException("The generic type argument must be an enum.");
				}
				var values = Enum.GetValues(enumType).Cast<object>();
				return ToOptions(values, item => item.GetDisplayNameOfEnum(enumType), valueSelector);
			}

			public TOptionsFormFragment FromCollection<TItem>(IEnumerable<TItem> items, Func<TItem, string> textSelector, Func<TItem, object> valueSelector)
			{
				return ToOptions(items, textSelector, item => valueSelector(item).ToNullSafeString(CultureInfo.CurrentCulture));
			}

			public TOptionsFormFragment FromCollection<TItem>(IEnumerable<TItem> items, Func<TItem, string> textSelector, Func<TItem, string> valueSelector)
			{
				return ToOptions(items, textSelector, valueSelector);
			}

			public TOptionsFormFragment FromCollection<TItem>(IEnumerable<TItem> items)
			{
				return ToOptions(items, item => item.ToNullSafeString(CultureInfo.CurrentCulture), item => item.ToNullSafeString(CultureInfo.CurrentCulture));
			}

			public TOptionsFormFragment FromSelectListItems(IEnumerable<SelectListItem> items)
			{
				return ToOptions(items);
			}

			public TOptionsFormFragment FromTextValuePairs(IEnumerable<TextValuePair> items)
			{
				return ToOptions(items, item => item.Text, item => item.Value);
			}

			public TOptionsFormFragment FromDictionary<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> items)
			{
				return ToOptions(items, kvp => kvp.Key.ToNullSafeString(CultureInfo.CurrentCulture), kvp => kvp.Value.ToNullSafeString(CultureInfo.CurrentCulture));
			}

			public TOptionsFormFragment FromDictionary(object items)
			{
				return FromDictionary(items.ToPropertyStringValuePairs());
			}

			public TOptionsFormFragment FromDictionary(params Func<string, string>[] items)
			{
				return ToOptions(items, func => func.Method.GetParameters()[0].Name, func => func(null));
			}

			public TOptionsFormFragment Boolean()
			{
				return ToOptions(OptionsAdapterUtility.Boolean(), item => item.Text, item => item.Value);
			}

			public TOptionsFormFragment Months()
			{
				return ToOptions(OptionsAdapterUtility.Months(), item => item.Text, item => item.Value);
			}
 
			private TOptionsFormFragment ToOptions<TItem>(IEnumerable<TItem> items, Func<TItem, string> textSelector, Func<TItem, string> valueSelector)
			{
				return _optionsFormFragment.SetOptions(new SelectListItemsAdapter<TItem>(items, textSelector, valueSelector,
																	   () => _optionsFormFragment.Selected()));
			}

			private TOptionsFormFragment ToOptions(IEnumerable<SelectListItem> items)
			{
				if (items != null)
				{
					HashSet<object> selectedValues = new HashSet<object>(_optionsFormFragment.Selected());
					selectedValues.UnionWith(items.Where(item => item.Selected).Map(item => item.Value));
					_optionsFormFragment.Selected(selectedValues);
				}
				return _optionsFormFragment.SetOptions(items);
			}
		}

		/// <summary>
		/// Create a list of text-value-selected tuples that clients can use to render
		/// select, radiobuttonlist and/or checkboxlist widgets.
		/// </summary>
		sealed class SelectListItemsAdapter<TItem> : IEnumerable<SelectListItem>
		{
			private readonly IEnumerable<TItem> _items;
			private readonly Func<IEnumerable> _selectedValuesThunk;
			private readonly Func<TItem, string> _textSelector;
			private readonly Func<TItem, string> _valueSelector;

			public SelectListItemsAdapter(IEnumerable<TItem> items, Func<TItem, string> textSelector, Func<TItem, string> valueSelector, Func<IEnumerable> selectedValuesThunk)
			{
				if (textSelector == null)
				{
					throw new ArgumentNullException("textSelector");
				}
				_items = items;
				_textSelector = textSelector;
				_valueSelector = valueSelector;
				_selectedValuesThunk = selectedValuesThunk;
			}

			public IEnumerator<SelectListItem> GetEnumerator()
			{
				return getListItems().GetEnumerator();
			}

			private IEnumerable<SelectListItem> getListItems()
			{
				if (_items == null)
				{
					return Enumerable.Empty<SelectListItem>();
				}
				
				ISet<string> selectedValues = getSelectedValues();

				return _valueSelector != null ? 
					getListItemsWithValueField(selectedValues) :
					getListItemsWithoutValueField(selectedValues);
			}

			ISet<string> getSelectedValues()
			{
				var selectedValueSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
				if (_selectedValuesThunk != null)
				{
					IEnumerable selectedValues = _selectedValuesThunk();
					if (selectedValues != null)
					{
						selectedValueSet.UnionWith(
							from object value in selectedValues 
							select TypeExtensions.ConvertSimpleType(CultureInfo.InvariantCulture, value, typeof(string)).ToNullSafeString()
						);
					}
				}
				return selectedValueSet;
			}

			private IEnumerable<SelectListItem> getListItemsWithoutValueField(ISet<string> selectedValueSet)
			{
				var listItems = from item in _items
				                let text = _textSelector(item)
				                select new SelectListItem
				                {
				                    Text = text,
				                    Selected = selectedValueSet.Contains(text) // HTML spec says text is the value, if value is not present
				                };
				return listItems.ToList();
			}

			private IEnumerable<SelectListItem> getListItemsWithValueField(ISet<string> selectedValues)
			{
				var listItems = from item in _items
				                let value = _valueSelector(item)
				                select new SelectListItem
				                {
				                    Value = value,
				                    Text = _textSelector(item),
				                    Selected = selectedValues.Contains(value)
				                };
				return listItems;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}
	}
}