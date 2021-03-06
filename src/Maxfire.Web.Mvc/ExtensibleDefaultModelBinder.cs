using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Web.Mvc;
using JetBrains.Annotations;
using Maxfire.Core.Extensions;
using Maxfire.Prelude.Linq;

namespace Maxfire.Web.Mvc
{
    /// <summary>
	/// An extensible duplicate of the MVC frameworks own DefaultModelBinder
	/// </summary>
	public class ExtensibleDefaultModelBinder : IModelBinder
	{
        public ExtensibleDefaultModelBinder() : this(null)
        {
        }

        public ExtensibleDefaultModelBinder(IModelBinderErrorMessageProvider errorMessageProvider)
        {
            ErrorMessageProvider = errorMessageProvider ?? new DefaultModelBinderErrorMessageProvider();
        }

        public IModelBinderErrorMessageProvider ErrorMessageProvider { get; }

		private void AddValueRequiredMessageToModelState(ControllerContext controllerContext, ModelStateDictionary modelState, string modelStateKey, Type elementType, object value)
		{
			if (value == null && !elementType.AllowsNullValue() && modelState.IsValidField(modelStateKey))
			{
				modelState.AddModelError(modelStateKey, ErrorMessageProvider.GetValueRequiredMessage(controllerContext));
			}
		}

		protected virtual void BindComplexElementalModel(ControllerContext controllerContext, ModelBindingContext bindingContext, object model)
		{
			// need to replace the property filter + model object and create an inner binding context
			ModelBindingContext newBindingContext = CreateComplexElementalModelBindingContext(controllerContext, bindingContext, model);

			// validation
			if (OnModelUpdating(controllerContext, newBindingContext))
			{
				BindProperties(controllerContext, newBindingContext);
				OnModelUpdated(controllerContext, newBindingContext);
			}
		}

		protected virtual object BindComplexModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			object model = bindingContext.Model;
			Type modelType = bindingContext.ModelType;

			// if we're being asked to create an array, create a list instead, then coerce to an array after the list is created
			if (model == null && modelType.IsArray)
			{
				Type elementType = modelType.GetElementType();
				Type listType = typeof(List<>).MakeGenericType(elementType);
				object collection = CreateModel(controllerContext, bindingContext, listType);

				var arrayBindingContext = new ModelBindingContext
				{
					ModelMetadata = GetMetadataForType(() => collection, listType),
					ModelName = bindingContext.ModelName,
					ModelState = bindingContext.ModelState,
					PropertyFilter = bindingContext.PropertyFilter,
					ValueProvider = bindingContext.ValueProvider
				};
				var list = (IList)UpdateCollection(controllerContext, arrayBindingContext, elementType);

				if (list == null)
				{
					return null;
				}

			    // ReSharper disable once AssignNullToNotNullAttribute
			    Array array = Array.CreateInstance(elementType, list.Count);
				list.CopyTo(array, 0);
				return array;
			}

			if (model == null)
			{
				model = CreateModel(controllerContext, bindingContext, modelType);
			}

			// special-case IDictionary<,> and ICollection<>
			Type dictionaryType = modelType.ExtractGenericInterface(typeof(IDictionary<,>));
			if (dictionaryType != null)
			{
				Type[] genericArguments = dictionaryType.GetGenericArguments();
				Type keyType = genericArguments[0];
				Type valueType = genericArguments[1];

				var dictionaryBindingContext = new ModelBindingContext
				{
					ModelMetadata = GetMetadataForType(() => model, modelType),
					ModelName = bindingContext.ModelName,
					ModelState = bindingContext.ModelState,
					PropertyFilter = bindingContext.PropertyFilter,
					ValueProvider = bindingContext.ValueProvider
				};
				object dictionary = UpdateDictionary(controllerContext, dictionaryBindingContext, keyType, valueType);
				return dictionary;
			}

