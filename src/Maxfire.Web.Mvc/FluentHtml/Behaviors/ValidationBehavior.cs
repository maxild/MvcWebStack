using System;
using System.Web.Mvc;
using Maxfire.Web.Mvc.FluentHtml.Html;

namespace Maxfire.Web.Mvc.FluentHtml.Behaviors
{
	public class ValidationBehavior : IBehavior
	{
		private const string DEFAULT_VALIDATION_CSS_CLASS = "input-validation-error";
		private readonly Func<ModelStateDictionary> _modelStateDictionaryFunc;
		private readonly string _validationErrorCssClass;

		public ValidationBehavior(Func<ModelStateDictionary> modelStateDictionaryFunc)
			: this(modelStateDictionaryFunc, DEFAULT_VALIDATION_CSS_CLASS) { }

		public ValidationBehavior(Func<ModelStateDictionary> modelStateDictionaryFunc, string validationErrorCssClass)
		{
			_modelStateDictionaryFunc = modelStateDictionaryFunc;
			_validationErrorCssClass = validationErrorCssClass;
		}

		public void Execute(IElement element)
		{
			var name = element.GetAttr(HtmlAttribute.Name);
			var supportsModelState = element as ISupportsModelState;

			if (name == null)
			{
				return;
			}

			ModelState state;
			if (_modelStateDictionaryFunc().TryGetValue(name, out state) && state.Errors != null && state.Errors.Count > 0)
			{
				element.AddCssClass(_validationErrorCssClass);

				// Todo: ISupporsModelState should be renamed to ISupportsModelStateValueBinding.BindTo(state.Value)
				if (state.Value != null && supportsModelState != null)
				{
					supportsModelState.ApplyModelState(state);
				}
			}
		}

	}
}