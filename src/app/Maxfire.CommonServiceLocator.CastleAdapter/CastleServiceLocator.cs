using System;
using System.Collections.Generic;
using Castle.MicroKernel;
using Microsoft.Practices.ServiceLocation;

namespace Maxfire.CommonServiceLocator.CastleAdapter
{
	/// <summary>
	/// Adapts the behavior of the Castle container to the common
	/// <see cref="IServiceLocator"/>
	/// </summary>
	public class CastleServiceLocator : ServiceLocatorImplBase
	{
		private readonly IKernel _kernel;

		/// <summary>
		/// Initializes a new instance of the <see cref="CastleServiceLocator"/> class.
		/// </summary>
		/// <param name="kernel">The container.</param>
		public CastleServiceLocator(IKernel kernel)
		{
			_kernel = kernel;
		}

		/// <summary>
		///             When implemented by inheriting classes, this method will do the actual work of resolving
		///             the requested service instance.
		/// </summary>
		/// <param name="serviceType">Type of instance requested.</param>
		/// <param name="key">Name of registered service you want. May be null.</param>
		/// <returns>
		/// The requested service instance.
		/// </returns>
		protected override object DoGetInstance(Type serviceType, string key)
		{
			if (key != null)
			{
				return _kernel.Resolve(key, serviceType);
			}

			return _kernel.Resolve(serviceType);
		}

		/// <summary>
		///             When implemented by inheriting classes, this method will do the actual work of
		///             resolving all the requested service instances.
		/// </summary>
		/// <param name="serviceType">Type of service requested.</param>
		/// <returns>
		/// Sequence of service instance objects.
		/// </returns>
		protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
		{
			return (object[])_kernel.ResolveAll(serviceType);
		}
	}
}