			Type enumerableType = modelType.ExtractGenericInterface(typeof(IEnumerable<>));
			if (enumerableType != null)
			{
				Type elementType = enumerableType.GetGenericArguments()[0];

				Type collectionType = typeof(ICollection<>).MakeGenericType(elementType);
				if (collectionType.IsInstanceOfType(model))
				{
					var collectionBindingContext = new ModelBindingContext
					{
						ModelMetadata = GetMetadataForType(() => model, modelType),
						ModelName = bindingContext.ModelName,
						ModelState = bindingContext.ModelState,
						PropertyFilter = bindingContext.PropertyFilter,
						ValueProvider = bindingContext.ValueProvider
					};
					object collection = UpdateCollection(controllerContext, collectionBindingContext, elementType);
					return collection;
				}
			}

			// otherwise, just update the properties on the complex type
			BindComplexElementalModel(controllerContext, bindingContext, model);
			return model;
		}

		public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			if (bindingContext == null)
			{
				throw new ArgumentNullException(nameof(bindingContext));
			}

			bool notFound = bindingContext.ModelName.IsNotEmpty() && !bindingContext.ValueProvider.ContainsPrefix(bindingContext.ModelName);
			if (notFound)
			{
				if (!bindingContext.FallbackToEmptyPrefix)
				{
					return null;
				}
				// Fallback to empty prefix
				bindingContext = new ModelBindingContext
				{
					ModelMetadata = bindingContext.ModelMetadata,
					ModelState = bindingContext.ModelState,
					PropertyFilter = bindingContext.PropertyFilter,
					ValueProvider = bindingContext.ValueProvider
				};
			}

