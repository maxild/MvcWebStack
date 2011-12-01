using System;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.MicroKernel;

namespace Maxfire.Castle.Web.Mvc
{
	public class CastleControllerActivator : IControllerActivator
	{
		private readonly IKernel _kernel;

		public CastleControllerActivator(IKernel kernel)
		{
			_kernel = kernel;
		}

		public virtual IController Create(RequestContext requestContext, Type controllerType)
		{
			return _kernel.TryResolve(controllerType) as IController;
		}

		public virtual void Release(IController controller)
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