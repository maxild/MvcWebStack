using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Xunit.Sdk;


namespace Maxfire.TestCommons.AssertExtensibility
{
	public class CollectionComparer<T> : IComparer<IEnumerable<T>>
		where T : IComparable<T>
	{
		int IComparer<IEnumerable<T>>.Compare(IEnumerable<T> x, IEnumerable<T> y)
		{
			return Compare(x, y);
		}

		/// <summary>
		/// Compare iterators for greater than, less than, or equality.
		/// </summary>
		/// <param name="left">Left hand side</param>
		/// <param name="right">Right hands side</param>
		/// <returns>0 if equal, -1 if left has less items or some item is smaller, 1 if left has more items or some item is greater.</returns>
		public static int Compare(IEnumerable<T> left, IEnumerable<T> right)
		{
		    using (IEnumerator<T> enumLeft = left.GetEnumerator())
            using (IEnumerator<T> enumRight = right.GetEnumerator())
		    {
		        while (true)
		        {
		            if (!enumLeft.MoveNext())
		            {
		                if (!enumRight.MoveNext())
		                    return 0;
		                return -1;
		            }

		            if (!enumRight.MoveNext())
		                return 1;

		            // ReSharper disable once PossibleNullReferenceException
		            int result = enumLeft.Current.CompareTo(enumRight.Current);

		            if (result != 0)
		                return result;
		        }
		    }
		}
	}

	public class CollectionComparer : IComparer<IEnumerable>
	{
		int IComparer<IEnumerable>.Compare(IEnumerable x, IEnumerable y)
		{
			return Compare(x, y);
		}

		[SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
		public static int Compare(IEnumerable left, IEnumerable right)
		{
			IEnumerator enumLeft = left.GetEnumerator();
			IEnumerator enumRight = right.GetEnumerator();

			while (true)
			{
				if (!enumLeft.MoveNext())
				{
					if (!enumRight.MoveNext())
						return 0;
					return -1;
				}

				if (!enumRight.MoveNext())
					return 1;

				var comparable = enumLeft.Current as IComparable;
				object other = enumRight.Current;

				if (comparable == null)
				{
					comparable = enumRight as IComparable;
					other = enumLeft.Current;
				}

				if (comparable == null)
				{
					throw new XunitException("Cannot compare the two IEnumerable objects, because none of the elements are IComparable");
				}

				int result = comparable.CompareTo(other);
				if (result != 0)
					return result;
			}
		}
	}
}
