using System.Web.Mvc;

namespace Maxfire.Web.Mvc
{
    public interface IModelBinderErrorMessageProvider
    {
        string GetValueRequiredMessage(ControllerContext controllerContext);
        string GetValueInvalidMessage(ControllerContext controllerContext);
    }
}
