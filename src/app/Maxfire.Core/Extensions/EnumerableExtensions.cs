using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Maxfire.Core.Extensions
{
	public static class EnumerableExtensions
	{
		public static bool IsEmpty<T>(this IEnumerable<T> iterator)
		{
			return !iterator.IsNotEmpty();
		}

		public static bool IsNotEmpty<T>(this IEnumerable<T> iterator)
		{
			return iterator != null && iterator.Any();
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
			foreach (var item in values)
			{
				eachAction(item);
			}

			return values;
		}

		[DebuggerStepThrough]
		public static IEnumerable Each(this IEnumerable values, Action<object, int> eachAction)
		{
			int i = 0;
			foreach (var item in values)
			{
				eachAction(item, i);
				i++;
			}

			return values;
		}

		[DebuggerStepThrough]
		public static IEnumerable<T> Each<T>(this IEnumerable<T> values, Action<T> eachAction)
		{
			foreach (var item in values)
			{
				eachAction(item);
			}

			return values;
		}

		[DebuggerStepThrough]
		public static IEnumerable<T> Each<T>(this IEnumerable<T> values, Action<T, int> eachAction)
		{
			int i = 0;
			foreach (var item in values)
			{
				eachAction(item, i);
				i++;
			}

			return values;
		}

		[DebuggerStepThrough]
		public static IEnumerable<TResult> Map<T, TResult>(this IEnumerable<T> values, Func<T, TResult> projection)
		{
			foreach (T item in values)
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
			var y = collection.Select((item, index) => new { Index = index, Item = item })
				.Where(x => predicate(x.Item))
				.FirstOrDefault();
			return y != null ? y.Index : -1;
		}
	}
}