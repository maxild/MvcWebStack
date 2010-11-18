namespace Maxfire.Web.Mvc
{
	public static class NameValueSerializerProviderExtensions
	{
		public static INameValueSerializer GetSerializerFor<TModel>(this INameValueSerializerProvider provider)
		{
			return provider.GetSerializer(typeof (TModel));
		}
	}
}