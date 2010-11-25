using System;
using System.Web.Mvc;
using Maxfire.Web.Mvc.FluentHtml.Extensions;
using Maxfire.Web.Mvc.Html5.HtmlTokens;

namespace Maxfire.Web.Mvc.Html5.Elements
{
	/// <summary>
	/// Base class for form elements that are associated with a model.
	/// </summary>
	public abstract class FormElement<T> : Element<T> where T : FormElement<T>
	{
		private const string DEFAULT_VALIDATION_CSS_CLASS = "input-validation-error";

		// TODO: Inherent label support via attribute (label from bindings that can take values none|before|after)
		private readonly IModelMetadataAccessor _accessor;

		protected FormElement(string tagName, string name, IModelMetadataAccessor accessor) : base(tagName)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("The argument cannot be empty.", "name");
			}
			// TODO: Can accessor be null?
			_accessor = accessor;
			Attr(HtmlAttribute.Name, name);
		}

		protected IModelMetadataAccessor ModelMetadataAccessor { get { return _accessor; }}

		protected virtual void ApplyModelState()
		{
			var name = Attr(HtmlAttribute.Name);
			if (name == null || _accessor == null)
			{
				return;
			}

			ModelState modelState = _accessor.GetModelState(name);
			if (modelState != null)
			{
				if (modelState.IsInvalid())
				{
					AddClass(DEFAULT_VALIDATION_CSS_CLASS);
				}
				if (modelState.Value != null)
				{
					ApplyModelStateAttemptedValue(modelState.Value);
				}
			}

		}

		private void InferIdFromName()
		{
			if (!HasAttr(HtmlAttribute.Id))
			{
				string name = Attr(HtmlAttribute.Name);
				SetId(name.FormatAsHtmlId());
			}
		}

		protected abstract void ApplyModelStateAttemptedValue(ValueProviderResult attemptedValue);

		public override string ToString()
		{
			InferIdFromName();
			ApplyModelState();
			return base.ToString();
		}
	}
}