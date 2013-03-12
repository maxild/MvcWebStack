using System;
using System.Reflection;
using Castle.MicroKernel;
using Castle.MicroKernel.ComponentActivator;

namespace Maxfire.Castle.Web.Mvc
{
	public static class CastleMicroKernelExtensions
	{
		public static void InjectProperties(this IKernel kernel, object target)
		{
			var type = target.GetType();
			foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
			{
				if (property.CanWrite && kernel.HasComponent(property.PropertyType))
				{
					var value = kernel.Resolve(property.PropertyType);
					try
					{
						property.SetValue(target, value, null);
					}
					catch (Exception ex)
					{
						var message = string.Format("Error setting property {0} on type {1}, See inner exception for more information.", property.Name, type.FullName);
						throw new ComponentActivatorException(message, ex, null);
					}
				}
			}
		}

		/// <summary>
		/// Determine if component can be resolved by serviceType
		/// </summary>
		public static bool HasComponent<TServiceType>(this IKernel kernel)
		{
			return kernel.HasComponent(typeof(TServiceType));
		}

		/// <summary>
		/// Try resolve component by serviceType
		/// </summary>
		public static TServiceType TryResolve<TServiceType>(this IKernel kernel)
			where TServiceType : class
		{
			return kernel.HasComponent<TServiceType>() ? kernel.Resolve<TServiceType>() : null;
		}

		/// <summary>
		/// Try resolve component by key.
		/// </summary>
		public static TServiceType TryResolve<TServiceType>(this IKernel kernel, string key)
			where TServiceType : class
		{
			return kernel.HasComponent(key) ? kernel.Resolve<TServiceType>(key) : null;
		}

		/// <summary>
		/// Try resolve component by serviceType
		/// </summary>
		public static object TryResolve(this IKernel kernel, Type serviceType)
		{
			return kernel.HasComponent(serviceType) ? kernel.Resolve(serviceType) : null;
		}

		/// <summary>
		/// Try resolve component by key.
		/// </summary>
		public static object TryResolve(this IKernel kernel, string key, Type serviceType)
		{
			return kernel.HasComponent(key) ? kernel.Resolve(key, serviceType) : null;
		}
	}
}