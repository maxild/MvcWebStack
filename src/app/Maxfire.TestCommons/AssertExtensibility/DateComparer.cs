using System;
using System.Collections.Generic;

namespace Maxfire.TestCommons.AssertExtensibility
{
	public class DateComparer : IComparer<DateTime>
	{
		public int Compare(DateTime lhs,
		                   DateTime rhs)
		{
			if (lhs.Year != rhs.Year)
				return lhs.Year - rhs.Year;

			if (lhs.Month != rhs.Month)
				return lhs.Month - rhs.Month;

			if (lhs.Day != rhs.Day)
				return lhs.Day - rhs.Day;

			return 0;
		}
	}
}