using System;
using System.Collections;
using System.Collections.Generic;

namespace Maxfire.TestCommons.AssertExtensibility
{
	public class AssertEqualityComparer<T> : IEqualityComparer<T>
	{
// ReSharper disable StaticFieldInGenericType
		private static readonly IEqualityComparer _defaultInnerComparer = new AssertEqualityComparerAdapter<object>(new AssertEqualityComparer<object>());
// ReSharper restore StaticFieldInGenericType

		private readonly IEqualityComparer _innerComparer;
		private readonly bool _skipTypeCheck;

		public AssertEqualityComparer(bool skipTypeCheck = false, IEqualityComparer innerComparer = null)
		{
			_skipTypeCheck = skipTypeCheck;
			_innerComparer = innerComparer ?? _defaultInnerComparer;
		}

		public bool Equals(T x, T y)
		{
			Type type = typeof(T);

			// Null?
			if (!type.IsValueType || (type.IsGenericType && type.GetGenericTypeDefinition().IsAssignableFrom(typeof(Nullable<>))))
			{
				if (Object.Equals(x, default(T)))
					return Object.Equals(y, default(T));

				if (Object.Equals(y, default(T)))
					return false;
			}

			// Same type?
			if (!_skipTypeCheck && x.GetType() != y.GetType())
				return false;

			// Implements IEquatable<T>?
			var equatable = x as IEquatable<T>;
			if (equatable != null)
				return equatable.Equals(y);

			// Implements IComparable<T>?
			var comparable1 = x as IComparable<T>;
			if (comparable1 != null)
				return comparable1.CompareTo(y) == 0;

			// Implements IComparable?
			var comparable2 = x as IComparable;
			if (comparable2 != null)
				return comparable2.CompareTo(y) == 0;

			// Enumerable?
			var enumerableX = x as IEnumerable;
			var enumerableY = y as IEnumerable;

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

					if (!_innerComparer.Equals(enumeratorX.Current, enumeratorY.Current))
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