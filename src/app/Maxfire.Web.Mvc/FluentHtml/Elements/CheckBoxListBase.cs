using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using Maxfire.Web.Mvc.FluentHtml.Extensions;
using Maxfire.Web.Mvc.FluentHtml.Html;
using Maxfire.Web.Mvc.Html;

namespace Maxfire.Web.Mvc.FluentHtml.Elements
{
	/// <summary>
	/// Base class for a list of checkboxes.
	/// </summary>
	public abstract class CheckBoxListBase<T> : OptionsElementBase<T> where T : CheckBoxListBase<T>
	{
		private string _itemFormat;
		private string _itemClass;

		protected CheckBoxListBase(string tag, string name, MemberExpression forMember)
			: base(tag, name, forMember)
		{
		}

		/// <summary>
		/// Set the selected values.
		/// </summary>
		/// <param name="selectedValues">Values matching the values of options to be selected.</param>
		public virtual T Selected(IEnumerable<string> selectedValues)
		{
			_selectedValues = selectedValues;
			return (T)this;
		}

		/// <summary>
		/// Specify a format string for the HTML of each checkbox button and label.
		/// </summary>
		/// <param name="value">A format string.</param>
		public virtual T ItemFormat(string value)
		{
			_itemFormat = value;
			return (T)this;
		}

		/// <summary>
		/// Specify the class for the input and label elements of each item.
		/// </summary>
		/// <param name="value">A format string.</param>
		public virtual T ItemClass(string value)
		{
			_itemClass = value;
			return (T)this;
		}

		protected override void PreRender()
		{
			SetInnerHtml(renderBody());
			base.PreRender();
		}

		protected override TagRenderMode TagRenderMode
		{
			get { return TagRenderMode.Normal; }
		}

		private string renderBody()
		{
			if (_options == null)
			{
				return null;
			}

			string idPrefix = Html401IdUtil.CreateSanitizedId(GetName());

			RemoveAttr(HtmlAttribute.Name);
			var sb = new StringBuilder();
			var i = 0;
			foreach (var option in _options)
			{
				var value = option.Value;
				var checkbox = new CheckBox(GetName(), ForMember)
					.ApplyBehaviors(AppliedBehaviors)
					.Id(string.Format("{0}_{1}", idPrefix, i))
					.Value(value)
					.LabelAfter(option.Text, _itemClass)
					.Checked(IsSelectedValue(value));
				if (_itemClass != null)
				{
					checkbox.Class(_itemClass);
				}
				sb.Append(_itemFormat == null
				          	? checkbox.ToCheckBoxOnlyHtml()
				          	: string.Format(_itemFormat, checkbox.ToCheckBoxOnlyHtml()));
				i += 1;
			}
			return sb.ToString();
		}
	}
}