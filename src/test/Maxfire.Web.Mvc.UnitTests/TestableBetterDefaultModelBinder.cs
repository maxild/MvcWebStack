using System.Web.Mvc;

namespace Maxfire.Web.Mvc.UnitTests
{
	public class TestableBetterDefaultModelBinder : BetterDefaultModelBinder
	{
		public virtual object PublicUpdateCollection(ControllerContext controllerContext, ModelBindingContext bindingContext, System.Type elementType)
		{
			return base.UpdateCollection(controllerContext, bindingContext, elementType);
		}

		protected override object UpdateCollection(ControllerContext controllerContext, ModelBindingContext bindingContext, System.Type elementType)
		{
			return PublicUpdateCollection(controllerContext, bindingContext, elementType);
		}
	}
}