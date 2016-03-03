using System.Web.Mvc;

namespace Maxfire.Web.Mvc
{
	public static class ModelStateExtensions
	{
		public static bool IsInvalid(this ModelState modelState)
		{
			return (modelState.Errors != null && modelState.Errors.Count > 0);
		}
	}
}