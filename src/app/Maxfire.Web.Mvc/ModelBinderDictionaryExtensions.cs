using System.Web.Mvc;

namespace Maxfire.Web.Mvc
{
	public static class ModelBinderDictionaryExtensions
	{
		public static IModelBinder GetBinderFor<TModel>(this ModelBinderDictionary binders)
		{
			return binders.GetBinder(typeof(TModel));
		}
	}
}