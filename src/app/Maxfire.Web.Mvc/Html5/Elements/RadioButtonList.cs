using System.Linq;
using Maxfire.Web.Mvc.Html5.HtmlTokens;

namespace Maxfire.Web.Mvc.Html5.Elements
{
	public class RadioButtonList : OptionsInputElementList<RadioButtonList>
	{
		public RadioButtonList(string name, IModelMetadataAccessor accessor) 
			: base(HtmlInputType.Radio, name, accessor)
		{
			if (accessor != null)
			{
				SetSelectedValue(accessor.GetModelMetadata(name).Model);
			}
		}

		public object SelectedValue()
		{
			return Selected().FirstOrDefault();
		}

		public RadioButtonList SelectedValue(object value)
		{
			BindExplicitValue(value);
			return self;
		}

		private void SetSelectedValue(object value)
		{
			Selected(value != null ? new[] { value } : null);
		}

		protected override void BindValue(object value)
		{
			SetSelectedValue(value);
		}
	}
}