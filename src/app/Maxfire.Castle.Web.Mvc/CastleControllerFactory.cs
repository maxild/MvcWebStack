using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.MicroKernel;

namespace Maxfire.Castle.Web.Mvc
{
	public class CastleControllerFactory : DefaultControllerFactory
	{
		private readonly IKernel _kernel;

		public CastleControllerFactory(IKernel kernel)
		{
			if (kernel == null)
			{
				throw new ArgumentNullException("kernel");
			}

			_kernel = kernel;
		}

		protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
		{
			if (controllerType == null)
			{
				throw new HttpException(404, string.Format("The controller for path '{0}' could not be found or it does not implement IController.", requestContext.HttpContext.Request.Path));
			}

			var controller = (IController) _kernel.GetService(controllerType);

			injectOpinionatedActionInvoker(controller);

			return controller;
		}

		private void injectOpinionatedActionInvoker(IController controller)
		{
			var opinionatedController = controller as Controller;
			if (opinionatedController != null)
			{
				opinionatedController.ActionInvoker = new CastleControllerActionInvoker(_kernel);
			}
		}

		public override void ReleaseController(IController controller)
		{
			var disposable = controller as IDisposable;

			if (disposable != null)
			{
				disposable.Dispose();
			}

			_kernel.ReleaseComponent(controller);
		}
	}
}