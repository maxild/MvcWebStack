using System;
using System.Globalization;
using System.Reflection;
using System.Web.Mvc;

namespace Maxfire.Web.Mvc.ValueProviders
{
	// Note: ChildActionValueProvider is sealed
	public sealed class BetterChildActionValueProvider : BetterDictionaryValueProvider<object>
	{
		public BetterChildActionValueProvider(ControllerContext controllerContext)
			: base(controllerContext.RouteData.Values, CultureInfo.InvariantCulture)
		{
		}

		// Note: ChildActionValueProvider.ChildActionValuesKey is internal
		private static string ChildActionValuesKey
		{
			get
			{
				// This will only run in full trust
				FieldInfo fieldInfo = typeof (ChildActionValueProvider).GetField("_childActionValuesKey", BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Static);
				return fieldInfo != null ? (string) fieldInfo.GetValue(null) : string.Empty;
			}
		}

		public override ValueProviderResult GetValue(string key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}

			ValueProviderResult explicitValues = base.GetValue(ChildActionValuesKey);
			if (explicitValues != null)
			{
				var rawExplicitValues = explicitValues.RawValue as DictionaryValueProvider<object>;
				if (rawExplicitValues != null)
				{
					return rawExplicitValues.GetValue(key);
				}
			}

			return null;
		}
	}
}