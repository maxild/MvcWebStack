using System;
using System.Globalization;
using System.Resources;
using System.Web.Mvc;

namespace Maxfire.Web.Mvc
{
    public class ResourceManagerModelBinderErrorMessageProvider : IModelBinderErrorMessageProvider
    {
        private readonly ResourceManager _resourceManager;

        public ResourceManagerModelBinderErrorMessageProvider(ResourceManager resourceManager)
        {
            if (resourceManager == null)
            {
                throw new ArgumentNullException("resourceManager");
            }
            _resourceManager = resourceManager;
        }

        public string GetValueRequiredMessage(ControllerContext controllerContext)
        {
            return GetResourceString("PropertyValueRequired");
        }

        public string GetValueInvalidMessage(ControllerContext controllerContext)
        {
            return GetResourceString("PropertyValueInvalid");
        }

        private string GetResourceString(string resourceName)
        {
            return _resourceManager.GetString(resourceName, CultureInfo.CurrentUICulture);
        }
    }
}
