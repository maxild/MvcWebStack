using System;
using System.Web.Mvc;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Maxfire.Core.Extensions;

namespace Maxfire.Castle.Web.Mvc
{
	public class CastleModelBinderProvider : IModelBinderProvider
	{
		private readonly IKernel _kernel;

		public CastleModelBinderProvider(IKernel kernel)
		{
			_kernel = kernel;
		}

		public IModelBinder GetBinder(Type modelType)
		{
			string key = GetKey(modelType);
			return _kernel.TryResolve<IModelBinder>(key);
		}

		public CastleModelBinderProvider AddModelBinderInstanceFor<TModel>(IModelBinder modelBinder)
		{
			string key = GetKey(typeof(TModel));
			_kernel.Register(Component.For<IModelBinder>()
				.Named(key)
				.UsingFactoryMethod(_ => modelBinder)
				.LifeStyle.Singleton);
			return this;
		}

		public static string GetKey(Type modelType)
		{
			return "ModelBinderFor_{0}".FormatWith(modelType.FullName);
		}
	}
}