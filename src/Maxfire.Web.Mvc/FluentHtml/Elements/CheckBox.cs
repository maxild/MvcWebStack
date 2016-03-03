using System.Linq.Expressions;
using Maxfire.Web.Mvc.FluentHtml.Html;

namespace Maxfire.Web.Mvc.FluentHtml.Elements
{
	/// <summary>
	/// Generate an HTML input element of type 'checkbox.'
	/// </summary>
	public class CheckBox : Input<CheckBox>
	{
		/// <summary>
		/// Generate an HTML input element of type 'checkbox.'
		/// </summary>
		/// <param name="name">Value of the 'name' attribute of the element.  Also used to derive the 'id' attribute.</param>
		/// <param name="forMember">Expression indicating the view model member assocaited with the element.</param>
		public CheckBox(string name, MemberExpression forMember) :
			base(HtmlInputType.Checkbox, name, forMember)
		{
		}

		/// <summary>
		/// Set the checked attribute.
		/// </summary>
		/// <param name="value">Whether the checkbox should be checked.</param>
		public virtual CheckBox Checked(bool value)
		{
			if (value)
			{
				SetAttr(HtmlAttribute.Checked, HtmlAttribute.Checked);
			}
			else
			{
				RemoveAttr(HtmlAttribute.Checked);
			}
			return this;
		}

		public override string ToString()
		{
			var html = ToCheckBoxOnlyHtml();
			var hiddenId = "_Hidden";
			if (HasId())
			{
				hiddenId = GetId() + hiddenId;
			}
			var hidden = new Hidden(GetName()).Id(hiddenId).Value("false").ToString();
			return string.Concat(html, hidden);
		}

		public string ToCheckBoxOnlyHtml()
		{
			return base.ToString();
		}

		protected override void ApplyModelState(System.Web.Mvc.ModelState state)
		{
			var isChecked = state.Value.ConvertTo(typeof(bool?)) as bool?;

			if (isChecked.HasValue)
			{
				Checked(isChecked.Value);
			}
		}
	}
}