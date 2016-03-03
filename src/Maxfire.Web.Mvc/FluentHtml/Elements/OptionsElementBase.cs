using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Maxfire.Core;

namespace Maxfire.Web.Mvc.FluentHtml.Elements
{
	public abstract class OptionsElementBase<T> : FormElement<T> where T : OptionsElementBase<T>
	{
		protected OptionsElementBase(string tag, string name, MemberExpression forMember)
			: base(tag, name, forMember) { }

		protected IEnumerable<ITextValuePair> _options;
		protected IEnumerable<string> _selectedValues;

		public IEnumerable<string> SelectedValues
		{
			get { return _selectedValues; }
		}

		public IEnumerable<ITextValuePair> Options
		{
			get { return _options; }
		}

		public T WithOptions(IEnumerable<ITextValuePair> options)
		{
			_options = options;
			return (T) this;
		}

		protected bool IsSelectedValue(string value)
		{
			if (_selectedValues != null)
			{
				return _selectedValues.Any(val => val == value);
			}
			return false;
		}
	}
}