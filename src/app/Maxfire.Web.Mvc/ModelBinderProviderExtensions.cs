using System.Web.Mvc;

namespace Maxfire.Web.Mvc
{
	public static class ModelBinderProviderExtensions
	{
		public static IModelBinder GetBinderFor<TModel>(this IModelBinderProvider provider)
		{
			return provider.GetBinder(typeof(TModel));
		}
	}
}