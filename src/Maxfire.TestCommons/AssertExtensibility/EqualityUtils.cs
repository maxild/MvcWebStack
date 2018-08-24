using System;
using System.Diagnostics.CodeAnalysis;

namespace Maxfire.TestCommons.AssertExtensibility
{
	internal static class EqualityUtils
	{
		public static bool Equals<T>(T x, T y, bool skipTypeCheck = false)
		{
			Type type = typeof(T);

			// Null?
			if (!type.IsValueType || type.IsGenericType && type.GetGenericTypeDefinition().IsAssignableFrom(typeof(Nullable<>)))
			{
				if (Object.Equals(x, default(T)))
					return Object.Equals(y, default(T));

				if (Object.Equals(y, default(T)))
					return false;
			}

			if (!skipTypeCheck && x.GetType() != y.GetType())
			{
				return false;
			}

		    if (x is IEquatable<T> equatableX)
			{
				return equatableX.Equals(y);
			}

		    if (y is IEquatable<T> equatableY)
			{
				return equatableY.Equals(x);
			}

			return Object.Equals(x, y);
		}

		[SuppressMessage("ReSharper", "PossibleNullReferenceException")]
		public static bool Equals(object x, object y, bool skipTypeCheck = false)
		{
			// Null?
			if (x == y) return true;
			if (x == null || y == null) return false;

			Type typeX = x.GetType();
			Type typeY = y.GetType();

			if (!skipTypeCheck && typeX != typeY)
			{
				return false;
			}

			Type typeEquatableY = typeof(IEquatable<>).MakeGenericType(typeY);
			if (typeEquatableY.IsAssignableFrom(typeX))
			{
				return (bool)typeEquatableY.GetMethod("Equals").Invoke(x, new[] { y });
			}

			Type typeEquatableX = typeof(IEquatable<>).MakeGenericType(typeX);
			if (typeEquatableX.IsAssignableFrom(typeY))
			{
				return (bool)typeEquatableX.GetMethod("Equals").Invoke(y, new[] { x });
			}

		    if (x is IEquatable<object> equatableX)
			{
				return equatableX.Equals(y);
			}

		    if (y is IEquatable<object> equatableY)
			{
				return equatableY.Equals(x);
			}

			// This call cannot be symmetric (that is x.Equals(y) or y.Equals(x) does 
			// not work, because all objects has an Equals method).
			return x.Equals(y);
		}
	}
}
