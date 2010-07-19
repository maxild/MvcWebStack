using System;
using System.Collections;
using System.Collections.Generic;

namespace Maxfire.TestCommons.AssertExtensibility
{
	public class AssertEqualityComparer<T> : IEqualityComparer<T>
	{
		static AssertEqualityComparer<object> innerComparer = new AssertEqualityComparer<object>();

		public bool Equals(T x, T y)
		{
			Type type = typeof(T);

			// We handle 'null' for Reference types or Nullable types
			if (!type.IsValueType || (type.IsGenericType && type.GetGenericTypeDefinition().IsAssignableFrom(typeof(Nullable<>))))
			{
				if (Object.Equals(x, default(T)))
					return Object.Equals(y, default(T));

				if (Object.Equals(y, default(T)))
					return false;
			}

			// Implements IEquatable<T>?
			IEquatable<T> equatable = x as IEquatable<T>;
			if (equatable != null)
				return equatable.Equals(y);

			// Implements IComparable<T>?
			IComparable<T> comparable1 = x as IComparable<T>;
			if (comparable1 != null)
				return comparable1.CompareTo(y) == 0;

			// Implements IComparable?
			IComparable comparable2 = x as IComparable;
			if (comparable2 != null)
				return comparable2.CompareTo(y) == 0;

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
						return (hasNextX == hasNextY);

					if (!innerComparer.Equals(enumeratorX.Current, enumeratorY.Current))
						return false;
				}
			}

			// Last case, rely on Object.Equals
			return Object.Equals(x, y);
		}

		public int GetHashCode(T obj)
		{
			throw new NotImplementedException();
		}
	}
}