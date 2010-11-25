using System.Web.Mvc;

namespace Maxfire.Web.Mvc
{
	public static class ValueProviderResultExtensions
	{
		public static T ConvertTo<T>(this ValueProviderResult valueProviderResult)
		{
			return (T)valueProviderResult.ConvertTo(typeof (T));
		}
	}
}