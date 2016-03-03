using System;
using System.Collections.Generic;
using System.Diagnostics;
using Maxfire.Core.Collections;

namespace Maxfire.Core.Extensions
{
	public static class BasicExtensions
	{
		[DebuggerStepThrough]
		public static void Times(this int maxCount, Action<int> eachAction)
		{
			for (var idx = 0; idx < maxCount; idx++)
			{
				eachAction(idx);
			}
		}

		[DebuggerStepThrough]
		public static void Times(this int maxCount, Action eachAction)
		{
			for (var idx = 0; idx < maxCount; idx++)
			{
				eachAction();
			}
		}

		[DebuggerStepThrough]
		public static void TimesStartingAt(this int maxCount, int startIndex, Action<int> eachAction)
		{
			for (var idx = 0; idx < maxCount; idx++)
			{
				eachAction(idx + startIndex);
			}
		}

		/// <summary>
		/// Creates an inclusive range between two values. The default comparer is used
		/// to compare values.
		/// </summary>
		/// <typeparam name="T">Type of the values</typeparam>
		/// <param name="start">Start of range.</param>
		/// <param name="end">End of range.</param>
		/// <returns>An inclusive range between the start point and the end point.</returns>
		public static Range<T> To<T>(this T start, T end)
		{
			return new Range<T>(start, end);
		}

		public static IEnumerable<int> UpTo(this int start, int end)
		{
			return start.To(end).UpBy(1);
		}

		public static IEnumerable<long> UpTo(this long start, long end)
		{
			return start.To(end).UpBy(1);
		}

		public static IEnumerable<int> DownTo(this int start, int end)
		{
			return start.To(end).DownBy(1);
		}

		public static IEnumerable<long> DownTo(this long start, long end)
		{
			return start.To(end).DownBy(1);
		}
	}
}