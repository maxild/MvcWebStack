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
                SetSelectedValue(accessor.GetModelValueAsString(name));
            }
        }

        public object SelectedValue()
        {
            return Selected().FirstOrDefault();
        }

        public RadioButtonList SelectedValue(object value)
        {
            BindExplicitValue(value); // we cannot call SetSelectedValue, because we need explicit value flag to be set
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
