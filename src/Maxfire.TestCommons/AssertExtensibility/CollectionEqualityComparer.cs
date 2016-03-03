using System;
using System.Collections;
using System.Collections.Generic;

namespace Maxfire.TestCommons.AssertExtensibility
{
	public class CollectionEqualityComparer<T> : IEqualityComparer<IEnumerable<T>>
	{
		bool IEqualityComparer<IEnumerable<T>>.Equals(IEnumerable<T> x, IEnumerable<T> y)
		{
			return Equals(x, y);
		}

		/// <summary>
		/// Check that two iterators are equal (i.e. all elements are equal, and are listed in the same order).
		/// </summary>
		public static bool Equals(IEnumerable<T> left, IEnumerable<T> right)
		{
			IEnumerator<T> enumLeft = left.GetEnumerator();
			IEnumerator<T> enumRight = right.GetEnumerator();

			while (true)
			{
				bool hasNextX = enumLeft.MoveNext();
				bool hasNextY = enumRight.MoveNext();

				if (!hasNextX || !hasNextY)
					return (hasNextX == hasNextY);

				if (!EqualityUtils.Equals(enumLeft.Current, enumRight.Current))
					return false;
			}
		}

		int IEqualityComparer<IEnumerable<T>>.GetHashCode(IEnumerable<T> obj)
		{
			throw new NotImplementedException();
		}
	}

	public class CollectionEqualityComparer : IEqualityComparer<IEnumerable>
	{
		bool IEqualityComparer<IEnumerable>.Equals(IEnumerable x, IEnumerable y)
		{
			return Equals(x, y);
		}

		/// <summary>
		/// Check that two iterators are equal (i.e. all elements are equal, and are listed in the same order).
		/// </summary>
		/// <param name="left">Left hand side</param>
		/// <param name="right">Right hand side</param>
		/// <returns>0 if equal, and -1 otherwise</returns>
		public static bool Equals(IEnumerable left, IEnumerable right)
		{
			IEnumerator enumLeft = left.GetEnumerator();
			IEnumerator enumRight = right.GetEnumerator();

			while (true)
			{
				bool hasNextX = enumLeft.MoveNext();
				bool hasNextY = enumRight.MoveNext();

				if (!hasNextX || !hasNextY)
					return (hasNextX == hasNextY);

				if (!EqualityUtils.Equals(enumLeft.Current, enumRight.Current))
					return false;
			}
		}

		int IEqualityComparer<IEnumerable>.GetHashCode(IEnumerable obj)
		{
			throw new NotImplementedException();
		}
	}
}