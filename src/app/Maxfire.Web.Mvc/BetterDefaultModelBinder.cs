using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Web.Mvc;
using Maxfire.Core.Extensions;

namespace Maxfire.Web.Mvc
{
	public class BetterDefaultModelBinder : IModelBinder
	{
		private static string _resourceClassKey;
		public static string ResourceClassKey
		{
			get { return _resourceClassKey ?? String.Empty; }
			set { _resourceClassKey = value; }
		}

		private static void AddValueRequiredMessageToModelState(ControllerContext controllerContext, ModelStateDictionary modelState, string modelStateKey, Type elementType, object value)
		{
			if (value == null && !elementType.AllowsNullValue() && modelState.IsValidField(modelStateKey))
			{
				modelState.AddModelError(modelStateKey, GetValueRequiredResource(controllerContext));
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

				ModelMetadata arrayModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => collection, listType);
				var arrayBindingContext = bindingContext.ChangeModelMetadata(arrayModelMetadata);
				var list = (IList)UpdateCollection(controllerContext, arrayBindingContext, elementType);

				if (list == null)
				{
					return null;
				}

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

				ModelMetadata dictionaryModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, modelType);
				ModelBindingContext dictionaryBindingContext = bindingContext.ChangeModelMetadata(dictionaryModelMetadata);
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
					ModelMetadata collectionModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, modelType);
					ModelBindingContext collectionBindingContext = bindingContext.ChangeModelMetadata(collectionModelMetadata);
					object collection = UpdateCollection(controllerContext, collectionBindingContext, elementType);
					return collection;
				}
			}

			// otherwise, just update the properties on the complex type
			BindComplexElementalModel(controllerContext, bindingContext, model);
			return model;
		}

		public virtual object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			if (bindingContext == null)
			{
				throw new ArgumentNullException("bindingContext");
			}

			// Because it is impossible to decide up front if any keys are matching the properties of 
			// a complex model type, if FallbackToEmptyPrefix == true have resulted in an empty 
			// modelname (=prefix). This check will however cover most cases, where only one model type 
			// is used at a time (on every action)
			var keyEnumerableValueProvider = bindingContext.ValueProvider as IKeyEnumerableValueProvider;
			if (keyEnumerableValueProvider != null)
			{
				// Complex model type with no keys found should return non-updated model (possibly null, instead of no-arg ctor value)
				if (keyEnumerableValueProvider.GetKeys().All(key => bindingContext.IsRequiredRouteValue(key)))
				{
					return bindingContext.Model;
				}
			}

			bool notFound = bindingContext.ModelName.IsNotEmpty() && !bindingContext.ValueProvider.ContainsPrefix(bindingContext.ModelName);
			if (notFound)
			{
				if (!bindingContext.FallbackToEmptyPrefix)
				{
					return null;
				}
				// Fallback to empty prefix
				bindingContext = bindingContext.ChangeModelName();
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
				foreach (ModelError error in modelState.Errors.Where(err => String.IsNullOrEmpty(err.ErrorMessage) && err.Exception != null).ToList())
				{
					for (Exception exception = error.Exception; exception != null; exception = exception.InnerException)
					{
						if (exception is FormatException)
						{
							string displayName = propertyMetadata.GetDisplayName();
							string errorMessageTemplate = GetValueInvalidResource(controllerContext);
							string errorMessage = String.Format(CultureInfo.CurrentCulture, errorMessageTemplate, modelState.Value.AttemptedValue, displayName);
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

		private ModelBindingContext CreateComplexElementalModelBindingContext(ControllerContext controllerContext, ModelBindingContext bindingContext, object model)
		{
			var bindAttr = (BindAttribute)GetTypeDescriptor(controllerContext, bindingContext).GetAttributes()[typeof(BindAttribute)];
			Predicate<string> newPropertyFilter = bindAttr != null
							   ? propertyName => bindAttr.IsPropertyAllowed(propertyName) && bindingContext.PropertyFilter(propertyName)
							   : bindingContext.PropertyFilter;

			var newBindingContext = new ModelBindingContext
			{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, bindingContext.ModelType),
				ModelName = bindingContext.ModelName,
				ModelState = bindingContext.ModelState,
				PropertyFilter = newPropertyFilter,
				ValueProvider = bindingContext.ValueProvider
			};

			return newBindingContext;
		}

		public virtual object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
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
			return String.Format(CultureInfo.InvariantCulture, "{0}[{1}]", prefix, index);
		}

		protected static string CreateSubIndexName(string prefix, string index)
		{
			return String.Format(CultureInfo.InvariantCulture, "{0}[{1}]", prefix, index);
		}

		protected static string CreateSubPropertyName(string prefix, string propertyName)
		{
			if (String.IsNullOrEmpty(prefix))
			{
				return propertyName;
			}
			if (String.IsNullOrEmpty(propertyName))
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

			if (bindingContext.ModelMetadata.ConvertEmptyStringToNull && Equals(value, String.Empty))
			{
				return null;
			}

			return value;
		}

		protected virtual ICustomTypeDescriptor GetTypeDescriptor(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			return TypeDescriptor.GetProvider(bindingContext.ModelType).GetTypeDescriptor(bindingContext.ModelType);
		}

		private static string GetUserResourceString(ControllerContext controllerContext, string resourceName)
		{
			if (ResourceClassKey.IsEmpty() || controllerContext == null || controllerContext.HttpContext == null)
				return null;

			return controllerContext.HttpContext.GetGlobalResourceObject(ResourceClassKey, resourceName, CultureInfo.CurrentUICulture) as string;
		}

		private static string GetValueRequiredResource(ControllerContext controllerContext)
		{
			return GetUserResourceString(controllerContext, "PropertyValueRequired") ?? "The value is required";
		}

		private static string GetValueInvalidResource(ControllerContext controllerContext)
		{
			return GetUserResourceString(controllerContext, "PropertyValueInvalid") ?? "The value is invalid";
		}

		private static IEnumerable<string> GetZeroBasedIndexes()
		{
			int i = 0;
			while (true)
			{
				yield return i.ToString(CultureInfo.InvariantCulture);
				i++;
			}
// ReSharper disable FunctionNeverReturns
		}
// ReSharper restore FunctionNeverReturns

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

		public virtual bool OnModelUpdating(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			// default implementation does nothing
			return true;
		}

		public virtual void OnPropertyValidated(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, object value)
		{
			// default implementation does nothing
		}

		public virtual bool OnPropertyValidating(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, object value)
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
				bindingContext.ModelState.AddModelError(modelStateKey, GetValueRequiredResource(controllerContext));
			}
		}

		private static bool ShouldPerformRequestValidation(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			if (controllerContext == null || controllerContext.Controller == null || bindingContext == null || bindingContext.ModelMetadata == null)
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
					ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, elementType),
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
					ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, keyType),
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

		private static KeyValuePair<object, object> CreateEntryForModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type valueType, IModelBinder valueBinder, string modelName, object modelKey)
		{
			var valueBindingContext = new ModelBindingContext
			{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, valueType),
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
			private static readonly MethodInfo _replaceCollectionMethod = typeof(CollectionHelpers).GetMethod("ReplaceCollectionImpl", BindingFlags.Static | BindingFlags.NonPublic);
			private static readonly MethodInfo _replaceDictionaryMethod = typeof(CollectionHelpers).GetMethod("ReplaceDictionaryImpl", BindingFlags.Static | BindingFlags.NonPublic);

			[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
			public static void ReplaceCollection(Type collectionType, object collection, object newContents)
			{
				MethodInfo targetMethod = _replaceCollectionMethod.MakeGenericMethod(collectionType);
				targetMethod.Invoke(null, new [] { collection, newContents });
			}

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
				MethodInfo targetMethod = _replaceDictionaryMethod.MakeGenericMethod(keyType, valueType);
				targetMethod.Invoke(null, new [] { dictionary, newContents });
			}

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

			if (vpResult != null)
			{
				// This is the only new line. We store the comma-separated string of arbitrary indexes 
				// in modelstate. This way UI helpers can reach through the CollectionIndexStore and 
				// the ExportModelStateToTempData and ImportModelStateFromTempData are working out of the box.
				// This makes validation in the framework work correctly in both re-rendering
				// and PRG pattern re-rendering scenarioes.
				bindingContext.ModelState.SetModelValue(indexKey, vpResult);

				var indexesArray = vpResult.ConvertTo(typeof(string[])) as string[];
				if (indexesArray != null)
				{
					return indexesArray;
				}
			}

			return null;
		}
	}
}