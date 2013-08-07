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
			if (x == y) return true;
			if (x == null || y == null) return false;

			Type typeX = x.GetType();
			Type typeY = y.GetType();

			if (!_skipTypeCheck && typeX != typeY)
			{
				return false;
			}

			Type typeEquatableY = typeof (IEquatable<>).MakeGenericType(typeY);
			if (typeEquatableY.IsAssignableFrom(typeX))
			{
				return (bool) typeEquatableY.GetMethod("Equals").Invoke(x, new[] {y});
			}

			Type typeEquatableX = typeof(IEquatable<>).MakeGenericType(typeX);
			if (typeEquatableX.IsAssignableFrom(typeY))
			{
				return (bool) typeEquatableX.GetMethod("Equals").Invoke(y, new[] { x });
			}
			
			var equatableX = x as IEquatable<object>;
			if (equatableX != null)
			{
				return equatableX.Equals(y);
			}

			var equatableY = y as IEquatable<object>;
			if (equatableY != null)
			{
				return equatableY.Equals(x);
			}

			// This call cannot be symmetric (that is x.Equals(y) or y.Equals(x) does 
			// not work, because all objects has an Equals method).
			return x.Equals(y);
		}

		public int GetHashCode(object obj)
		{
			throw new NotImplementedException();
		}
	}
}