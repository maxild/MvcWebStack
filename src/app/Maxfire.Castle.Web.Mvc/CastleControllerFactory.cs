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
				// First try IoC container, but if nothing is registered,
				// then try default ctor of controller type. This way if 
				// routing is delegating to 'foreign' controllers, and this 
				// have been setup by 'foreigners' (e.g. RavenDB profiler)
				// using something like PreApplicationStartMethodAttribute,
				// the controller factory still supports it.
				var controller = _kernel.TryResolve(controllerType) as IController ??
				                 Activator.CreateInstance(controllerType) as IController;
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