using System;

namespace Maxfire.AutoMapper.Web.Mvc.Formatters
{
	public class DateFormatter : ConventionsValueFormatter<DateTime>
	{
		protected override string FormatValueCore(DateTime value)
		{
			return Conventions.FormatDate(value);
		}
	}
}