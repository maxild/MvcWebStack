using System;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.MicroKernel;
using Maxfire.Core.Extensions;

namespace Maxfire.Castle.Web.Mvc
{
	public class CastleControllerFactory : DefaultControllerFactory
	{
		private readonly IKernel _kernel;
		private readonly Func<RequestContext, string, Type> _getControllerTypeThunk;

		public CastleControllerFactory(IKernel kernel, IActionInvoker actionInvoker) 
			: this(kernel, actionInvoker, null)
		{
		}

		/// <summary>
		/// This constructor is only used for testing the factory.
		/// </summary>
		/// <remarks>
		/// The DefaultControllerFactory.GetControllerType() method will call 
		/// BuildManager.EnsureTopLevelFilesCompiled() and this fails in tests.
		/// </remarks>
		public CastleControllerFactory(IKernel kernel, IActionInvoker actionInvoker, Func<RequestContext, string, Type> getControllerTypeThunk)
			: base(new CastleControllerActivator(kernel, actionInvoker))
		{
			_kernel = kernel;
			_getControllerTypeThunk = getControllerTypeThunk;
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

		protected override Type GetControllerType(RequestContext requestContext, string controllerName)
		{
			return _getControllerTypeThunk != null
				       ? _getControllerTypeThunk(requestContext, controllerName)
				       : base.GetControllerType(requestContext, controllerName);
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
				// have been setup using a bootstrapper direct call or with
				// PreApplicationStartMethodAttribute, then the controller 
				// factory will still support the MvcRouteHandler route.
				IController controller;
				try
				{
					controller = _kernel.TryResolve(controllerType) as IController ??
					             Activator.CreateInstance(controllerType) as IController;
				}
				catch (Exception ex)
				{
					throw new InvalidOperationException(
						"The {0} failed to create an instance of {1}.".FormatWith(typeof (CastleControllerFactory).Name,
						                                                          controllerType.FullName), ex);
				}
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