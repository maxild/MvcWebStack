using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Maxfire.Core.Extensions;

namespace Maxfire.Web.Mvc
{
	public class BetterControllerActionInvoker : ControllerActionInvoker
	{
		protected override IDictionary<string, object> GetParameterValues(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
		{
			Dictionary<string, object> parametersDict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
			ParameterDescriptor[] parameterDescriptors = actionDescriptor.GetParameters();
			IEnumerable<string> nonNullableParameterNames = GetNonNullableParameterNames(parameterDescriptors);

			foreach (ParameterDescriptor parameterDescriptor in parameterDescriptors)
			{
				parametersDict[parameterDescriptor.ParameterName] = GetParameterValue(controllerContext, parameterDescriptor, nonNullableParameterNames);
			}

			return parametersDict;
		}

		private object GetParameterValue(ControllerContext controllerContext, ParameterDescriptor parameterDescriptor, IEnumerable<string> nonNullableParameterNames)
		{
			Type parameterType = parameterDescriptor.ParameterType;
			IModelBinder binder = GetModelBinder(parameterDescriptor);
			IValueProvider valueProvider = controllerContext.Controller.ValueProvider;
			string parameterName = parameterDescriptor.BindingInfo.Prefix ?? parameterDescriptor.ParameterName;
			Predicate<string> propertyFilter = GetPropertyFilter(parameterDescriptor);

			ModelMetadata modelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, parameterType);

			string[] requiredParameterNames = nonNullableParameterNames
				.Where(name => !name.Equals(parameterDescriptor.ParameterName, StringComparison.OrdinalIgnoreCase))
				.ToArray();

			modelMetadata.SetRequiredParameterNames(requiredParameterNames);

			ModelBindingContext bindingContext = new ModelBindingContext
			{
				FallbackToEmptyPrefix = (parameterDescriptor.BindingInfo.Prefix == null),
				ModelMetadata = modelMetadata,
				ModelName = parameterName,
				ModelState = controllerContext.Controller.ViewData.ModelState,
				PropertyFilter = propertyFilter,
				ValueProvider = valueProvider
			};

			object result = binder.BindModel(controllerContext, bindingContext);
			return result ?? parameterDescriptor.DefaultValue;
		}

		private static IEnumerable<string> GetNonNullableParameterNames(IEnumerable<ParameterDescriptor> parameterDescriptors)
		{
			return parameterDescriptors
				.Where(p => p.ParameterType.AllowsNullValue() == false)
				.Select(p => p.ParameterName);
		}

		private IModelBinder GetModelBinder(ParameterDescriptor parameterDescriptor)
		{
			return parameterDescriptor.BindingInfo.Binder ?? Binders.GetBinder(parameterDescriptor.ParameterType);
		}

		private static Predicate<string> GetPropertyFilter(ParameterDescriptor parameterDescriptor)
		{
			ParameterBindingInfo bindingInfo = parameterDescriptor.BindingInfo;
			return propertyName => IsPropertyAllowed(propertyName, bindingInfo.Include.ToArray(), bindingInfo.Exclude.ToArray());
		}

		static bool IsPropertyAllowed(string propertyName, string[] includeProperties, string[] excludeProperties)
		{
			// We allow	a property to be bound if its both in the include list AND not in the exclude list.
			// An empty	include	list implies all properties	are	allowed.
			// An empty	exclude	list implies no	properties are disallowed.
			bool includeProperty = (includeProperties == null) || (includeProperties.Length == 0) || includeProperties.Contains(propertyName, StringComparer.OrdinalIgnoreCase);
			bool excludeProperty = (excludeProperties != null) && excludeProperties.Contains(propertyName, StringComparer.OrdinalIgnoreCase);
			return includeProperty && !excludeProperty;
		}
	}
}