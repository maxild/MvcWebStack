using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Maxfire.Core.Extensions;
using Maxfire.Prelude.Linq;

namespace Maxfire.Web.Mvc
{
	public class DateModelBinder : SimpleNameValueSerializer<DateTime>, IModelBinder
	{
		public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			if (bindingContext == null)
			{
				throw new ArgumentNullException(nameof(bindingContext));
			}

			if (bindingContext.ValueProvider.ContainsPrefix(bindingContext.ModelName) == false)
			{
				return null;
			}

			DateTime? dateTimeAttempt = GetDateTime(bindingContext);
			if (dateTimeAttempt != null)
			{
				return dateTimeAttempt.Value;
			}

			// Missing values (parts) will show up as entries in the dictionary where kvp.Value == null
			IDictionary<string, ValueProviderResult> parts = GetParts(bindingContext, Year, Month, Day);

			// We do not differentiate between being missing (not being posted) and
			// being empty (posted as an empty value). This way a set of partial request
			// params (e.g. 'birthday.day' and 'birthday.month') will bind to a nullable
			// DateTime without any errors, if the values of the partial params are empty.
			if (parts.Values.Any(IsMissingOrEmpty))
			{
				return null;
			}

			// All validation errors are reported back in ModelState under ModelName (i.e. 'udbdato', not 'udbdato.Day' etc)
			// This way the view can use the ValidationMessage html helper after a DateSelects element. Only disadvantage is
			// that all selects are colored red, because we do not differentiate between which of the date components (day/month/year)
			// that was invalid.

			bool nullResult = false;

			var values = GetValues(bindingContext, parts);

			if (values[0] != null && values[0] <= 0)
			{
				bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Værdien '{0}' er ikke et validt år.".FormatWith(values[0]));
				nullResult = true;
			}

			if (values[1] != null && (values[1] < 1 || values[1] > 12))
			{
				bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Værdien '{0}' er ikke en valid måned.".FormatWith(values[1]));
				nullResult = true;
			}

			if (values[2] != null && (values[2] < 1 || values[2] > DateTime.DaysInMonth(values[0] ?? 2012, values[1] ?? 1)))
			{
				bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Værdien '{0}' er ikke en valid dag for den valgte måned.".FormatWith(values[2]));
				nullResult = true;
			}

			if (nullResult || values.Any(val => val == null))
			{
				return null;
			}

			return new DateTime(values[0].GetValueOrDefault(), values[1].GetValueOrDefault(), values[2].GetValueOrDefault());
		}

		private static DateTime? GetDateTime(ModelBindingContext bindingContext)
		{
			ValueProviderResult valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
			if (valueResult == null)
			{
				return null;
			}

			string valueAsString = valueResult.ConvertTo<string>();
			if (valueAsString.IsEmpty())
			{
				return null;
			}

			bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueResult);
			try
			{
				return DateTime.ParseExact(valueAsString,
				                           new[] { @"d\/M-yyyy", @"dd\/MM-yyyy", "yyyy-MM-dd", "d-M-yyyy", "dd-MM-yyyy"},
										   CultureInfo.InvariantCulture,
				                           DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite);
			}
			catch (Exception ex)
			{
				bindingContext.ModelState.AddModelError(bindingContext.ModelName, ex);
				return null;
			}
		}

		private static IDictionary<string, ValueProviderResult> GetParts(ModelBindingContext bindingContext, params string[] keys)
		{
			return keys.Map(key => GetPart(bindingContext, key)).ToDictionary(tuple => tuple.Item1, tuple => tuple.Item2);
		}

		private static Tuple<string, ValueProviderResult> GetPart(ModelBindingContext bindingContext, string key)
		{
			string name = bindingContext.ModelName + "." + key;
			ValueProviderResult valueResult = bindingContext.ValueProvider.GetValue(name);

			if (valueResult == null && bindingContext.FallbackToEmptyPrefix)
			{
				name = key;
				valueResult = bindingContext.ValueProvider.GetValue(key);
			}

			if (valueResult != null)
			{
				bindingContext.ModelState.SetModelValue(name, valueResult);
			}

			return new Tuple<string, ValueProviderResult>(name, valueResult);
		}

		private static IList<int?> GetValues(ModelBindingContext bindingContext, IEnumerable<KeyValuePair<string, ValueProviderResult>> parts)
		{
			return parts.Map(kvp => GetValue(bindingContext, kvp.Key, kvp.Value)).ToList();
		}

		private static int? GetValue(ModelBindingContext bindingContext, string key, ValueProviderResult valueResult)
		{
			int value;
			try
			{
				value = (int)valueResult.ConvertTo(typeof(int));
			}
			catch (Exception ex)
			{
				bindingContext.ModelState.AddModelError(key, ex);
				return null;
			}

			return value;
		}

		private static bool IsMissingOrEmpty(ValueProviderResult valueResult)
		{
			return valueResult == null || string.IsNullOrWhiteSpace(valueResult.AttemptedValue);
		}

		private string _day;
		public string Day
		{
			get { return _day ?? "Day"; }
			set { _day = value; }
		}

		private string _month;
		public string Month
		{
			get { return _month ?? "Month"; }
			set { _month = value; }
		}

		private string _year;
		public string Year
		{
			get { return _year ?? "Year"; }
			set { _year = value; }
		}

		protected override IDictionary<string, object> GetValuesCore(DateTime value, string prefix)
		{
			return string.IsNullOrEmpty(prefix) ?
					new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase) { { Day, value.Day.ToString(CultureInfo.InvariantCulture) }, { Month, value.Month.ToString(CultureInfo.InvariantCulture) }, { Year, value.Year.ToString(CultureInfo.InvariantCulture) } } :
					new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase) { { prefix + "." + Day, value.Day.ToString(CultureInfo.InvariantCulture) }, { prefix + "." + Month, value.Month.ToString(CultureInfo.InvariantCulture) }, { prefix + "." + Year, value.Year.ToString(CultureInfo.InvariantCulture) } };
		}
	}

	public class DateFromPartsAttribute : CustomModelBinderAttribute
	{
		/// <summary>
		/// The key used for the day part (as in 'birthday.Day')
		/// </summary>
// ReSharper disable UnusedAutoPropertyAccessor.Global
		public string Day { get; set; }

		/// <summary>
		/// The key used for the month part (as in 'birthday.Month')
		/// </summary>
		public string Month { get; set; }

		/// <summary>
		/// The key used for the year part (as in 'birthday.Year')
		/// </summary>
		public string Year { get; set; }
// ReSharper restore UnusedAutoPropertyAccessor.Global

		private IModelBinder _binder;
		public override IModelBinder GetBinder()
		{
			return _binder ?? (_binder = new DateModelBinder { Year = Year, Month = Month, Day = Day });
		}
	}
}
