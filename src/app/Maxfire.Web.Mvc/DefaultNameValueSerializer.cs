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
		private readonly CultureInfo _culture;


		public DefaultNameValueSerializer()
			: this(CultureInfo.InvariantCulture)
		{
		}

		public DefaultNameValueSerializer(CultureInfo culture) 
			: this(culture, new DefaultNameValueSerializerProvider())
		{
		}

		public DefaultNameValueSerializer(CultureInfo culture, INameValueSerializerProvider provider)
		{
			_culture = culture;
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
			return GetSerializerCore(modelType) ?? new FallbackNameValueSerializer(_culture);
		}

		protected virtual INameValueSerializer GetSerializerCore(Type modelType)
		{
			return _provider.GetSerializer(modelType);
		}

		class FallbackNameValueSerializer : INameValueSerializer
		{
			private readonly CultureInfo _culture;

			public FallbackNameValueSerializer(CultureInfo culture)
			{
				_culture = culture;
			}

			public IDictionary<string, object> GetValues(object model, string prefix)
			{
				object value = TypeExtensions.ConvertSimpleType(_culture, model, typeof(string));
				return new Dictionary<string, object>(1){{prefix, value}};
			}
		}
	}
}