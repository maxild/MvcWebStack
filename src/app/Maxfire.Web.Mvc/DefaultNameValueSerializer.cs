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
				var serializer = GetSerializer(modelType);
				values.AddRange(serializer.GetValues(model, prefix));
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

		protected virtual INameValueSerializer GetSerializer(Type modelType)
		{
			return _provider.GetSerializer(modelType) ?? new FallbackNameValueSerializer();
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