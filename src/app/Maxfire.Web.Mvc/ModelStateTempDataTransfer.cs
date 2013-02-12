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
		protected const string KEY = "__modelStateTempDataTransfer";
	}

	public class ExportModelStateToTempData : ModelStateTempDataTransfer
	{
		public override void OnActionExecuted(ActionExecutedContext filterContext)
		{
			// Only export if model state is invalid and we are redirecting (i.e. using PRG pattern)
			if (filterContext.Controller.ViewData.ModelState.IsInvalid() &&
				((filterContext.Result is RedirectResult) || (filterContext.Result is RedirectToRouteResult)))
			{
				// export model state
				filterContext.Controller.TempData[KEY] = filterContext.Controller.ViewData.ModelState;
				// export collection indices to be recycled
				filterContext.Controller.TempData["..todo..."] = filterContext.Controller.ViewData["..todo..."];
			}

			base.OnActionExecuted(filterContext);
		}
	}

	public class ImportModelStateFromTempData : ModelStateTempDataTransfer
	{
		private ModelStateDictionary _originalModelState;

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			ModelStateDictionary modelState = filterContext.Controller.TempData[KEY] as ModelStateDictionary;
			if (modelState != null)
			{
				_originalModelState = DeepCopy(filterContext.Controller.ViewData.ModelState);
				filterContext.Controller.ViewData.ModelState.Merge(modelState);
			}
			base.OnActionExecuting(filterContext);
		}

		public override void OnActionExecuted(ActionExecutedContext filterContext)
		{
			if (_originalModelState != null)
			{
				// If we are not rendering a page or a partial page 
				if (!(filterContext.Result is ViewResultBase))
				{
					// restore the modelstate and remove the temp data
					filterContext.Controller.ViewData.ModelState.Clear();
					filterContext.Controller.ViewData.ModelState.Merge(_originalModelState);
					filterContext.Controller.TempData.Remove(KEY);
				}
			}
			base.OnActionExecuted(filterContext);
		}

		public static ModelStateDictionary DeepCopy(ModelStateDictionary obj)
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