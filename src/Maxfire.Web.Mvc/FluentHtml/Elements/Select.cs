using System.Linq.Expressions;
using Maxfire.Core.Extensions;

namespace Maxfire.Web.Mvc.FluentHtml.Elements
{
	/// <summary>
	/// Generate and HTML select element.
	/// </summary>
	public class Select : SelectBase<Select>
	{
		/// <summary>
		/// Generate an HTML select element.
		/// </summary>
		/// <param name="name">Value of the 'name' attribute of the element.  Also used to derive the 'id' attribute.</param>
		/// <param name="forMember">Expression indicating the model member assocaited with the element.</param>
		public Select(string name, MemberExpression forMember)
			: base(name, forMember)
		{
		}


		/// <summary>
		/// Set the selected option.
		/// </summary>
		/// <param name="selectedValue">A value matching the option to be selected.</param>
		/// <returns></returns>
		public virtual Select Selected(object selectedValue)
		{
			SelectedValues = new [] { selectedValue.ToNullSafeString() };
			return this;
		}
	}
}
