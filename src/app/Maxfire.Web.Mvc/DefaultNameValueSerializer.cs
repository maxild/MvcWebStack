using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Web.Mvc;
using Maxfire.Core.Extensions;
using Maxfire.Core.Reflection;

namespace Maxfire.Web.Mvc
{
	public class DefaultNameValueSerializer : INameValueSerializer
	{
		public IDictionary<string, object> GetValues(object model)
		{
			return GetValues(model, string.Empty);
		}

		public IDictionary<string, object> GetValues(object model, string prefix)
		{
			Type modelType = model.GetType();
			var values = new Dictionary<string, object>();

			if (modelType.IsSimpleType())
			{
				var serializer = GetSerializerCore(modelType);
				if (serializer != null)
				{
					AddRange(values, serializer.GetValues(model, prefix));
				}
				else
				{
					object value = TypeExtensions.ConvertSimpleType(CultureInfo.InvariantCulture, model, typeof (string));
					values.Add(prefix, value);
				}
				return values;
			}

			Type dictionaryType = modelType.MatchesGenericInterface(typeof(IDictionary<,>));
			if (dictionaryType != null)
			{
				// todo
				throw new NotSupportedException("TODO: Implement disctionary support");
			}

			Type enumerableType = modelType.MatchesGenericInterface(typeof(IEnumerable<>));
			if (enumerableType != null)
			{
				Type itemType = enumerableType.GetGenericArguments()[0];
				var serializer = GetSerializer(itemType);
				((IEnumerable)model).Each((item, index) => AddRange(values, serializer.GetValues(item, prefix + "[" + index + "]")));
				return values;
			}

			// otherwise, just serialize the properties on the complex type
			foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(model))
			{
				object value = descriptor.GetValue(model);
				if (value == null) continue;
				string name = descriptor.Name;
				if (!string.IsNullOrEmpty(prefix))
				{
					name = prefix + "." + name;
				}

				Type propertyType = descriptor.PropertyType;
				var serializer = GetSerializer(propertyType);
				AddRange(values, serializer.GetValues(value, name));
			}

			return values;
		}

		private static void AddRange(IDictionary<string, object> values, IDictionary<string, object> valuesToAdd)
		{
			foreach (var kvp in valuesToAdd)
			{
				values.Add(kvp.Key, kvp.Value);
			}
		}

		private INameValueSerializer GetSerializer(Type modelType)
		{
			return GetSerializerCore(modelType) ?? this;
		}

		protected virtual INameValueSerializer GetSerializerCore(Type modelType)
		{
			// 1. Binder returned from provider
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