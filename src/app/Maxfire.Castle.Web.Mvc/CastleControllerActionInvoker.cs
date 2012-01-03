using System.Collections.Generic;
using System.Web.Mvc;
using Castle.MicroKernel;

namespace Maxfire.Castle.Web.Mvc
{
	public class CastleControllerActionInvoker : ControllerActionInvoker
	{
		private readonly IKernel _kernel;

		public CastleControllerActionInvoker(IKernel kernel)
		{
			_kernel = kernel;
		}

		protected override ActionExecutedContext InvokeActionMethodWithFilters(ControllerContext controllerContext, 
		                                                                       IList<IActionFilter> filters, ActionDescriptor actionDescriptor, IDictionary<string, object> parameters)
		{
			doSetterInjection(filters);
			return base.InvokeActionMethodWithFilters(controllerContext, filters, actionDescriptor, parameters);
		}

		private void doSetterInjection(IEnumerable<IActionFilter> filters)
		{
			foreach (var filter in filters)
			{
				_kernel.InjectProperties(filter);
			}
		}
	}
}