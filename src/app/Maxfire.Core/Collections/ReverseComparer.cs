using System.Collections.Generic;
using Maxfire.Core.Extensions;

namespace Maxfire.Core.Collections
{
	/// <summary>
	/// Implementation of IComparer{T} based on another one;
	/// this simply reverses the original comparison.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public sealed class ReverseComparer<T> : IComparer<T>
	{
		readonly IComparer<T> _originalComparer;

		/// <summary>
		/// Returns the original comparer; this can be useful to avoid multiple
		/// reversals.
		/// </summary>
		public IComparer<T> OriginalComparer
		{
			get { return _originalComparer; }
		}

		/// <summary>
		/// Creates a new reversing comparer.
		/// </summary>
		/// <param name="original">The original comparer to use for comparisons.</param>
		public ReverseComparer(IComparer<T> original)
		{
			original.ThrowIfNull("original");
			_originalComparer = original;
		}

		/// <summary>
		/// Returns the result of comparing the specified values using the original
		/// comparer, but reversing the order of comparison.
		/// </summary>
		public int Compare(T x, T y)
		{
			return _originalComparer.Compare(y, x);
		}
	}
}