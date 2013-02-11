using System.Linq;
using System.Text;
using Maxfire.Core.Extensions;
using Maxfire.Web.Mvc.Html5.HtmlTokens;

namespace Maxfire.Web.Mvc.Html5.Elements
{
	public class Select : OptionsFormElement<Select>
	{
		public Select(string name, IModelMetadataAccessor accessor) 
			: base(HtmlElement.Select, name, accessor)
		{
			if (accessor != null)
			{
				//SetSelectedValue(accessor.GetModelMetadata(name).Model);
				SetSelectedValue(accessor.GetModelValueAsString(name));
			}
		}

		public object SelectedValue()
		{
			return Selected().FirstOrDefault();
		}

		public Select SelectedValue(object value)
		{
			BindExplicitValue(value);
			return self;
		}

		private void SetSelectedValue(object value)
		{
			Selected(value != null ? new[] { value } : null);
		}

		protected override string RenderOptions()
		{
			return GetOptions()
				.Map(item => new Option().Value(item.Value).ToggleAttr(HtmlAttribute.Selected, item.Selected).InnerText(item.Text))
				.Aggregate(new StringBuilder(), (sb, option) => sb.Append(option.ToHtmlString()))
				.ToString();
		}

		protected override void BindValue(object value)
		{
			SetSelectedValue(value);
		}
	}
}