using System;
using System.Web.Mvc;

namespace Maxfire.Web.Mvc.TestCommons.AssertExtensions
{
	public static class ActionResultExtensions
	{
		public static T AssertResultIs<T>(this ActionResult result) where T : ActionResult
		{
			if (result == null)
				throw new ArgumentNullException("result");

			var converted = result as T;

			if (converted == null)
				throw new ActionResultAssertionException(
					string.Format("Expected result to be of type {0}. It is actually of type {1}.", typeof(T).Name, result.GetType().Name));

			return converted;
		}

		public static ViewResult ForView(this ViewResult result, string viewName)
		{
			if (result.ViewName != viewName)
			{
				throw new ActionResultAssertionException(string.Format("Expected view name '{0}', actual was '{1}'", viewName, result.ViewName));
			}
			return result;
		}

		public static TModel WithModel<TModel>(this JsonResult jsonResult)
		{
			return withModel<JsonResult, TModel>(jsonResult, result => result.Data);
		}

		public static TModel WithModel<TModel>(this ViewResult viewResult)
		{
			return withModel<ViewResult, TModel>(viewResult, result => result.ViewData.Model);
		}

		private static TModel withModel<TActionResult, TModel>(TActionResult result, Func<TActionResult, object> modelAccessor)
		{
			object actualModel = modelAccessor(result);
			var expectedType = typeof(TModel);

			if (actualModel == null && expectedType.IsValueType)
			{
				throw new ActionResultAssertionException(
					string.Format("Expected model is a value type of type '{0}', but actual model is NULL.",
								  expectedType.Name));
			}

			if (actualModel == null)
			{
				return default(TModel);
			}

			if (!expectedType.IsAssignableFrom(actualModel.GetType()))
			{
				throw new ActionResultAssertionException(string.Format("Expected model type is '{0}', actual model type was '{1}'.",
																	   expectedType.Name, actualModel.GetType().Name));
			}

			return (TModel)actualModel;
		}

		public class ActionResultAssertionException : Exception
		{
			public ActionResultAssertionException(string message)
				: base(message)
			{
			}
		}
	}
}