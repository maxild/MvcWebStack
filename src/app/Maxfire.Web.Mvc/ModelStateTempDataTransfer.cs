using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web.Mvc;

namespace Maxfire.Web.Mvc
{
	/////////////////////////
	// PRG pattern support //
	/////////////////////////

	public class ModelStateTempDataTransferBagWrapper : BagWrapper<TempDataDictionary, ModelStateDictionary>
	{
		public ModelStateTempDataTransferBagWrapper(Func<TempDataDictionary> thunk)
			: base(thunk, "__modelStateTempDataTransfer")
		{
		}
	}

	public class ExportModelStateToTempData : ActionFilterAttribute
	{
		public override void OnActionExecuted(ActionExecutedContext filterContext)
		{
			// Only export if model state is invalid and we are redirecting (i.e. using PRG pattern)
			if (filterContext.Controller.ViewData.ModelState.IsInvalid() &&
				((filterContext.Result is RedirectResult) || (filterContext.Result is RedirectToRouteResult)))
			{
				var helper = new ModelStateTempDataTransferBagWrapper(() => filterContext.Controller.TempData);
				if (helper.Value == null)
				{
					// only export model state once (controller might have done it already)
					helper.Value = filterContext.Controller.ViewData.ModelState;
				}
			}
			base.OnActionExecuted(filterContext);
		}
	}

	public class ImportModelStateFromTempData : ActionFilterAttribute
	{
		private ModelStateDictionary _originalModelState;

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var helper = new ModelStateTempDataTransferBagWrapper(() => filterContext.Controller.TempData);
			if (helper.Value != null)
			{
				_originalModelState = DeepCopy(filterContext.Controller.ViewData.ModelState);
				filterContext.Controller.ViewData.ModelState.Merge(helper.Value);
			}
			base.OnActionExecuting(filterContext);
		}

		public override void OnActionExecuted(ActionExecutedContext filterContext)
		{
			if (_originalModelState != null)
			{
				// If we are not rendering a page or a partial page...
				if (!(filterContext.Result is ViewResultBase))
				{
					// ...then we restore the modelstate and remove the temp data
					filterContext.Controller.ViewData.ModelState.Clear();
					filterContext.Controller.ViewData.ModelState.Merge(_originalModelState);
					var helper = new ModelStateTempDataTransferBagWrapper(() => filterContext.Controller.TempData);
					helper.RemoveValue();
				}
			}
			base.OnActionExecuted(filterContext);
		}

		static ModelStateDictionary DeepCopy(ModelStateDictionary obj)
		{
			using (var ms = new MemoryStream())
			{
				var formatter = new BinaryFormatter();
				formatter.Serialize(ms, obj);
				ms.Position = 0;
				return (ModelStateDictionary)formatter.Deserialize(ms);
			}
		}
	}
}