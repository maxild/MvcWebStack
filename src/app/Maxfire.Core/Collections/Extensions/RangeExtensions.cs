using System;
using System.Collections.Generic;

namespace Maxfire.Core.Collections.Extensions
{
	public static class RangeExtensions
	{
		public static Range<TDest> Convert<TSource, TDest>(this Range<TSource> range, Func<TSource, TDest> selector)
		{
			return new Range<TDest>(selector(range.Start), selector(range.End), Comparer<TDest>.Default, range.IncludesStart, range.IncludesEnd);
		}
	}
}