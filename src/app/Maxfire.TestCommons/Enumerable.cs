using System.Collections.Generic;
using System.Linq;

namespace Maxfire.TestCommons
{
	public static class Enumerable<T>
	{
		public static IEnumerable<T> Empty()
		{
			return Enumerable.Empty<T>();
		}

		public static IEnumerable<T> WithItems(params T[] items)
		{
			foreach (var item in items)
			{
				yield return item;
			}
		}
	}
}