using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;

namespace Maxfire.Web.Mvc.UnitTests
{
	public class TestableExtensibleDefaultModelBinder : ExtensibleDefaultModelBinder
	{
		private ModelBinderDictionary _binders;
		public ModelBinderDictionary Binders
		{
			get { return _binders ?? (_binders = new ModelBinderDictionary()); }
			set { _binders = value; }
		}

		protected override IModelBinder GetBinder(System.Type modelType)
		{
			return Binders.GetBinder(modelType) ?? base.GetBinder(modelType);
		}

		public virtual object PublicBindSimpleModel(ControllerContext controllerContext, ModelBindingContext bindingContext, ValueProviderResult valueProviderResult)
		{
			return base.BindSimpleModel(controllerContext, bindingContext, valueProviderResult);
		}

		protected override object BindSimpleModel(ControllerContext controllerContext, ModelBindingContext bindingContext, ValueProviderResult valueProviderResult)
		{
			return PublicBindSimpleModel(controllerContext, bindingContext, valueProviderResult);
		}

		public virtual void PublicBindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor property)
		{
			base.BindProperty(controllerContext, bindingContext, property);
		}

		protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor property)
		{
			PublicBindProperty(controllerContext, bindingContext, property);
		}

		public virtual void PublicBindComplexElementalModel(ControllerContext controllerContext, ModelBindingContext bindingContext, object model)
		{
			base.BindComplexElementalModel(controllerContext, bindingContext, model);
		}

		protected override void BindComplexElementalModel(ControllerContext controllerContext, ModelBindingContext bindingContext, object model)
		{
			PublicBindComplexElementalModel(controllerContext, bindingContext, model);
		}

		public virtual PropertyDescriptorCollection PublicGetModelProperties(ControllerContext controllerContext,
		                                                                   ModelBindingContext bindingContext)
		{
			return base.GetModelProperties(controllerContext, bindingContext);
		}

		protected override PropertyDescriptorCollection GetModelProperties(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			return PublicGetModelProperties(controllerContext, bindingContext);
		}

		public virtual object PublicBindComplexModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			return base.BindComplexModel(controllerContext, bindingContext);
		}

		protected override object BindComplexModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			return PublicBindComplexModel(controllerContext, bindingContext);
		}

		public virtual object PublicGetPropertyValue(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, IModelBinder propertyBinder)
		{
			return base.GetPropertyValue(controllerContext, bindingContext, propertyDescriptor, propertyBinder);
		}

		protected override object GetPropertyValue(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, IModelBinder propertyBinder)
		{
			return PublicGetPropertyValue(controllerContext, bindingContext, propertyDescriptor, propertyBinder);
		}

		public virtual bool PublicOnModelUpdating(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			return base.OnModelUpdating(controllerContext, bindingContext);
		}

		protected override bool OnModelUpdating(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			return PublicOnModelUpdating(controllerContext, bindingContext);
		}

		public virtual void PublicOnModelUpdated(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			base.OnModelUpdated(controllerContext, bindingContext);
		}

		protected override void OnModelUpdated(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			PublicOnModelUpdated(controllerContext, bindingContext);
		}

		public virtual bool PublicOnPropertyValidating(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor property, object value)
		{
			return base.OnPropertyValidating(controllerContext, bindingContext, property, value);
		}

		protected override bool OnPropertyValidating(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor property, object value)
		{
			return PublicOnPropertyValidating(controllerContext, bindingContext, property, value);
		}

		public virtual void PublicOnPropertyValidated(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor property, object value)
		{
			base.OnPropertyValidated(controllerContext, bindingContext, property, value);
		}

		protected override void OnPropertyValidated(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor property, object value)
		{
			PublicOnPropertyValidated(controllerContext, bindingContext, property, value);
		}

		public virtual void PublicSetProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor property, object value)
		{
			base.SetProperty(controllerContext, bindingContext, property, value);
		}

		protected override void SetProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor property, object value)
		{
			PublicSetProperty(controllerContext, bindingContext, property, value);
		}

		public virtual ModelBindingContext PublicCreateComplexElementalModelBindingContext(ControllerContext controllerContext, ModelBindingContext bindingContext, object model)
		{
			return base.CreateComplexElementalModelBindingContext(controllerContext, bindingContext, model);
		}

		protected override ModelBindingContext CreateComplexElementalModelBindingContext(ControllerContext controllerContext, ModelBindingContext bindingContext, object model)
		{
			return PublicCreateComplexElementalModelBindingContext(controllerContext, bindingContext, model);
		}

		public virtual object PublicCreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, System.Type modelType)
		{
			return base.CreateModel(controllerContext, bindingContext, modelType);
		}

		protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, System.Type modelType)
		{
			return PublicCreateModel(controllerContext, bindingContext, modelType);
		}

		public string PublicCreateSubIndexName(string prefix, int indexName)
		{
			return CreateSubIndexName(prefix, indexName);
		}

		public string PublicCreateSubPropertyName(string prefix, string propertyName)
		{
			return CreateSubPropertyName(prefix, propertyName);
		}

		public IEnumerable<PropertyDescriptor> PublicGetFilteredModelProperties(ControllerContext controllerContext,
		                                                                     ModelBindingContext bindingContext)
		{
			return GetFilteredModelProperties(controllerContext, bindingContext);
		}

		public virtual object PublicUpdateCollection(ControllerContext controllerContext, ModelBindingContext bindingContext, System.Type elementType)
		{
			return base.UpdateCollection(controllerContext, bindingContext, elementType);
		}

		protected override object UpdateCollection(ControllerContext controllerContext, ModelBindingContext bindingContext, System.Type elementType)
		{
			return PublicUpdateCollection(controllerContext, bindingContext, elementType);
		}

		public virtual object PublicUpdateDictionary(ControllerContext controllerContext, ModelBindingContext bindingContext, System.Type keyType, System.Type valueType)
		{
			return base.UpdateDictionary(controllerContext, bindingContext, keyType, valueType);
		}

		protected override object UpdateDictionary(ControllerContext controllerContext, ModelBindingContext bindingContext, System.Type keyType, System.Type valueType)
		{
			return PublicUpdateDictionary(controllerContext, bindingContext, keyType, valueType);
		}
	}
}