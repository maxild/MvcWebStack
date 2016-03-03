using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
				throw new ArgumentNullException("iterator");
			}
			if (expectedCount < 0)
			{
				return false;
			}
			IList<T> list = iterator as IList<T>;
			if (list != null)
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
				throw new ArgumentNullException("iterator");
			}
			if (expectedCount < 0)
			{
				return true;
			}
			IList<T> list = iterator as IList<T>;
			if (list != null)
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

		[DebuggerStepThrough]
		public static IEnumerable Each(this IEnumerable values, Action<object> eachAction)
		{
// ReSharper disable PossibleMultipleEnumeration
			foreach (var item in values)
			{
				eachAction(item);
			}

			return values;
// ReSharper restore PossibleMultipleEnumeration
		}

		[DebuggerStepThrough]
		public static IEnumerable Each(this IEnumerable values, Action<object, int> eachAction)
		{
			int i = 0;
// ReSharper disable PossibleMultipleEnumeration
			foreach (var item in values)
			{
				eachAction(item, i);
				i++;
			}

			return values;
// ReSharper restore PossibleMultipleEnumeration
		}

		[DebuggerStepThrough]
		public static IEnumerable<T> Each<T>(this IEnumerable<T> values, Action<T> eachAction)
		{
// ReSharper disable PossibleMultipleEnumeration
			foreach (var item in values)
			{
				eachAction(item);
			}

			return values;
// ReSharper restore PossibleMultipleEnumeration
		}

		[DebuggerStepThrough]
		public static IEnumerable<T> Each<T>(this IEnumerable<T> values, Action<T, int> eachAction)
		{
			int i = 0;
// ReSharper disable PossibleMultipleEnumeration
			foreach (var item in values)
			{
				eachAction(item, i);
				i++;
			}

			return values;
// ReSharper restore PossibleMultipleEnumeration
		}

		[DebuggerStepThrough]
		public static IEnumerable<TResult> Map<T, TResult>(this IEnumerable<T> values, Func<T, TResult> projection)
		{
// ReSharper disable LoopCanBeConvertedToQuery
			foreach (T item in values)
// ReSharper restore LoopCanBeConvertedToQuery
			{
				yield return projection(item);
			}
		}

		[DebuggerStepThrough]
		public static IEnumerable<TResult> Map<T, TResult>(this IEnumerable<T> values, Func<T, int, TResult> projectionWithIndex)
		{
			int index = 0;
			foreach (T item in values)
			{
				yield return projectionWithIndex(item, index);
				index++;
			}
		}

		public static int IndexOf<TItem>(this IEnumerable<TItem> collection, Func<TItem, bool> predicate)
		{
			var y = collection
				.Map((item, index) => new { Index = index, Item = item })
				.FirstOrDefault(x => predicate(x.Item));
			return y != null ? y.Index : -1;
		}
	}
}