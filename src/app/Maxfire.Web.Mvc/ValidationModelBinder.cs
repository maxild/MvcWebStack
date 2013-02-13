using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Maxfire.Core.Extensions;

namespace Maxfire.Web.Mvc
{
	public class ValidationModelBinder : DefaultModelBinder
	{
		protected override IModelBinder GetBinder(Type modelType)
		{
			var binder = Binders.GetBinder(modelType, false);
			return binder ?? this;
		}

		protected override void OnModelUpdated(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			foreach (ModelValidator validator in bindingContext.ModelMetadata.GetValidators(controllerContext))
			{
				foreach (ModelValidationResult validationResult in validator.Validate(bindingContext.Model))
				{
					bindingContext.ModelState.AddModelError(
						CreateSubPropertyName(bindingContext.ModelName, validationResult.MemberName), 
						validationResult.Message);
				}
			}
		}

		// TODO: Try to use base version that validates => Should imply too many (duplicate) model errors
		protected override void OnPropertyValidated(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, object value)
		{
			// Q: What about required value logic....?
		}

		protected override IEnumerable<string> GetCollectionIndexes(ModelBindingContext bindingContext)
		{
			string prefix = bindingContext.ModelName;

			var subIndexNames = new List<string>();
			var subIndexPattern = new Regex("^" + Regex.Escape(prefix) + @"\[([^\]]+)\]", RegexOptions.IgnoreCase);

			foreach (var name in bindingContext.ValueProvider.GetKeys())
			{
				var match = subIndexPattern.Match(name);
				if (!match.Success)
				{
					continue;
				}

				var subIndex = match.Groups[1].Value;
				if (!subIndexNames.Contains(subIndex))
				{
					subIndexNames.Add(subIndex);
				}
			}

			return subIndexNames;
		}

		protected override object UpdateDictionary(ControllerContext controllerContext, ModelBindingContext bindingContext, Type keyType, Type valueType)
		{
			if (!keyType.IsSimpleType())
			{
				return null;
			}

			IEnumerable<string> indexes = GetCollectionIndexes(bindingContext);

			if (indexes == null)
			{
				return null;
			}

			IModelBinder elementBinder = Binders.GetBinder(valueType);

			List<KeyValuePair<object, object>> modelList = new List<KeyValuePair<object, object>>();

			foreach (string index in indexes)
			{
				string subIndexKey = CreateSubIndexName(bindingContext.ModelName, index);

				object thisKey = convertCollectionIndex(bindingContext.ModelState, subIndexKey, index, keyType);

				ModelBindingContext innerContext = new ModelBindingContext
				{
					ModelMetadata = GetMetadataForType(null, valueType),
					ModelName = subIndexKey,
					ModelState = bindingContext.ModelState,
					PropertyFilter = bindingContext.PropertyFilter,
					ValueProvider = bindingContext.ValueProvider
				};
				object thisValue = elementBinder.BindModel(controllerContext, innerContext);

				// Note: Uncommented because AddValueRequiredMessageToModelState is private
				//AddValueRequiredMessageToModelState(controllerContext, bindingContext.ModelState, valueFieldKey, valueType, thisValue);
				KeyValuePair<object, object> kvp = new KeyValuePair<object, object>(thisKey, thisValue);
				modelList.Add(kvp);
			}

			if (modelList.Count == 0)
			{
				return null;
			}

			object dictionary = bindingContext.Model;
			CollectionHelpers.ReplaceDictionary(keyType, valueType, dictionary, modelList);
			return dictionary;
		}

		private static object convertCollectionIndex(ModelStateDictionary modelState, string modelStateKey, string index, Type destinationType)
		{
			try
			{
				object convertedValue = TypeExtensions.ConvertSimpleType(CultureInfo.InvariantCulture, index, destinationType);
				return convertedValue;
			}
			catch (Exception ex)
			{
				modelState.AddModelError(modelStateKey, ex);
				return null;
			}
		}
	}
}