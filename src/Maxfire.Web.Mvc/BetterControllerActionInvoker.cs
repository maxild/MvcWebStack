using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Maxfire.Prelude.Linq;

namespace Maxfire.Web.Mvc
{
	public class BetterControllerActionInvoker : ControllerActionInvoker
	{
		private readonly IValueProviderFactory _valueProviderFactory;

		public BetterControllerActionInvoker()
			: this(null)
		{
		}

		public BetterControllerActionInvoker(IValueProviderFactory valueProviderFactory)
		{
			_valueProviderFactory = valueProviderFactory;
		}

		private IValueProvider GetValueProvider(ControllerContext controllerContext)
		{
			return _valueProviderFactory != null
				       ? _valueProviderFactory.GetValueProvider(controllerContext)
				       : controllerContext.Controller.ValueProvider;
		}

		protected override IDictionary<string, object> GetParameterValues(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
		{
			ParameterDescriptor[] parameterDescriptors = actionDescriptor.GetParameters();
			IEnumerable<string> requiredParameterNames = GetRequiredParameterNames(parameterDescriptors);

			var parameterValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

			foreach (ParameterDescriptor parameterDescriptor in parameterDescriptors)
			{
			    // ReSharper disable once PossibleMultipleEnumeration
			    parameterValues[parameterDescriptor.ParameterName] = GetParameterValue(controllerContext, parameterDescriptor, requiredParameterNames);
			}

			return parameterValues;
		}

		private object GetParameterValue(ControllerContext controllerContext, ParameterDescriptor parameterDescriptor, IEnumerable<string> requiredParameterNames)
		{
			ModelMetadata modelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, parameterDescriptor.ParameterType);

			string[] requiredParameterNamesExceptOwnParameterName = requiredParameterNames
				.Where(name => false == name.Equals(parameterDescriptor.ParameterName, StringComparison.OrdinalIgnoreCase))
				.ToArray();

			modelMetadata.SetRequiredParameterNames(requiredParameterNamesExceptOwnParameterName);

			var bindingContext = new ModelBindingContext
			{
				FallbackToEmptyPrefix = (parameterDescriptor.BindingInfo.Prefix == null), // only fall back if prefix not specified
				ModelMetadata = modelMetadata,
				ModelName = parameterDescriptor.BindingInfo.Prefix ?? parameterDescriptor.ParameterName,
				ModelState = controllerContext.Controller.ViewData.ModelState,
				PropertyFilter = GetPropertyFilter(parameterDescriptor),
				ValueProvider = GetValueProvider(controllerContext)
			};

			IModelBinder binder = GetModelBinder(parameterDescriptor);

			object result = binder.BindModel(controllerContext, bindingContext);

			return result ?? parameterDescriptor.DefaultValue;
		}

		private static IEnumerable<string> GetRequiredParameterNames(IEnumerable<ParameterDescriptor> parameterDescriptors)
		{
			return parameterDescriptors
				.Where(p => false == p.IsDefined(typeof(NonRequiredAttribute), false))
				.Map(p => p.ParameterName);
		}

		private IModelBinder GetModelBinder(ParameterDescriptor parameterDescriptor)
		{
			// look on the parameter itself, then look in the global table
			return parameterDescriptor.BindingInfo.Binder ?? Binders.GetBinder(parameterDescriptor.ParameterType);
		}

		private static Predicate<string> GetPropertyFilter(ParameterDescriptor parameterDescriptor)
		{
			ParameterBindingInfo bindingInfo = parameterDescriptor.BindingInfo;
			return propertyName => IsPropertyAllowed(propertyName, bindingInfo.Include.ToArray(), bindingInfo.Exclude.ToArray());
		}

		static bool IsPropertyAllowed(string propertyName, string[] includeProperties, string[] excludeProperties)
		{
			// We allow a property to be bound if its both in the include list AND not in the exclude list.
			// An empty include list implies all properties are allowed.
			// An empty exclude list implies no properties are disallowed.
			bool includeProperty = (includeProperties == null) || (includeProperties.Length == 0) || includeProperties.Contains(propertyName, StringComparer.OrdinalIgnoreCase);
			bool excludeProperty = (excludeProperties != null) && excludeProperties.Contains(propertyName, StringComparer.OrdinalIgnoreCase);
			return includeProperty && !excludeProperty;
		}
	}
}
