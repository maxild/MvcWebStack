using System;

namespace Maxfire.Web.Mvc.AutoMapper
{
	public class DateFormatter : ConventionsValueFormatter<DateTime>
	{
		protected override string FormatValueCore(DateTime value)
		{
			return Conventions.FormatDate(value);
		}
	}
}