using System;
using System.Web.Mvc;
using Maxfire.Core;

namespace Maxfire.Web.Mvc
{
	public abstract class OpinionatedRestfulController<TInputModel, TEditModel, TModel, TId> : OpinionatedController, IRestfulController<TInputModel, TId>
		where TInputModel : class, IEntityViewModel<TId>, new()
		where TModel : class, IEntity<TId>
	{
		protected OpinionatedRestfulController(IModelUnflattener<TInputModel, TModel> modelUnflattener)
		{
			ModelUnflattener = modelUnflattener;
		}

		public string BindingPrefix { get; private set; }

		protected IModelUnflattener<TInputModel, TModel> ModelUnflattener { get; }

		public abstract ActionResult Create(TInputModel input);

		public abstract ActionResult Update(TInputModel input);

		protected ActionResult ProcessInput(
			TInputModel input,
			Func<TModel> modelInstantiator,
			Func<ActionResult> invalidInputResult,
			Func<ActionResult> actionFailureResult,
			Func<TModel, ActionResult> successResult)
		{
			if (this.IsInputInvalid())
			{
				return invalidInputResult();
			}

			PerformUpdateValidation(input);

			if (this.IsInputInvalid())
			{
				return invalidInputResult();
			}

			TModel model = null;

			if (modelInstantiator != null)
			{
				model = modelInstantiator();
				if (model == null)
				{
					return actionFailureResult();
				}
			}

			return successResult(model);
		}

		protected virtual void PerformUpdateValidation(TInputModel input)
		{
			var validationErrors = ModelUnflattener.ValidateInput(input);
			ModelState.AddValidationErrors(validationErrors, BindingPrefix);
		}

		protected virtual ActionResult EditViewFor(TEditModel editModel)
		{
			return View("Edit", editModel);
		}

		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			BindingPrefix = filterContext.GetBindingPrefixOfParameterWithType<TInputModel>();
		}
	}
}
