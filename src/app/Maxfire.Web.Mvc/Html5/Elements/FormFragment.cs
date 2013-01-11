using System;
using System.Web.Mvc;
using Maxfire.Web.Mvc.FluentHtml.Extensions;
using Maxfire.Web.Mvc.Html5.HtmlTokens;

namespace Maxfire.Web.Mvc.Html5.Elements
{
	/// <summary>
	/// Base class for form elements that are associated with a model.
	/// </summary>
	public abstract class FormFragment<T> : Fragment<T> where T : FormFragment<T>
	{
		private const string DEFAULT_VALIDATION_CSS_CLASS = "input-validation-error";

		// TODO: Inherent label support via attribute (label from bindings that can take values none|before|after)
		private readonly IModelMetadataAccessor _accessor;

		protected FormFragment(string elementName, string name, IModelMetadataAccessor accessor) : base(elementName)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("The argument cannot be empty.", "name");
			}
			_accessor = accessor;
			Attr(HtmlAttribute.Name, name);
			// We do not want to auto generate id attribute. Front-end dev can write the id explicitly in markup.
			//Attr(HtmlAttribute.Id, name.FormatAsHtmlId());
		}

		protected IModelMetadataAccessor ModelMetadataAccessor { get { return _accessor; }}

		private void ApplyModelState()
		{
			if (_accessor != null)
			{
				string name = Attr(HtmlAttribute.Name);
				ModelState modelState = _accessor.GetModelState(name);
				if (modelState != null)
				{
					if (modelState.IsInvalid())
					{
						AddClass(DEFAULT_VALIDATION_CSS_CLASS);
					}
					if (modelState.Value != null)
					{
						// ModelState should always take precedence over any model based values (by convention)
						// To avoid modelstate overwriting model based values either remove it or use an explicit value
						BindAttemptedValue(GetAttemptedValue((modelState.Value)));
					}
				}
			}
		}

		protected virtual object GetAttemptedValue(ValueProviderResult attemptedValue)
		{
			return attemptedValue.ConvertTo<string>();
		}

		protected override string ToTagString()
		{
			ApplyModelState();
			return base.ToTagString();
		}
	}
}