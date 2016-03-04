using System.Globalization;
using System.Web.Mvc;
using Maxfire.Core.Extensions;

namespace Maxfire.Web.Mvc
{
    public class DefaultModelBinderErrorMessageProvider : IModelBinderErrorMessageProvider
    {
        public DefaultModelBinderErrorMessageProvider(string resourceClassKey = null)
        {
            ResourceClassKey = resourceClassKey ?? string.Empty;
        }

        public string ResourceClassKey { get; }

        public string GetValueRequiredMessage(ControllerContext controllerContext)
        {
            return GetGlobalResourceString(controllerContext, "PropertyValueRequired") ?? "A value is required.";
        }

        public string GetValueInvalidMessage(ControllerContext controllerContext)
        {
            return GetGlobalResourceString(controllerContext, "PropertyValueInvalid") ?? "The value '{0}' is not valid for {1}.";
        }

        private string GetGlobalResourceString(ControllerContext controllerContext, string resourceName)
        {
            if (ResourceClassKey.IsEmpty() || controllerContext?.HttpContext == null)
                return null;

            return controllerContext.HttpContext.GetGlobalResourceObject(ResourceClassKey, resourceName,
                    CultureInfo.CurrentUICulture) as string;
        }
    }
}
