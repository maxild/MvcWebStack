using System;
using System.Collections.Generic;
using System.Linq;

namespace Maxfire.Core.Extensions
{
	public static class EnumerableExtensions
	{
		public static int IndexOf<TItem>(this IEnumerable<TItem> collection, Func<TItem, bool> predicate)
		{
			var y = collection.Select((item, index) => new { Index = index, Item = item })
				.Where(x => predicate(x.Item))
				.FirstOrDefault();
			return y != null ? y.Index : -1;
		}
	}
}