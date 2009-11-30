using System.Collections.Generic;
using System.Linq.Expressions;
using Maxfire.Web.Mvc.FluentHtml.Html;

namespace Maxfire.Web.Mvc.FluentHtml.Elements
{
	/// <summary>
	/// Generate a select HTML element with multiselect attribute = 'true.'
	/// </summary>
	public class MultiSelect : SelectBase<MultiSelect>
	{
		/// <summary>
		/// Generate a select HTML element with multiselect attribute = 'true'
		/// </summary>
		/// <param name="name">Value of the 'name' attribute of the element.  Also used to derive the 'id' attribute.</param>
		/// <param name="forMember">Expression indicating the view model member assocaited with the element</param>
		public MultiSelect(string name, MemberExpression forMember)
			: base(name, forMember)
		{
			SetAttr(HtmlAttribute.Multiple, HtmlAttribute.Multiple);
		}

		/// <summary>
		/// Set the selected values.
		/// </summary>
		/// <param name="selectedValues">Values matching the values of options to be selected.</param>
		public virtual MultiSelect Selected(IEnumerable<string> selectedValues)
		{
			_selectedValues = selectedValues;
			return this;
		}
	}
}