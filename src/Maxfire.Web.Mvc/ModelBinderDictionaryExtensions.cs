using System;
using System.Web.Mvc;

namespace Maxfire.Web.Mvc
{
	public static class ModelBinderDictionaryExtensions
	{
		/// <summary>
		/// Get model binder with _no_ fallback to default model binder
		/// </summary>
		public static IModelBinder GetNonDefaultBinder(this ModelBinderDictionary binders, Type modelType)
		{
			return binders.GetBinder(modelType, false);
		}

		/// <summary>
		/// Get model binder with _no_ fallback to default model binder
		/// </summary>
		public static IModelBinder GetNonDefaultBinderFor<TModel>(this ModelBinderDictionary binders)
		{
			return binders.GetBinder(typeof(TModel), false);
		}

		/// <summary>
		/// Get model binder with fallback to default model binder
		/// </summary>
		public static IModelBinder GetBinderFor<TModel>(this ModelBinderDictionary binders)
		{
			return binders.GetBinder(typeof(TModel));
		}
	}
}