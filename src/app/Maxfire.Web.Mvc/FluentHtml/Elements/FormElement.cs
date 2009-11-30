using System.Linq.Expressions;
using System.Web.Mvc;
using Maxfire.Core.Extensions;
using Maxfire.Web.Mvc.FluentHtml.Extensions;
using Maxfire.Web.Mvc.FluentHtml.Html;

namespace Maxfire.Web.Mvc.FluentHtml.Elements
{
	/// <summary>
	/// Base class for form elements.
	/// </summary>
	/// <typeparam name="T">Derived type</typeparam>
	public abstract class FormElement<T> : Element<T>, ILabeledElement where T : FormElement<T>, IElement, ILabeledElement
	{
		const string LABEL_FORMAT = "{0}_Label";

		protected FormElement(string tag, string name, MemberExpression forMember)
			: base(tag, forMember)
		{
			if (name.IsNotEmpty())
			{
				SetName(name);
			}
		}

		/// <summary>
		/// All names of form elements are camel cased (i.e. first character is lower case)
		/// </summary>
		protected string GetName()
		{
			return GetAttr(HtmlAttribute.Name); 
		}
		
		protected void SetName(string name) {
			SetAttr(HtmlAttribute.Name, name.LowerCaseFirstWord());
		}

		/// <summary>
		/// Set the disabled attribute.
		/// </summary>
		/// <param name="value">Whether the element should be disabled.</param>
		public T Disabled(bool value)
		{
			if (value)
			{
				SetAttr(HtmlAttribute.Disabled, HtmlAttribute.Disabled);
			}
			else
			{
				RemoveAttr(HtmlAttribute.Disabled);
			}
			return (T)this;
		}

		/// <summary>
		/// Generate a label before the element.
		/// </summary>
		/// <param name="value">The inner text of the label.</param>
		/// <param name="class">The value of the 'class' attribute for the label.</param>
		public T Label(string value, string @class)
		{
			LabelBeforeText = value;
			LabelClass = @class;
			return (T)this;
		}

		/// <summary>
		/// Generate a label before the element.
		/// </summary>
		/// <param name="value">The inner text of the label.</param>
		public T Label(string value)
		{
			LabelBeforeText = value;
			return (T)this;
		}

		/// <summary>
		/// Generate a label after the element.
		/// </summary>
		/// <param name="value">The inner text of the label.</param>
		/// <param name="class">The value of the 'class' attribute for the label.</param>
		public T LabelAfter(string value, string @class)
		{
			LabelAfterText = value;
			LabelClass = @class;
			return (T)this;
		}

		/// <summary>
		/// Generate a label after the element.
		/// </summary>
		/// <param name="value">The inner text of the label.</param>
		public T LabelAfter(string value)
		{
			LabelAfterText = value;
			return (T)this;
		}

		public T NoLabel()
		{
			LabelBeforeText = null;
			LabelAfterText = null;
			return (T)this;
		}

		string ILabeledElement.LabelBeforeText
		{
			get { return LabelBeforeText; }
			set { LabelBeforeText = value; }
		}
		
		string ILabeledElement.LabelAfterText
		{
			get { return LabelAfterText; }
			set { LabelAfterText = value; }
		}
		
		string ILabeledElement.LabelClass
		{
			get { return LabelClass; }
			set { LabelClass = value; }
		}

		public override string ToString()
		{
			InferIdFromName();
			base.PreRender();
			string html = RenderLabel(LabelBeforeText);
			html += base.ToString();
			html += RenderLabel(LabelAfterText);
			return html;
		}

		protected virtual string RenderLabel(string labelText)
		{
			if (labelText == null)
			{
				return null;
			}
			var labelBuilder = getLabelBuilder();
			labelBuilder.SetInnerText(labelText);
			return labelBuilder.ToString();
		}

		private TagBuilder getLabelBuilder()
		{
			var labelBuilder = new TagBuilder(HtmlTag.Label);
			if (HasId())
			{
				var id = GetId();
				labelBuilder.MergeAttribute(HtmlAttribute.For, id);
				labelBuilder.MergeAttribute(HtmlAttribute.Id, string.Format(LABEL_FORMAT, id));
			}
			if (!string.IsNullOrEmpty(LabelClass))
			{
				labelBuilder.MergeAttribute(HtmlAttribute.Class, LabelClass);
			}
			return labelBuilder;
		}

		/// <summary>
		/// Determines how the HTML element is closed.
		/// </summary>
		protected override TagRenderMode TagRenderMode
		{
			get { return TagRenderMode.SelfClosing; }
		}

		protected virtual void InferIdFromName()
		{
			if (!HasId())
			{
				SetId(GetName().FormatAsHtmlId());
			}
		}

		protected string LabelBeforeText { get; set; }

		protected string LabelAfterText { get; set; }

		protected string LabelClass { get; set; }
	}
}