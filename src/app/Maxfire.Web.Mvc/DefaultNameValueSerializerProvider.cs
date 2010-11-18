using System;
using System.Web.Mvc;
using Maxfire.Core.Reflection;

namespace Maxfire.Web.Mvc
{
	public class DefaultNameValueSerializerProvider : INameValueSerializerProvider
	{
		public INameValueSerializer GetSerializer(Type modelType)
		{
			// 1. Binder returned from provider (this is a multi-service registration point, and GetBinder(modelType) will pick the first provider returning non-null binder)
			INameValueSerializer serializer = ModelBinderProviders.BinderProviders.GetBinder(modelType) as INameValueSerializer;
			if (serializer != null)
			{
				return serializer;
			}

			// 2. Binder registered in the global table
			IModelBinder binder;
			if (ModelBinders.Binders.TryGetValue(modelType, out binder) && binder is INameValueSerializer)
			{
				return binder as INameValueSerializer;
			}

			// 3. Binder attribute defined on the type
			var modelBinderAttribute = modelType.GetCustomAttribute<CustomModelBinderAttribute>();
			if (modelBinderAttribute != null)
			{
				return modelBinderAttribute.GetBinder() as INameValueSerializer;
			}

			return null;
		}
	}
}