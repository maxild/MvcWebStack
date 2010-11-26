using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Maxfire.Core;

namespace Maxfire.Web.Mvc.Html5.Mixins
{
	public interface ISupportsOptions<out T>
	{
		IEnumerable<SelectListItem> Options();
		T Options(IEnumerable<SelectListItem> options);
		T Options(IEnumerable<ITextValuePair> options);
		IEnumerable<object> SelectedValues();
		T SelectedValues(IEnumerable selectedValues);
		bool IsSelectedValue(string value);
	}

	public class OptionsMixin<T> : Mixin<T>, ISupportsOptions<T> where T : OptionsMixin<T>
	{
		// LINQ Cast<object> has super duper perf on .NET 4 due to contra-variance (only 
		// ArrayList or old-time IEnumerable impl need iterator to do the safe cast).
#pragma warning disable 649
		private IEnumerable _options;
		private Func<object, string> _textSelector;
		private Func<object, string> _valueSelector;
		private IEnumerable _selectedValues;
#pragma warning restore 649

		public IEnumerable<SelectListItem> Options()
		{
			if (_options == null)
			{
				return Enumerable.Empty<SelectListItem>();
			}
			return from option in _options.Cast<object>()
			       let value = _valueSelector(option)
			       let text = _textSelector(option)
			       select new SelectListItem { Text = text, Value = value, Selected = IsSelectedValue(value ?? text) };
		}

		// TODO: Move OptionsAdapter logic here somehow
		// Q: What core type should GetOptionsFor/SetOptionsFor use? IEnumerable<TextValuePair>, because SelectedValues cannot be defined
		// Q: What core type should GetOptionsFor/SetOptionsFor use? IEnumerable<SelectListItem>, because SelectedValues can be defined

		public T Options(IEnumerable<SelectListItem> options)
		{
			// TODO
			return self;
		}

		public T Options(IEnumerable<ITextValuePair> options)
		{
			// TODO
			return self;
		}

		public IEnumerable<object> SelectedValues()
		{
			return _selectedValues != null ? _selectedValues.Cast<object>() : Enumerable.Empty<object>();
		}

		public T SelectedValues(IEnumerable selectedValues)
		{
			_selectedValues = selectedValues;
			return self;
		}

		public bool IsSelectedValue(string value)
		{
			return SelectedValues().Any(selectedValue => value.Equals(Convert.ToString(selectedValue, CultureInfo.InvariantCulture)));
		}
	}
}