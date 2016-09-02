using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using Maxfire.Core.Extensions;
using Maxfire.Web.Mvc.FluentHtml.Extensions;
using Maxfire.Web.Mvc.FluentHtml.Html;

namespace Maxfire.Web.Mvc.FluentHtml.Elements
{
    /// <summary>
    /// Base class for a set of radio buttons.
    /// </summary>
    public abstract class RadioSetBase<T> : OptionsElementBase<T> where T : RadioSetBase<T>
    {
        private string _format;
        private string _itemClass;

        protected RadioSetBase(string tag, string name, MemberExpression forMember)
            : base(tag, name, forMember)
        {
        }

        /// <summary>
        /// Set the selected option.
        /// </summary>
        /// <param name="selectedValue">A value matching the option to be selected.</param>
        /// <returns></returns>
        public virtual T Selected(object selectedValue)
        {
            SelectedValues = new[] { selectedValue.ToNullSafeString() };
            return (T)this;
        }

        /// <summary>
        /// Specify a format string for the HTML of each radio button and label.
        /// </summary>
        /// <param name="format">A format string.</param>
        public virtual T ItemFormat(string format)
        {
            _format = format;
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
            SetInnerHtml(RenderBody());
            base.PreRender();
        }

        protected override TagRenderMode TagRenderMode
        {
            get { return TagRenderMode.Normal; }
        }

        private string RenderBody()
        {
            RemoveAttr(HtmlAttribute.Name);
            var sb = new StringBuilder();
            foreach (var option in Options)
            {
                var value = option.Value;
                var radioButton = (new RadioButton(GetName(), ForMember)
                    .ApplyBehaviors(AppliedBehaviors)
                    .Value(value)
                    .Format(_format))
                    .LabelAfter(option.Text, _itemClass)
                    .Checked(IsSelectedValue(value));
                if (_itemClass != null)
                {
                    radioButton.Class(_itemClass);
                }
                sb.Append(radioButton);
            }
            return sb.ToString();
        }
    }
}
