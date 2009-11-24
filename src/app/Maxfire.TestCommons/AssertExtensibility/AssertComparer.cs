using System;
using System.Collections;
using System.Collections.Generic;

namespace Maxfire.TestCommons.AssertExtensibility
{
	/// <summary>
	/// This comparer handles Contains, Equal and InRange logic
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class AssertComparer<T> : IComparer<T>
	{
		public int Compare(T x, T y)
		{
			Type type = typeof(T);

			// We handle 'null' for Reference types or Nullable types
			if (!type.IsValueType || (type.IsGenericType && type.GetGenericTypeDefinition().IsAssignableFrom(typeof(Nullable<>))))
			{
				T nullValue = default(T);
				if (Equals(x, nullValue))
				{
					if (Equals(y, nullValue))
						return 0;
					return -1;
				}

				if (Equals(y, nullValue))
					return -1;
			}

			// Implements IComparable<T>?
			IComparable<T> comparable1 = x as IComparable<T>;

			if (comparable1 != null)
				return comparable1.CompareTo(y);

			// Implements IComparable?
			IComparable comparable2 = x as IComparable;

			if (comparable2 != null)
				return comparable2.CompareTo(y);

			// Implements IEquatable<T>?
			IEquatable<T> equatable = x as IEquatable<T>;

			if (equatable != null)
				return equatable.Equals(y) ? 0 : -1;

			// Enumerable?
			IEnumerable enumerableX = x as IEnumerable;
			IEnumerable enumerableY = y as IEnumerable;

			if (enumerableX != null && enumerableY != null)
			{
				IEnumerator enumeratorX = enumerableX.GetEnumerator();
				IEnumerator enumeratorY = enumerableY.GetEnumerator();

				while (true)
				{
					bool hasNextX = enumeratorX.MoveNext();
					bool hasNextY = enumeratorY.MoveNext();

					if (!hasNextX || !hasNextY)
						return (hasNextX == hasNextY ? 0 : -1);

					if (!Equals(enumeratorX.Current, enumeratorY.Current))
						return -1;
				}
			}

			// Last case, rely on Object.Equals
			return Equals(x, y) ? 0 : -1;
		}
	}
}