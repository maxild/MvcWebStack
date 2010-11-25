using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using Maxfire.Core.Extensions;
using Maxfire.Core.Reflection;

namespace Maxfire.Web.Mvc
{
	public static class ModelStateDictionaryExtensions
	{
		public static bool IsInvalid(this ModelStateDictionary modelStateDictionary)
		{
			return !modelStateDictionary.IsValid;
		}

		public static void AddValidationErrors(this ModelStateDictionary state, IDictionary<string, string[]> validationErrors)
		{
			state.AddValidationErrors(validationErrors, string.Empty);
		}

		public static void AddValidationErrors(this ModelStateDictionary state, IDictionary<string, string[]> validationErrors, string prefix)
		{
			foreach (string key in validationErrors.Keys)
			{
				foreach (string errorMessage in validationErrors[key])
				{
					string prefixedKey = key;
					if (prefix.IsNotEmpty())
					{
						prefixedKey = prefix + "." + key;
					}
					// Todo: Maybe both key, value and errormessage should known here suvh that SetModelValue can be called
					state.AddModelError(prefixedKey, errorMessage);
				}
			}
		}

		public static IDictionary<string, string[]> GetValidationErrors(this ModelStateDictionary modelStateDictionary)
		{
			Dictionary<string, string[]> validationErrors = new Dictionary<string, string[]>();

			if (modelStateDictionary != null)
			{
				foreach (var kvp in modelStateDictionary)
				{
					var modelState = kvp.Value;
					if (modelState.Errors != null)
					{
						List<string> messages = new List<string>();
						modelState.Errors.Each(modelError => messages.Add(modelError.ErrorMessage));
						if (messages.Count > 0)
						{
							//var id = kvp.Key.FormatAsHtmlId();
							validationErrors.Add(kvp.Key, messages.ToArray());
						}
					}
				}
			}

			return validationErrors;
		}

		public static void AddModelErrorFor<TInputModel>(this ModelStateDictionary modelStateDictionary, Expression<Func<TInputModel, object>> expression, string errorMessage)
			where TInputModel : class
		{
			var name = expression.GetNameFor();
			modelStateDictionary.AddModelError(name, errorMessage);
		}
	}
}