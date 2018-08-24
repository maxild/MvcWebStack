using System;
using System.Collections.Generic;
using System.Linq;

namespace Maxfire.TestCommons.AssertExtensibility
{
	public class CollectionEquivalenceComparer<T> : IEqualityComparer<IEnumerable<T>>
		where T : IComparable<T>
	{
		public bool Equals(IEnumerable<T> left, IEnumerable<T> right)
		{
			var leftList = new List<T>(left ?? Enumerable.Empty<T>());
			var rightList = new List<T>(right ?? Enumerable.Empty<T>());

			leftList.Sort();
			rightList.Sort();

			return CollectionEqualityComparer<T>.Equals(leftList, rightList);
		}

		int IEqualityComparer<IEnumerable<T>>.GetHashCode(IEnumerable<T> obj)
		{
			throw new NotImplementedException();
		}
	}
}
