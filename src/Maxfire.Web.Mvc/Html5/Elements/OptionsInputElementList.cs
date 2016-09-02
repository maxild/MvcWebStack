using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Maxfire.Core.Extensions;
using Maxfire.Web.Mvc.Html;
using Maxfire.Web.Mvc.Html5.HtmlTokens;

namespace Maxfire.Web.Mvc.Html5.Elements
{
    public abstract class OptionsInputElementList<T> : OptionsFormFragment<T> where T : OptionsInputElementList<T>
    {
        protected OptionsInputElementList(string type, string name, IModelMetadataAccessor accessor)
            : base(HtmlElement.Input, name, accessor)
        {
            Attr(HtmlAttribute.Type, type);
        }

        private string _wrapFormat;
        public T Wrap(string s)
        {
            // s could be '<p>{0}</p>'
            _wrapFormat = s;
            return self;
        }

        private IEnumerable<KeyValuePair<string, object>> _labelAttr;
        public T LabelAttr(IEnumerable<KeyValuePair<string, object>> attributes)
        {
            _labelAttr = attributes.ToArray();
            return self;
        }

        public T LabelAttr(object attributes)
        {
            _labelAttr = HtmlHelper.AnonymousObjectToHtmlAttributes(attributes);
            return self;
        }

        public override string ToHtmlString()
        {
            string idPrefix = GetName();

            if (string.IsNullOrWhiteSpace(_wrapFormat))
            {
                return GetOptions()
                    .Map(item => RenderOption(idPrefix, item))
                    .Aggregate(new StringBuilder(), (sb, html) => sb.Append(html))
                    .ToString();
            }
            return GetOptions()
                .Map(item => RenderOption(idPrefix, item))
                .Aggregate(new StringBuilder(), (sb, html) => sb.AppendFormat(_wrapFormat, html))
                .ToString();
        }

        private string RenderOption(string idPrefix, SelectListItem item)
        {
            string id = Html401IdUtil.CreateSanitizedId(idPrefix + "-" + item.Value);

            // <input id="id-value" type="radio" value="value" />
            T input = Attr(HtmlAttribute.Id, id)
                .Value(item.Value)
                .ToggleAttr(HtmlAttribute.Checked, item.Selected);
            // <label for="id-value">Text</label>
            Label label = new Label()
                .Attr(HtmlAttribute.For, id)
                .Attr(_labelAttr)
                .InnerText(item.Text);

            return input.ToTagString() + label.ToHtmlString();
        }

        class Label : Fragment<Label>
        {
            public Label()
                : base(HtmlElement.Label)
            {
            }
        }
    }
}