			return BindModelCore(controllerContext, bindingContext);
		}

		private void BindProperties(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			IEnumerable<PropertyDescriptor> properties = GetFilteredModelProperties(controllerContext, bindingContext);
			foreach (PropertyDescriptor property in properties)
			{
				BindProperty(controllerContext, bindingContext, property);
			}
		}

		protected virtual void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor)
		{
			// need to skip properties that aren't part of the request, else we might hit a StackOverflowException
			string fullPropertyKey = CreateSubPropertyName(bindingContext.ModelName, propertyDescriptor.Name);
			if (!bindingContext.ValueProvider.ContainsPrefix(fullPropertyKey))
			{
				return;
			}

			// call into the property's model binder
			IModelBinder propertyBinder = GetBinder(propertyDescriptor.PropertyType);
			object originalPropertyValue = propertyDescriptor.GetValue(bindingContext.Model);
			ModelMetadata propertyMetadata = bindingContext.PropertyMetadata[propertyDescriptor.Name];
			propertyMetadata.Model = originalPropertyValue;
			var innerBindingContext = new ModelBindingContext
			{
				ModelMetadata = propertyMetadata,
				ModelName = fullPropertyKey,
				ModelState = bindingContext.ModelState,
				ValueProvider = bindingContext.ValueProvider
			};
			object newPropertyValue = GetPropertyValue(controllerContext, innerBindingContext, propertyDescriptor, propertyBinder);
			propertyMetadata.Model = newPropertyValue;

			// validation
			ModelState modelState = bindingContext.ModelState[fullPropertyKey];
			if (modelState == null || modelState.Errors.Count == 0)
			{
				if (OnPropertyValidating(controllerContext, bindingContext, propertyDescriptor, newPropertyValue))
				{
					SetProperty(controllerContext, bindingContext, propertyDescriptor, newPropertyValue);
					OnPropertyValidated(controllerContext, bindingContext, propertyDescriptor, newPropertyValue);
				}
			}
			else
			{
				SetProperty(controllerContext, bindingContext, propertyDescriptor, newPropertyValue);

				// Convert FormatExceptions (type conversion failures) into InvalidValue messages
				foreach (ModelError error in modelState.Errors.Where(err => string.IsNullOrEmpty(err.ErrorMessage) && err.Exception != null).ToList())
				{
					for (Exception exception = error.Exception; exception != null; exception = exception.InnerException)
					{
						if (exception is FormatException)
						{
							string displayName = propertyMetadata.GetDisplayName();
							string errorMessageTemplate = ErrorMessageProvider.GetValueInvalidMessage(controllerContext);
							string errorMessage = string.Format(CultureInfo.CurrentCulture, errorMessageTemplate, modelState.Value.AttemptedValue, displayName);
							modelState.Errors.Remove(error);
							modelState.Errors.Add(errorMessage);
							break;
						}
					}
				}
			}
		}

		protected virtual object BindSimpleModel(ControllerContext controllerContext, ModelBindingContext bindingContext, ValueProviderResult valueProviderResult)
		{
			bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

			// if the value provider returns an instance of the requested data type, we can just short-circuit
			// the evaluation and return that instance
			if (bindingContext.ModelType.IsInstanceOfType(valueProviderResult.RawValue))
			{
				return valueProviderResult.RawValue;
			}

			// since a string is an IEnumerable<char>, we want it to skip the two checks immediately following
			if (bindingContext.ModelType != typeof(string))
			{
				if (bindingContext.ModelType.IsArray)
				{
					// ValueProviderResult.ConvertTo() understands array types, so pass in the array type directly
					object modelArray = ConvertProviderResult(bindingContext.ModelState, bindingContext.ModelName, valueProviderResult, bindingContext.ModelType);
					return modelArray;
				}

				Type enumerableType = bindingContext.ModelType.ExtractGenericInterface(typeof(IEnumerable<>));
				if (enumerableType != null)
				{
					// need to call ConvertTo() on the array type, then copy the array to the collection
					object modelCollection = CreateModel(controllerContext, bindingContext, bindingContext.ModelType);
					Type elementType = enumerableType.GetGenericArguments()[0];
					Type arrayType = elementType.MakeArrayType();
					object modelArray = ConvertProviderResult(bindingContext.ModelState, bindingContext.ModelName, valueProviderResult, arrayType);

					Type collectionType = typeof(ICollection<>).MakeGenericType(elementType);
					if (collectionType.IsInstanceOfType(modelCollection))
					{
						CollectionHelpers.ReplaceCollection(elementType, modelCollection, modelArray);
					}
					return modelCollection;
				}
			}

			object model = ConvertProviderResult(bindingContext.ModelState, bindingContext.ModelName, valueProviderResult, bindingContext.ModelType);
			return model;
		}

		private static bool CanUpdateReadonlyTypedReference(Type type)
		{
			// value types aren't strictly immutable, but because they have copy-by-value semantics
			// we can't update a value type that is marked readonly
			if (type.IsValueType)
			{
				return false;
			}

			// arrays are mutable, but because we can't change their length we shouldn't try
			// to update an array that is referenced readonly
			if (type.IsArray)
			{
				return false;
			}

			// special-case known common immutable types
			if (type == typeof(string))
			{
				return false;
			}

			return true;
		}

		private static object ConvertProviderResult(ModelStateDictionary modelState, string modelStateKey, ValueProviderResult valueProviderResult, Type destinationType)
		{
			try
			{
				object convertedValue = valueProviderResult.ConvertTo(destinationType);
				return convertedValue;
			}
			catch (Exception ex)
			{
				modelState.AddModelError(modelStateKey, ex);
				return null;
			}
		}

		protected virtual ModelBindingContext CreateComplexElementalModelBindingContext(ControllerContext controllerContext, ModelBindingContext bindingContext, object model)
		{
			var bindAttr = (BindAttribute)GetTypeDescriptor(controllerContext, bindingContext).GetAttributes()[typeof(BindAttribute)];
			Predicate<string> newPropertyFilter = bindAttr != null
							   ? propertyName => bindAttr.IsPropertyAllowed(propertyName) && bindingContext.PropertyFilter(propertyName)
							   : bindingContext.PropertyFilter;

			var newBindingContext = new ModelBindingContext
			{
				ModelMetadata = GetMetadataForType(() => model, bindingContext.ModelType),
				ModelName = bindingContext.ModelName,
				ModelState = bindingContext.ModelState,
				PropertyFilter = newPropertyFilter,
				ValueProvider = bindingContext.ValueProvider
			};

			return newBindingContext;
		}

		protected virtual object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
		{
			Type typeToCreate = modelType;

			// we can understand some collection interfaces, e.g. IList<>, IDictionary<,>
			if (modelType.IsGenericType)
			{
				Type genericTypeDefinition = modelType.GetGenericTypeDefinition();
				if (genericTypeDefinition == typeof(IDictionary<,>))
				{
					typeToCreate = typeof(Dictionary<,>).MakeGenericType(modelType.GetGenericArguments());
				}
				else if (genericTypeDefinition == typeof(IEnumerable<>) ||
						 genericTypeDefinition == typeof(ICollection<>) ||
						 genericTypeDefinition == typeof(IList<>))
				{
					typeToCreate = typeof(List<>).MakeGenericType(modelType.GetGenericArguments());
				}
			}

			// fallback to the type's default constructor
			return Activator.CreateInstance(typeToCreate);
		}

		protected static string CreateSubIndexName(string prefix, int index)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}[{1}]", prefix, index);
		}

		protected static string CreateSubIndexName(string prefix, string index)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}[{1}]", prefix, index);
		}

		protected static string CreateSubPropertyName(string prefix, string propertyName)
		{
			if (string.IsNullOrEmpty(prefix))
			{
				return propertyName;
			}
			if (string.IsNullOrEmpty(propertyName))
			{
				return prefix;
			}
			return prefix + "." + propertyName;
		}

		protected IEnumerable<PropertyDescriptor> GetFilteredModelProperties(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			PropertyDescriptorCollection properties = GetModelProperties(controllerContext, bindingContext);
			Predicate<string> propertyFilter = bindingContext.PropertyFilter;

			return from PropertyDescriptor property in properties
				   where ShouldUpdateProperty(property, propertyFilter)
				   select property;
		}

		private void GetIndexes(ModelBindingContext bindingContext, out bool stopOnIndexNotFound, out IEnumerable<string> indexes)
		{
			stopOnIndexNotFound = false;
			indexes = GetCollectionIndexes(bindingContext);
			if (indexes == null)
			{
				stopOnIndexNotFound = true;
				indexes = GetZeroBasedIndexes();
			}
		}

		protected virtual PropertyDescriptorCollection GetModelProperties(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			return GetTypeDescriptor(controllerContext, bindingContext).GetProperties();
		}

		protected virtual object GetPropertyValue(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, IModelBinder propertyBinder)
		{
			object value = propertyBinder.BindModel(controllerContext, bindingContext);

			if (bindingContext.ModelMetadata.ConvertEmptyStringToNull && Equals(value, string.Empty))
			{
				return null;
			}

			return value;
		}

		protected virtual ICustomTypeDescriptor GetTypeDescriptor(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			return TypeDescriptor.GetProvider(bindingContext.ModelType).GetTypeDescriptor(bindingContext.ModelType);
		}


		[SuppressMessage("ReSharper", "IteratorNeverReturns")]
		private static IEnumerable<string> GetZeroBasedIndexes()
		{
			int i = 0;
			while (true)
			{
				yield return i.ToString(CultureInfo.InvariantCulture);
				i++;
			}
		}

		protected virtual void OnModelUpdated(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			var startedValid = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

			foreach (ModelValidationResult validationResult in ModelValidator.GetModelValidator(bindingContext.ModelMetadata, controllerContext).Validate(null))
			{
				string subPropertyName = CreateSubPropertyName(bindingContext.ModelName, validationResult.MemberName);

				if (!startedValid.ContainsKey(subPropertyName))
				{
					startedValid[subPropertyName] = bindingContext.ModelState.IsValidField(subPropertyName);
				}

				if (startedValid[subPropertyName])
				{
					bindingContext.ModelState.AddModelError(subPropertyName, validationResult.Message);
				}
			}
		}

		protected virtual bool OnModelUpdating(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			// default implementation does nothing
			return true;
		}

		protected virtual void OnPropertyValidated(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, object value)
		{
			// default implementation does nothing
		}

		protected virtual bool OnPropertyValidating(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, object value)
		{
			// default implementation does nothing
			return true;
		}

		protected virtual void SetProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, object value)
		{
			ModelMetadata propertyMetadata = bindingContext.PropertyMetadata[propertyDescriptor.Name];
			propertyMetadata.Model = value;
			string modelStateKey = CreateSubPropertyName(bindingContext.ModelName, propertyMetadata.PropertyName);

			// If the value is null, and the validation system can find a Required validator for
			// us, we'd prefer to run it before we attempt to set the value; otherwise, property
			// setters which throw on null (f.e., Entity Framework properties which are backed by
			// non-nullable strings in the DB) will get their error message in ahead of us.
			//
			// We are effectively using the special validator -- Required -- as a helper to the
			// binding system, which is why this code is here instead of in the Validating/Validated
			// methods, which are really the old-school validation hooks.
			if (value == null && bindingContext.ModelState.IsValidField(modelStateKey))
			{
				ModelValidator requiredValidator = ModelValidatorProviders.Providers.GetValidators(propertyMetadata, controllerContext).FirstOrDefault(v => v.IsRequired);
				if (requiredValidator != null)
				{
					foreach (ModelValidationResult validationResult in requiredValidator.Validate(bindingContext.Model))
					{
						bindingContext.ModelState.AddModelError(modelStateKey, validationResult.Message);
					}
				}
			}

			bool isNullValueOnNonNullableType = value == null && !propertyDescriptor.PropertyType.AllowsNullValue();

			// Try to set a value into the property unless we know it will fail (read-only
			// properties and null values with non-nullable types)
			if (!propertyDescriptor.IsReadOnly && !isNullValueOnNonNullableType)
			{
				try
				{
// ReSharper disable AssignNullToNotNullAttribute
					propertyDescriptor.SetValue(bindingContext.Model, value);
// ReSharper restore AssignNullToNotNullAttribute
				}
				catch (Exception ex)
				{
					// Only add if we're not already invalid
					if (bindingContext.ModelState.IsValidField(modelStateKey))
					{
						bindingContext.ModelState.AddModelError(modelStateKey, ex);
					}
				}
			}

			// Last chance for an error on null values with non-nullable types, we'll use
			// the default "A value is required." message.
			if (isNullValueOnNonNullableType && bindingContext.ModelState.IsValidField(modelStateKey))
			{
				bindingContext.ModelState.AddModelError(modelStateKey, ErrorMessageProvider.GetValueRequiredMessage(controllerContext));
			}
		}

		private static bool ShouldPerformRequestValidation(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			if (controllerContext?.Controller == null || bindingContext?.ModelMetadata == null)
			{
				return true;
			}

			// We should perform request validation only if both the controller and the model ask for it. This is the
			// default behavior for both. If either the controller (via [ValidateInput(false)]) or the model (via [AllowHtml])
			// opts out, we don't validate.
			return (controllerContext.Controller.ValidateRequest && bindingContext.ModelMetadata.RequestValidationEnabled);
		}

		private static bool ShouldUpdateProperty(PropertyDescriptor property, Predicate<string> propertyFilter)
		{
			if (property.IsReadOnly && !CanUpdateReadonlyTypedReference(property.PropertyType))
			{
				return false;
			}

			// if this property is rejected by the filter, move on
			if (!propertyFilter(property.Name))
			{
				return false;
			}

			// otherwise, allow
			return true;
		}

		protected virtual object UpdateCollection(ControllerContext controllerContext, ModelBindingContext bindingContext, Type elementType)
		{
			bool stopOnIndexNotFound;
			IEnumerable<string> indexes;
			GetIndexes(bindingContext, out stopOnIndexNotFound, out indexes);
			IModelBinder elementBinder = GetBinder(elementType);

			// build up a list of items from the request
			var modelList = new List<object>();
			foreach (string currentIndex in indexes)
			{
				string subIndexKey = CreateSubIndexName(bindingContext.ModelName, currentIndex);
				if (!bindingContext.ValueProvider.ContainsPrefix(subIndexKey))
				{
					if (stopOnIndexNotFound)
					{
						// we ran out of elements to pull
						break;
					}
					continue;
				}

				var innerContext = new ModelBindingContext
				{
					ModelMetadata = GetMetadataForType(null, elementType),
					ModelName = subIndexKey,
					ModelState = bindingContext.ModelState,
					PropertyFilter = bindingContext.PropertyFilter,
					ValueProvider = bindingContext.ValueProvider
				};
				object thisElement = elementBinder.BindModel(controllerContext, innerContext);

				// we need to merge model errors up
				AddValueRequiredMessageToModelState(controllerContext, bindingContext.ModelState, subIndexKey, elementType, thisElement);
				modelList.Add(thisElement);
			}

			// if there weren't any elements at all in the request, just return
			if (modelList.Count == 0)
			{
				return null;
			}

			// replace the original collection
			object collection = bindingContext.Model;
			CollectionHelpers.ReplaceCollection(elementType, collection, modelList);
			return collection;
		}

		protected virtual object UpdateDictionary(ControllerContext controllerContext, ModelBindingContext bindingContext, Type keyType, Type valueType)
		{
			bool stopOnIndexNotFound;
			IEnumerable<string> indexes;
			GetIndexes(bindingContext, out stopOnIndexNotFound, out indexes);

			IModelBinder keyBinder = GetBinder(keyType);
			IModelBinder valueBinder = GetBinder(valueType);

			// build up a list of items from the request
			var modelList = new List<KeyValuePair<object, object>>();
			foreach (string currentIndex in indexes)
			{
				string subIndexKey = CreateSubIndexName(bindingContext.ModelName, currentIndex);
				string keyFieldKey = CreateSubPropertyName(subIndexKey, "key");
				string valueFieldKey = CreateSubPropertyName(subIndexKey, "value");

				if (!(bindingContext.ValueProvider.ContainsPrefix(keyFieldKey) && bindingContext.ValueProvider.ContainsPrefix(valueFieldKey)))
				{
					if (stopOnIndexNotFound)
					{
						// we ran out of elements to pull
						break;
					}
					continue;
				}

				// bind the key
				var keyBindingContext = new ModelBindingContext
				{
					ModelMetadata = GetMetadataForType(null, keyType),
					ModelName = keyFieldKey,
					ModelState = bindingContext.ModelState,
					ValueProvider = bindingContext.ValueProvider
				};
				object thisKey = keyBinder.BindModel(controllerContext, keyBindingContext);

				// we need to merge model errors up
				AddValueRequiredMessageToModelState(controllerContext, bindingContext.ModelState, keyFieldKey, keyType, thisKey);
				if (!keyType.IsInstanceOfType(thisKey))
				{
					// we can't add an invalid key, so just move on
					continue;
				}

				// bind the value
				modelList.Add(CreateEntryForModel(controllerContext, bindingContext, valueType, valueBinder, valueFieldKey, thisKey));
			}

			// Let's try another method
			if (modelList.Count == 0)
			{
				var enumerableValueProvider = bindingContext.ValueProvider as IEnumerableValueProvider;
				if (enumerableValueProvider != null)
				{
					IDictionary<string, string> keys = enumerableValueProvider.GetKeysFromPrefix(bindingContext.ModelName);
					modelList.AddRange(keys.Map(thisKey => CreateEntryForModel(controllerContext, bindingContext, valueType, valueBinder, thisKey.Value, thisKey.Key)));
				}
			}

			// if there weren't any elements at all in the request, just return
			if (modelList.Count == 0)
			{
				return null;
			}

			// replace the original collection
			object dictionary = bindingContext.Model;
			CollectionHelpers.ReplaceDictionary(keyType, valueType, dictionary, modelList);
			return dictionary;
		}

		private KeyValuePair<object, object> CreateEntryForModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type valueType, IModelBinder valueBinder, string modelName, object modelKey)
		{
			var valueBindingContext = new ModelBindingContext
			{
				ModelMetadata = GetMetadataForType(null, valueType),
				ModelName = modelName,
				ModelState = bindingContext.ModelState,
				PropertyFilter = bindingContext.PropertyFilter,
				ValueProvider = bindingContext.ValueProvider
			};
			object thisValue = valueBinder.BindModel(controllerContext, valueBindingContext);
			AddValueRequiredMessageToModelState(controllerContext, bindingContext.ModelState, modelName, valueType, thisValue);
			return new KeyValuePair<object, object>(modelKey, thisValue);
		}

		private static class CollectionHelpers
		{
			private static readonly MethodInfo REPLACE_COLLECTION_METHOD = typeof(CollectionHelpers).GetMethod("ReplaceCollectionImpl", BindingFlags.Static | BindingFlags.NonPublic);
			private static readonly MethodInfo REPLACE_DICTIONARY_METHOD = typeof(CollectionHelpers).GetMethod("ReplaceDictionaryImpl", BindingFlags.Static | BindingFlags.NonPublic);

			[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
			public static void ReplaceCollection(Type collectionType, object collection, object newContents)
			{
				MethodInfo targetMethod = REPLACE_COLLECTION_METHOD.MakeGenericMethod(collectionType);
				targetMethod.Invoke(null, new [] { collection, newContents });
			}

			[UsedImplicitly]
			private static void ReplaceCollectionImpl<T>(ICollection<T> collection, IEnumerable newContents)
			{
				collection.Clear();
				if (newContents != null)
				{
					foreach (object item in newContents)
					{
						// if the item was not a T, some conversion failed. the error message will be propagated,
						// but in the meanwhile we need to make a placeholder element in the array.
						T castItem = (item is T) ? (T)item : default(T);
						collection.Add(castItem);
					}
				}
			}

			[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
			public static void ReplaceDictionary(Type keyType, Type valueType, object dictionary, object newContents)
			{
				MethodInfo targetMethod = REPLACE_DICTIONARY_METHOD.MakeGenericMethod(keyType, valueType);
				targetMethod.Invoke(null, new [] { dictionary, newContents });
			}

			[UsedImplicitly]
			private static void ReplaceDictionaryImpl<TKey, TValue>(IDictionary<TKey, TValue> dictionary, IEnumerable<KeyValuePair<object, object>> newContents)
			{
				dictionary.Clear();
				foreach (KeyValuePair<object, object> item in newContents)
				{
					// if the item was not a T, some conversion failed. the error message will be propagated,
					// but in the meanwhile we need to make a placeholder element in the dictionary.
					var castKey = (TKey)item.Key; // this cast shouldn't fail
					var castValue = (item.Value is TValue) ? (TValue)item.Value : default(TValue);
					dictionary[castKey] = castValue;
				}
			}
		}

		//
		// New methods
		//

		protected virtual IModelBinder GetBinder(Type modelType)
		{
			IModelBinder binder = ModelBinders.Binders.GetBinder(modelType);
			return binder ?? this;
		}

		protected virtual ModelMetadata GetMetadataForType(Func<object> modelAccessor, Type modelType)
		{
			return ModelMetadataProviders.Current.GetMetadataForType(modelAccessor, modelType);
		}

		protected virtual object BindModelCore(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			if (bindingContext.ModelName.IsNotEmpty())
			{
				bool performRequestValidation = ShouldPerformRequestValidation(controllerContext, bindingContext);
				IUnvalidatedValueProvider unvalidatedValueProvider = bindingContext.GetUnvalidatedValueProvider();
				var valueProviderResult = unvalidatedValueProvider.GetValue(bindingContext.ModelName, !performRequestValidation);
				if (valueProviderResult != null)
				{
					// a value in the request did exactly match the name of the model we're binding to,
					// and therefore we have a simple model (int, string, etc). This is a 'leaf node',
					// and the call to BindSimpleModel is non-recursive.
					return BindSimpleModel(controllerContext, bindingContext, valueProviderResult);
				}
			}

			if (!bindingContext.ModelMetadata.IsComplexType)
			{
				return null;
			}

			return BindComplexModel(controllerContext, bindingContext);
		}

		protected virtual IEnumerable<string> GetCollectionIndexes(ModelBindingContext bindingContext)
		{
			string indexKey = CreateSubPropertyName(bindingContext.ModelName, "index");
			ValueProviderResult vpResult = bindingContext.ValueProvider.GetValue(indexKey);
			return vpResult != null ? vpResult.ConvertTo(typeof(string[])) as string[] : null;
		}
	}
}
