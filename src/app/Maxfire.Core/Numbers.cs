using System;
using System.IO;
using System.Linq;

namespace Maxfire.Core
{
	// BetterMath
	public static class Numbers
	{
		public static T Max<T>(T number, params T[] numbers)
		{
			return Find(number, numbers, Operator<T>.GreaterThan);
		}

		public static T Min<T>(T number, params T[] numbers)
		{
			return Find(number, numbers, Operator<T>.LessThan);
		}

		static T Find<T>(T number, T[] numbers, Func<T, T, bool> predicate)
		{
			if (numbers == null)
				return number;

			T elem = number;

			for (int i = 0; i < numbers.Length; i++)
			{
				if (predicate(numbers[i], elem))
				{
					elem = numbers[i];
				}
			}

			return elem;
		}
	}

	public static class BetterPath
	{
		public static string Combine(string path, params string[] paths)
		{
			return paths == null ? path : paths.Aggregate(path, Path.Combine);
		}
	}
}