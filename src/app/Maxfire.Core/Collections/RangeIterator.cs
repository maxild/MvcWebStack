using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Maxfire.Core.Collections.Extensions;
using Maxfire.Core.Extensions;

namespace Maxfire.Core.Collections
{
	public class RangeIterator<T> : IEnumerable<T>
	{
		private readonly Range<T> _range;
		private readonly Func<T, T> _step;
		private readonly bool _ascending;

		public override string ToString()
		{
			const char sepToken = ';';
			const char space = ' ';
			StringBuilder sb = new StringBuilder();
			sb.Append('[');
			T lastValue = default(T);
			int count = 0;
			foreach (var value in this)
			{
				count++;
				if (count == 1)
				{
					sb.Append(value.ToString());
				}
				if (count == 2)
				{
					sb.Append(sepToken);
					sb.Append(space);
					sb.Append(value.ToString());
				}
				else if (count == 4)
				{
					sb.Append(sepToken);
					sb.Append(space);
					sb.Append("...");
					lastValue = value;
				}
				else 
				{
					lastValue = value;
				}
			}
			if (count > 2)
			{
				sb.Append(sepToken);
				sb.Append(space);
				sb.Append(lastValue.ToString());
			}
			sb.Append(']');
			return sb.ToString();
		}

		/// <summary>
		/// Returns the range this object iterates over
		/// </summary>
		public Range<T> Range
		{
			get { return _range; }
		}

		/// <summary>
		/// Returns the step function used for this range
		/// </summary>
		public Func<T, T> Step
		{
			get { return _step; }
		}

		/// <summary>
		/// Returns whether or not this iterator works up from the start point (ascending)
		/// or down from the end point (descending)
		/// </summary>
		public bool Ascending
		{
			get { return _ascending; }
		}

		/// <summary>
		/// Creates an ascending iterator over the given range with the given step function
		/// </summary>
		public RangeIterator(Range<T> range, Func<T, T> step)
			: this(range, step, true)
		{
		}

		/// <summary>
		/// Creates an iterator over the given range with the given step function,
		/// with the specified direction.
		/// </summary>
		public RangeIterator(Range<T> range, Func<T, T> step, bool ascending)
		{
			step.ThrowIfNull("step");

			if ((ascending && range.Comparer.Compare(range.Start, step(range.Start)) >= 0) ||
			    (!ascending && range.Comparer.Compare(range.End, step(range.End)) <= 0))
			{
				throw new ArgumentException("step does nothing, or progresses the wrong way");
			}
			_ascending = ascending;
			_range = range;
			_step = step;
		}

		/// <summary>
		/// Returns an IEnumerator{T} running over the range.
		/// </summary>
		public IEnumerator<T> GetEnumerator()
		{
			bool includesStart = _ascending ? _range.IncludesStart : _range.IncludesEnd;
			bool includesEnd = _ascending ? _range.IncludesEnd : _range.IncludesStart;
			T start = _ascending ? _range.Start : _range.End;
			T end = _ascending ? _range.End : _range.Start;
			IComparer<T> comparer = _ascending ? _range.Comparer : _range.Comparer.Reverse();

			T value = start;

			if (includesStart)
			{
				// Deal with possibility that start point = end point
				if (includesEnd || comparer.Compare(value, end) < 0)
				{
					yield return value;
				}
			}
			value = _step(value);

			while (comparer.Compare(value, end) < 0)
			{
				yield return value;
				value = _step(value);
			}

			// We've already performed a step, therefore we can't
			// still be at the start point
			if (includesEnd && comparer.Compare(value, end) == 0)
			{
				yield return value;
			}
		}

		/// <summary>
		/// Returns an <see cref="IEnumerator"/> running over the range.
		/// </summary>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	public static class RangeIterator
	{
		public static RangeIterator<T> EmptyWithStep<T>(T stepAmount)
		{
			return new RangeIterator<T>(Range.Empty<T>(), t => Operator.Add(t, stepAmount));
		} 
	}
}