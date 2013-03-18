using System.ComponentModel;
using System.Web.Mvc;

namespace Maxfire.Web.Mvc.UnitTests
{
	public class TestableDefaultModelBinder : BetterDefaultModelBinder
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

		public new virtual void BindComplexElementalModel(ControllerContext controllerContext, ModelBindingContext bindingContext, object model)
		{
			base.BindComplexElementalModel(controllerContext, bindingContext, model);
		}

		public new virtual PropertyDescriptorCollection GetModelProperties(ControllerContext controllerContext,
		                                                                   ModelBindingContext bindingContext)
		{
			return base.GetModelProperties(controllerContext, bindingContext);
		}

		public new virtual void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext,
		                                     PropertyDescriptor propertyDescriptor)
		{
			base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
		}

		public new virtual object BindComplexModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			return base.BindComplexModel(controllerContext, bindingContext);
		}
	}
}