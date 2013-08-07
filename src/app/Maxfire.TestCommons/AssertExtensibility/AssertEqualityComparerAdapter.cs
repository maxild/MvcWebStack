using System;
using System.Collections;
using System.Collections.Generic;

namespace Maxfire.TestCommons.AssertExtensibility
{
	public class AssertEqualityComparerAdapter<T> : IEqualityComparer
	{
		private readonly IEqualityComparer<T> _innerComparer;

		public AssertEqualityComparerAdapter(IEqualityComparer<T> innerComparer)
		{
			_innerComparer = innerComparer;
		}

		public new bool Equals(object x, object y)
		{
			return _innerComparer.Equals((T)x, (T)y);
		}

		public int GetHashCode(object obj)
		{
			throw new NotImplementedException();
		}
	}
}