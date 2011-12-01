using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Maxfire.Web.Mvc
{
	internal static class ModelMetadataExtensions
	{
		const string REQUIRED_PARAMS = "RequiredParams";

		public static IEnumerable<string> GetRequiredParameterNames(this ModelMetadata modelMetadata)
		{
			IEnumerable<string> requiredParams = null;
			if (modelMetadata.AdditionalValues.ContainsKey(REQUIRED_PARAMS))
			{
				requiredParams = modelMetadata.AdditionalValues[REQUIRED_PARAMS] as IEnumerable<string>;
			}
			return requiredParams ?? Enumerable.Empty<string>();
		}

		public static void SetRequiredParameterNames(this ModelMetadata modelMetadata, IEnumerable<string> requiredParameterNames)
		{
			modelMetadata.AdditionalValues[REQUIRED_PARAMS] = requiredParameterNames;
		}
	}
}