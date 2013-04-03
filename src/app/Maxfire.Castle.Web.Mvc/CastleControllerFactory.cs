using System;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.MicroKernel;

namespace Maxfire.Castle.Web.Mvc
{
	public class CastleControllerFactory : DefaultControllerFactory
	{
		private readonly IKernel _kernel;

		public CastleControllerFactory(IKernel kernel, IActionInvoker actionInvoker) 
			: base(new CastleControllerActivator(kernel, actionInvoker))
		{
			_kernel = kernel;
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

		class CastleControllerActivator : IControllerActivator
		{
			private readonly IKernel _kernel;
			private readonly IActionInvoker _actionInvoker;

			public CastleControllerActivator(IKernel kernel, IActionInvoker actionInvoker)
			{
				_kernel = kernel;
				_actionInvoker = actionInvoker;
			}

			public IController Create(RequestContext requestContext, Type controllerType)
			{
				var controller = _kernel.TryResolve(controllerType) as IController;
				if (_actionInvoker != null)
				{
					var c = controller as Controller;
					if (c != null)
					{
						c.ActionInvoker = _actionInvoker;
					}
				}
				return controller;
			}
		}
	}
}