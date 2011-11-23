using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using Maxfire.Core.Extensions;

namespace Maxfire.Web.Mvc
{
	public class DateModelBinder : SimpleNameValueSerializer<DateTime>, IModelBinder
	{
		private readonly CultureInfo _culture;

		public DateModelBinder(CultureInfo culture = null)
		{
			_culture = culture ?? CultureInfo.InvariantCulture;
		}

		public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			if (bindingContext == null)
			{
				throw new ArgumentNullException("bindingContext");
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

			// If the prefix is present among the values all parts are required

			int? year = GetPart(bindingContext, Year, y => y > 0, "Værdien '{0}' er ikke et validt år.");
			int? month = GetPart(bindingContext, Month, m => 1 <= m && m <= 12, "Værdien '{0}' er ikke en valid måned.");

			if (month == null || year == null)
			{
				if (month != null)
				{
					// We validate the day component using the given month in a leap year (31)
					GetPart(bindingContext, Day, d => 1 <= d && d <= DateTime.DaysInMonth(2012, month.Value),
							"Værdien '{0}' er ikke en valid dag for den valgte måned.");
				}
				else
				{
					// We validate the day component using the largest possible day in any month or year (31)
					GetPart(bindingContext, Day, d => 1 <= d && d <= 31,
							"Værdien '{0}' er ikke en valid dag for den valgte måned.");
				}
				return null;
			}

			// We validate the day component using the valid year and month
			int? day = GetPart(bindingContext, Day,
				d => 1 <= d && d <= DateTime.DaysInMonth(year.Value, month.Value),
				"Værdien '{0}' er ikke en valid dag for den valgte måned.");

			if (day == null)
			{
				return null;
			}

			return new DateTime(year.Value, month.Value, day.Value);
		}

		private DateTime? GetDateTime(ModelBindingContext bindingContext)
		{
			ValueProviderResult valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
			if (valueResult == null)
			{
				return null;
			}

			bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueResult);
			try
			{
				return (DateTime?)valueResult.ConvertTo(typeof(DateTime?), _culture);
			}
			catch (Exception ex)
			{
				bindingContext.ModelState.AddModelError(bindingContext.ModelName, ex);
				return null;
			}
		}

		private static int? GetPart(ModelBindingContext bindingContext, string key, Func<int, bool> validator, string errorMessage)
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

			if (valueResult == null || string.IsNullOrWhiteSpace(valueResult.AttemptedValue))
			{
				bindingContext.ModelState.AddModelError(name, "Værdien for '{0}' mangler.".FormatWith(name));
				return null;
			}

			int value;
			try
			{
				value = (int)valueResult.ConvertTo(typeof(int));
			}
			catch (Exception ex)
			{
				bindingContext.ModelState.AddModelError(name, ex);
				return null;
			}

			if (validator(value) == false)
			{
				bindingContext.ModelState.AddModelError(name, errorMessage.FormatWith(value));
				return null;
			}

			return value;
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
					new Dictionary<string, object> { { Day, value.Day.ToString() }, { Month, value.Month.ToString() }, { Year, value.Year.ToString() } } :
					new Dictionary<string, object> { { prefix + "." + Day, value.Day.ToString() }, { prefix + "." + Month, value.Month.ToString() }, { prefix + "." + Year, value.Year.ToString() } };
		}
	}

	public class DateFromPartsAttribute : CustomModelBinderAttribute
	{
		/// <summary>
		/// The key used for the day part (as in 'birthday.Day')
		/// </summary>
		public string Day { get; set; }

		/// <summary>
		/// The key used for the month part (as in 'birthday.Month')
		/// </summary>
		public string Month { get; set; }

		/// <summary>
		/// The key used for the year part (as in 'birthday.Year')
		/// </summary>
		public string Year { get; set; }

		private IModelBinder _binder;
		public override IModelBinder GetBinder()
		{
			return _binder ?? (_binder = new DateModelBinder { Year = Year, Month = Month, Day = Day });
		}
	}
}