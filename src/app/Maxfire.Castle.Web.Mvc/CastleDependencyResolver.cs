using System;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel;
using Castle.Windsor;
using IDependencyResolver = System.Web.Mvc.IDependencyResolver;

namespace Maxfire.Castle.Web.Mvc
{
	/// <summary>
	/// MVC Service Locator based on Castle.Windsor
	/// </summary>
	public class CastleDependencyResolver : IDependencyResolver
	{
		private readonly IKernel _kernel;

		public CastleDependencyResolver(IWindsorContainer container)
		{
			_kernel = container.Kernel;
		}

		public object GetService(Type serviceType)
		{
			return _kernel.TryResolve(serviceType);
		}

		public IEnumerable<object> GetServices(Type serviceType)
		{
			return _kernel.HasComponent(serviceType) ? _kernel.ResolveAll(serviceType).Cast<object>() : Enumerable.Empty<object>();
		}
	}
}