using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Maxfire.Core.Extensions;

namespace Maxfire.Web.Mvc
{
	public class DefaultNameValueSerializer : INameValueSerializer
	{
		private readonly INameValueSerializerProvider _provider;

		public DefaultNameValueSerializer() :this(new DefaultNameValueSerializerProvider())
		{
		}

		public DefaultNameValueSerializer(INameValueSerializerProvider provider)
		{
			_provider = provider;
		}

		public IDictionary<string, object> GetValues(object model, string prefix)
		{
			var values = new Dictionary<string, object>();

			if (model == null)
			{
				return values;
			}

			Type modelType = model.GetType();
			if (modelType.IsSimpleType())
			{
				var serializer = GetSerializerForSimpleType(modelType);
				values.AddRange(serializer.GetValues(model, prefix));
				return values;
			}

			Type dictionaryType = modelType.MatchesGenericInterface(typeof(IDictionary<,>));
			if (dictionaryType != null)
			{
				// todo
				throw new NotSupportedException("TODO: Implement dictionary support");
			}

			Type enumerableType = modelType.MatchesGenericInterface(typeof(IEnumerable<>));
			if (enumerableType != null)
			{
				Type itemType = enumerableType.GetGenericArguments()[0];
				var serializer = GetSerializer(itemType);
				((IEnumerable)model).Each((item, index) => values.AddRange(serializer.GetValues(item, prefix + "[" + index + "]")));
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
				values.AddRange(serializer.GetValues(value, name));
			}

			return values;
		}

		private INameValueSerializer GetSerializer(Type modelType)
		{
			// See if provider knows how to handle type, otherwise fallback to recursion
			return GetSerializerCore(modelType) ?? this;
		}

		private INameValueSerializer GetSerializerForSimpleType(Type modelType)
		{
			// See if provider knows how to handle type, otherwise fallback to simple type conversion
			return GetSerializerCore(modelType) ?? new FallbackNameValueSerializer();
		}

		protected virtual INameValueSerializer GetSerializerCore(Type modelType)
		{
			return _provider.GetSerializer(modelType);
		}

		class FallbackNameValueSerializer : INameValueSerializer
		{
			public IDictionary<string, object> GetValues(object model, string prefix)
			{
				object value = TypeExtensions.ConvertSimpleType(CultureInfo.InvariantCulture, model, typeof(string));
				return new Dictionary<string, object>{{prefix, value}};
			}
		}
	}
}