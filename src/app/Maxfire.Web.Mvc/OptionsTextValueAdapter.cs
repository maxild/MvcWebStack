using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Maxfire.Core;

namespace Maxfire.Web.Mvc
{
	public class OptionsTextValueAdapter<T> : IEnumerable<ITextValuePair>
	{
		private readonly IEnumerable<T> _options;
		private readonly Func<T, string> _textSelector;
		private readonly Func<T, string> _valueSelector;

		public OptionsTextValueAdapter(IEnumerable<T> options, Func<T, string> textSelector, Func<T, string> valueSelector)
		{
			_options = options;
			_textSelector = textSelector;
			_valueSelector = valueSelector;
		}

		public IEnumerator<ITextValuePair> GetEnumerator()
		{
			if (_options == null)
			{
				return Enumerable.Empty<ITextValuePair>().GetEnumerator();
			}

			return _options
				.Select(option => (ITextValuePair)new TextValuePair(_textSelector(option), _valueSelector(option)))
				.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}