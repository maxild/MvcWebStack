using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using Maxfire.Core.Extensions;
using Maxfire.Core.Reflection;

namespace Maxfire.Web.Mvc
{
	public static class ControllerExtensions
	{
		public static bool IsInputValid(this Controller controller)
		{
			return controller.ModelState.IsValid;
		}

		public static bool IsInputInvalid(this Controller controller)
		{
			return !controller.ModelState.IsValid;
		}

		public static void AddModelErrorFor<TViewModel, TId>(this IRestfulController<TViewModel, TId> controller, Expression<Func<TViewModel, object>> modelProperty, string errorMessage)
			where TViewModel : class, IEntityViewModel<TId>
		{
			string name = controller.getModelName(modelProperty);
			controller.ModelState.AddModelError(name, errorMessage);
		}

		public static ModelState GetModelStateFor<TViewModel, TId>(this IRestfulController<TViewModel, TId> controller, Expression<Func<TViewModel, object>> modelProperty)
			where TViewModel : class, IEntityViewModel<TId>
		{
			string name = controller.getModelName(modelProperty);
			ModelState modelState;
			return controller.ModelState.TryGetValue(name, out modelState) ? modelState : null;
		}

		public static bool IsInputInvalidFor<TViewModel, TId>(this IRestfulController<TViewModel, TId> controller, Expression<Func<TViewModel, object>> expression)
			where TViewModel: class, IEntityViewModel<TId>
		{
			ModelState modelState = controller.GetModelStateFor(expression);
			return (modelState != null && modelState.Errors != null && modelState.Errors.Count > 0);
		}

		public static bool IsInputValidFor<TViewModel, TId>(this IRestfulController<TViewModel, TId> controller, Expression<Func<TViewModel, object>> expression)
			where TViewModel : class, IEntityViewModel<TId>
		{
			return !controller.IsInputInvalidFor(expression);
		}

		private static string getModelName<TViewModel, TId>(this IRestfulController<TViewModel, TId> controller, Expression<Func<TViewModel, object>> modelProperty)
			where TViewModel : class, IEntityViewModel<TId>
		{
			string name = modelProperty.GetNameFor();
			if (controller.BindingPrefix.IsNotEmpty())
			{
				name = controller.BindingPrefix + "." + name;
			}
			return name;
		}

		/// <summary>
		/// Redirects to an action on the same controller using expression-based syntax
		/// </summary>
		public static ActionResult RedirectToAction<TController>(this TController controller, Expression<Action<TController>> action)
			where TController : OpinionatedController
		{
			return ((OpinionatedController)controller).RedirectToAction(action);
		}

		/// <summary>
		/// Redirects to an action on the same or another controller using expression-based syntax
		/// </summary>
		public static ActionResult RedirectToAction<TController>(this OpinionatedController controller, Expression<Action<TController>> action)
			where TController : OpinionatedController
		{
			// When calling RedirectToAction from a controller super layer type this is necessary, because otherwise
			// the name of the controller will be resolved as typeof(TController).Name, and the name of the super 
			// layer type is no good.
			string controllerName = controller.GetType().Name;
			if (controller.IsAjaxRequest)
			{
				string url = controller.UrlFor(action, controllerName);
				var data = JsonUtil.Message(new { redirectUrl = url });
				return JsonUtil.JsonNetResult(data);
			}
			var routeValues = RouteValuesHelper.GetRouteValuesFromExpression(action, controllerName);
			return new RedirectToRouteResult(routeValues);
		}
	}
}