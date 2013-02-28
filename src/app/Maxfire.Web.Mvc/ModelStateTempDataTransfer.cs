using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web.Mvc;

namespace Maxfire.Web.Mvc
{
	/////////////////////////
	// PRG pattern support //
	/////////////////////////

	public abstract class ModelStateTempDataTransfer : ActionFilterAttribute
	{
		public static NamedValue<ModelStateDictionary> GetNamedValue(Func<TempDataDictionary> tempDataAccessor)
		{
			return new NamedValue<ModelStateDictionary>("__modelStateTempDataTransfer", tempDataAccessor);
		}
	}

	public class ExportModelStateToTempData : ModelStateTempDataTransfer
	{
		public override void OnActionExecuted(ActionExecutedContext filterContext)
		{
			// Only export if model state is invalid and we are redirecting (i.e. using PRG pattern)
			if (filterContext.Controller.ViewData.ModelState.IsInvalid() &&
				((filterContext.Result is RedirectResult) || (filterContext.Result is RedirectToRouteResult)))
			{
				var namedTempData = GetNamedValue(() => filterContext.Controller.TempData);
				if (namedTempData.Value == null)
				{
					// only export model state once (controller might have done it already)
					namedTempData.Value = filterContext.Controller.ViewData.ModelState;
				}
			}
			base.OnActionExecuted(filterContext);
		}
	}

	public class ImportModelStateFromTempData : ModelStateTempDataTransfer
	{
		private ModelStateDictionary _originalModelState;

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var modelStateInTempData = GetNamedValue(() => filterContext.Controller.TempData);
			if (modelStateInTempData.Value != null)
			{
				_originalModelState = DeepCopy(filterContext.Controller.ViewData.ModelState);
				filterContext.Controller.ViewData.ModelState.Merge(modelStateInTempData.Value);
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
					// ...then we restore the modelstate
					filterContext.Controller.ViewData.ModelState.Clear();
					filterContext.Controller.ViewData.ModelState.Merge(_originalModelState);
					// ...and remove the temp data
					GetNamedValue(() => filterContext.Controller.TempData).Delete();
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