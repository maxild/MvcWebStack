using System;
using System.Collections.Generic;

namespace Maxfire.TestCommons.AssertExtensibility
{
	public class CollectionEquivalenceComparer<T> : IComparer<IEnumerable<T>>
		where T : IComparable<T>
	{
		public int Compare(IEnumerable<T> left, IEnumerable<T> right)
		{
			List<T> leftList = new List<T>(left);
			List<T> rightList = new List<T>(right);
			leftList.Sort();
			rightList.Sort();

			return CollectionComparer<T>.Compare(leftList, rightList);
		}
	}
}