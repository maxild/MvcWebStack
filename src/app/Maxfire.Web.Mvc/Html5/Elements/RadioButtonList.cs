using System.Linq;
using Maxfire.Web.Mvc.Html5.HtmlTokens;

namespace Maxfire.Web.Mvc.Html5.Elements
{
	public class RadioButtonList : OptionsInputElementList<RadioButtonList>
	{
		public RadioButtonList(string name, IModelMetadataAccessor accessor) 
			: base(HtmlInputType.Radio, name, accessor)
		{
		}

		public object SelectedValue()
		{
			return Selected().FirstOrDefault();
		}

		public RadioButtonList SelectedValue(object value)
		{
			Selected(value != null ? new[] { value } : null);
			return self;
		}

		protected override void BindValue(object value)
		{
			SelectedValue(value);
		}
	}
}