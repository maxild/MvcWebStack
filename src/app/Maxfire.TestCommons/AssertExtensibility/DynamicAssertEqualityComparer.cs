using System;
using System.Collections;
using System.Collections.Generic;

namespace Maxfire.TestCommons.AssertExtensibility
{
	public class DynamicAssertEqualityComparer : IEqualityComparer, IEqualityComparer<object>
	{
		private readonly bool _skipTypeCheck;

		public DynamicAssertEqualityComparer(bool skipTypeCheck = false)
		{
			_skipTypeCheck = skipTypeCheck;
		}

		public new bool Equals(object x, object y)
		{
			return EqualityUtils.Equals(x, y, _skipTypeCheck);
		}

		public int GetHashCode(object obj)
		{
			throw new NotImplementedException();
		}
	}
}