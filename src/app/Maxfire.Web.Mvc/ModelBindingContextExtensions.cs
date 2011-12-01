using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Maxfire.Web.Mvc
{
	public static class ModelBindingContextExtensions
	{
		public static bool IsRequiredRouteValue(this ModelBindingContext bindingContext, string value)
		{
			return GetRequiredRouteValues(bindingContext)
				.Any(s => value.Equals(s, StringComparison.OrdinalIgnoreCase));
		}

		private static IEnumerable<string> GetRequiredRouteValues(ModelBindingContext bindingContext)
		{
			yield return "area";
			yield return "controller";
			yield return "action";
			foreach (string requiredParam in bindingContext.ModelMetadata.GetRequiredParameterNames())
			{
				yield return requiredParam;
			}
		}
	}
}