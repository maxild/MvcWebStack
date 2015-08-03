using System;
using System.Collections.Generic;
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
        public static NamedValue<ModelStateDictionary> GetNamedValue(Func<IDictionary<string, object>> tempDataAccessor)
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
                var namedTempData = GetNamedValue(() => new PeekReadingTempDateDictionary(filterContext.Controller.TempData));

                // Only export model state once (controller might have done it already)
                if (namedTempData.Value == null)
                {
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
            var modelStateInTempData = GetNamedValue(() => new PeekReadingTempDateDictionary(filterContext.Controller.TempData));

            // Always import such that controller will see any exported modelstate (but 
            // for redirects we will restore the original modelstate just after the action 
            // has finished...see below in OnActionExecuted)
            if (modelStateInTempData.Value != null)
            {
                _originalModelState = DeepCopy(filterContext.Controller.ViewData.ModelState);
                filterContext.Controller.ViewData.ModelState.Merge(modelStateInTempData.Value);
            }

            base.OnActionExecuting(filterContext);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            // Did we import any model state before executing the action?
            if (_originalModelState != null)
            {
                // If we are not rendering a page or a partial page, i.e. we are redirecting, ...
                if (!(filterContext.Result is ViewResultBase))
                {
                    // ...then we restore the modelstate
                    filterContext.Controller.ViewData.ModelState.Clear();
                    filterContext.Controller.ViewData.ModelState.Merge(_originalModelState);
                }

                // Remove the model state from temp data
                GetNamedValue(() => filterContext.Controller.TempData).Delete();

                _originalModelState = null;
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
