using System.Collections.Generic;
using System.Web.Mvc;
using Castle.MicroKernel;
using Maxfire.Web.Mvc;

namespace Maxfire.Castle.Web.Mvc
{
	public class CastleControllerActionInvoker : BetterControllerActionInvoker
	{
		private readonly IKernel _kernel;

		public CastleControllerActionInvoker(IKernel kernel) 
			: this(kernel, null)
		{
		}

		public CastleControllerActionInvoker(IKernel kernel, IValueProviderFactory valueProviderFactory)
			: base(valueProviderFactory)
		{
			_kernel = kernel;
		}

		protected override ActionExecutedContext InvokeActionMethodWithFilters(ControllerContext controllerContext, 
		                                                                       IList<IActionFilter> filters, ActionDescriptor actionDescriptor, IDictionary<string, object> parameters)
		{
			foreach (var filter in filters)
			{
				_kernel.InjectProperties(filter);
			}
			return base.InvokeActionMethodWithFilters(controllerContext, filters, actionDescriptor, parameters);
		}
	}
}