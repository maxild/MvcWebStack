using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Maxfire.Core.Extensions;

namespace Maxfire.Core.Collections
{
	public class RangeIteratorSet<T> : IEnumerable<T> 
		where T : IComparable<T>
	{
		private readonly SortedList<T, RangeIterator<T>> _rangeIterators = new SortedList<T, RangeIterator<T>>();
		
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			int count = 0;
			RangeIterator<T> lastvalue = null;
			foreach (var iterator in _rangeIterators.Values)
			{
				if (lastvalue != null)
				{
					if (count > 0)
					{
						sb.Append(", ");
					}
					sb.Append(lastvalue.ToString());
					count++;
				}
				lastvalue = iterator;
			}
			if (lastvalue == null)
			{
				sb.Append("{ Ø }");
			}
			else 
			{
				if (count > 0)
				{
					sb.Append(" eller ");
				}
				sb.Append(lastvalue.ToString());
			}
			return sb.ToString();
		}

		public bool Contains(T value)
		{
			foreach (var range in _rangeIterators.Values)
			{
				if (range.Contains(value))
				{
					return true;
				}
			}
			return false;
		}

		public void Add(RangeIterator<T> values)
		{
			_rangeIterators.Add(values.Range.Start, values);
		}

		public IEnumerator<T> GetEnumerator()
		{
			var combinedIterator = Enumerable.Empty<T>();
			_rangeIterators.Values.Each(rangeIterator => combinedIterator = combinedIterator.Concat(rangeIterator));
			return combinedIterator.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}