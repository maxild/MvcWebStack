using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Maxfire.TestCommons.AssertExtensibility
{
	public class AssertEqualityComparer<T> : IEqualityComparer<T>
	{
// ReSharper disable StaticFieldInGenericType
		private static readonly IEqualityComparer DEFAULT_INNER_COMPARER = new AssertEqualityComparerAdapter<object>(new AssertEqualityComparer<object>());
// ReSharper restore StaticFieldInGenericType

		private readonly IEqualityComparer _innerComparer;
		private readonly bool _skipTypeCheck;

		public AssertEqualityComparer(bool skipTypeCheck = false, IEqualityComparer innerComparer = null)
		{
			_skipTypeCheck = skipTypeCheck;
			_innerComparer = innerComparer ?? DEFAULT_INNER_COMPARER;
		}

		[SuppressMessage("ReSharper", "PossibleNullReferenceException")]
		public bool Equals(T x, T y)
		{
			Type type = typeof(T);

			// Null?
			if (!type.IsValueType || type.IsGenericType && type.GetGenericTypeDefinition().IsAssignableFrom(typeof(Nullable<>)))
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
		    if (x is IEquatable<T> equatable)
				return equatable.Equals(y);

			// Implements IComparable<T>?
		    if (x is IComparable<T> comparable1)
				return comparable1.CompareTo(y) == 0;

			// Implements IComparable?
		    if (x is IComparable comparable2)
				return comparable2.CompareTo(y) == 0;

			// Enumerable?
			var enumerableX = x as IEnumerable;

		    if (enumerableX != null && y is IEnumerable enumerableY)
			{
				IEnumerator enumeratorX = enumerableX.GetEnumerator();
				IEnumerator enumeratorY = enumerableY.GetEnumerator();

				while (true)
				{
					bool hasNextX = enumeratorX.MoveNext();
					bool hasNextY = enumeratorY.MoveNext();

					if (!hasNextX || !hasNextY)
						return hasNextX == hasNextY;

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
