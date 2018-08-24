using System;
using System.Collections.Generic;
using System.Linq;
using Maxfire.Prelude.Linq;

namespace Maxfire.Core.Extensions
{
	public static class EnumerableExtensions
	{
		public static bool IsSingular<T>(this IEnumerable<T> iterator)
		{
			return iterator.HasCountEqualTo(1);
		}

		public static bool IsNotSingular<T>(this IEnumerable<T> iterator)
		{
			return !iterator.IsSingular();
		}

		public static bool HasCountEqualTo<T>(this IEnumerable<T> iterator, int expectedCount)
		{
			if (iterator == null)
			{
				throw new ArgumentNullException(nameof(iterator));
			}
			if (expectedCount < 0)
			{
				return false;
			}

		    if (iterator is IList<T> list)
			{
				return list.Count == expectedCount;
			}
			using (IEnumerator<T> enumerator = iterator.GetEnumerator())
			{
				int count = 0;
				while (enumerator.MoveNext() && count <= expectedCount)
				{
					count += 1;
				}
				return count == expectedCount && !enumerator.MoveNext();
			}
		}

		public static bool IsEmpty<T>(this IEnumerable<T> iterator)
		{
			return !iterator.IsNotEmpty();
		}

		public static bool IsNotEmpty<T>(this IEnumerable<T> iterator)
		{
			return iterator != null && iterator.Any();
		}

		public static bool HasCountGreaterThan<T>(this IEnumerable<T> iterator, int expectedCount)
		{
			if (iterator == null)
			{
				throw new ArgumentNullException(nameof(iterator));
			}
			if (expectedCount < 0)
			{
				return true;
			}

		    if (iterator is IList<T> list)
			{
				return list.Count > expectedCount;
			}
			using (IEnumerator<T> enumerator = iterator.GetEnumerator())
			{
				int count = 0;
				while (enumerator.MoveNext() && count <= expectedCount)
				{
					count += 1;
				}
				return count > expectedCount;
			}
		}

		public static T[] AsArrayOfSize<T>(this T value, int size)
		{
			var array = new T[size];
			for (int i = 0; i < size; i++)
			{
				array[i] = value;
			}
			return array;
		}

		public static int IndexOf<TItem>(this IEnumerable<TItem> collection, Func<TItem, bool> predicate)
		{
			var y = collection
				.Map((item, index) => new { Index = index, Item = item })
				.FirstOrDefault(x => predicate(x.Item));
			return y?.Index ?? -1;
		}
	}
}
