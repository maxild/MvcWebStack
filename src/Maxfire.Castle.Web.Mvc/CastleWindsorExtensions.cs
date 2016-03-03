using System;
using Castle.Windsor;

namespace Maxfire.Castle.Web.Mvc
{
	public static class CastleWindsorExtensions
	{
		/// <summary>
		/// Determine if component can be resolved by key
		/// </summary>
		public static bool HasComponent(this IWindsorContainer container, string key)
		{
			return container.Kernel.HasComponent(key);
		}

		/// <summary>
		/// Determine if component can be resolved by serviceType
		/// </summary>
		public static bool HasComponent(this IWindsorContainer container, Type serviceType)
		{
			return container.Kernel.HasComponent(serviceType);
		}

		/// <summary>
		/// Determine if component can be resolved by serviceType
		/// </summary>
		public static bool HasComponent<TServiceType>(this IWindsorContainer container)
		{
			return container.Kernel.HasComponent(typeof(TServiceType));
		}

		/// <summary>
		/// Try resolve component by serviceType
		/// </summary>
		public static TServiceType TryResolve<TServiceType>(this IWindsorContainer container)
			where TServiceType : class
		{
			return container.HasComponent<TServiceType>() ? container.Resolve<TServiceType>() : null;
		}

		/// <summary>
		/// Try resolve component by key.
		/// </summary>
		public static TServiceType TryResolve<TServiceType>(this IWindsorContainer container, string key)
			where TServiceType : class
		{
			return container.HasComponent(key) ? container.Resolve<TServiceType>(key) : null;
		}

		/// <summary>
		/// Try resolve component by serviceType
		/// </summary>
		public static object TryResolve(this IWindsorContainer container, Type serviceType)
		{
			return container.HasComponent(serviceType) ? container.Resolve(serviceType) : null;
		}

		/// <summary>
		/// Try resolve component by key.
		/// </summary>
		public static object TryResolve(this IWindsorContainer container, string key, Type serviceType)
		{
			return container.HasComponent(key) ? container.Resolve(key, serviceType) : null;
		}
	}
}