using System;
using System.Web.Mvc;

namespace Maxfire.Web.Mvc
{
	public class NullableModelBinderWrapper : NullableModelBinder
	{
		private readonly IModelBinder _modelBinder;

		public NullableModelBinderWrapper(IModelBinder modelBinder)
		{
			if (modelBinder == null)
			{
				throw new ArgumentNullException("modelBinder");
			}
			_modelBinder = modelBinder;
		}

		protected override object BindModelCore(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			return _modelBinder.BindModel(controllerContext, bindingContext);
		}
	}
}