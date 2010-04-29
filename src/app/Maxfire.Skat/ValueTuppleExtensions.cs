using System;
using System.Collections.Generic;
using System.Linq;
using Maxfire.Core;

namespace Maxfire.Skat
{
	public static class ValueTuppleExtensions
	{
		public static ValueTupple<T> ToTupple<T>(this T value)
		{
			return new ValueTupple<T>(value);
		}

		public static ValueTupple<T> ToTupple<T>(this T first, T second)
		{
			return new ValueTupple<T>(first, second);
		}

		public static ValueTupple<T> ToTupple<T>(this IEnumerable<T> coll)
		{
			return new ValueTupple<T>(coll.ToList());
		}

		/// <summary>
		/// Map performs a projection
		/// </summary>
		// TODO: Svarer til Select i LINQ
		public static ValueTupple<TItem> Map<TParent, TItem>(this ValueTupple<TParent> tupple, Func<TParent, TItem> projection)
		{
			var list = new List<TItem>(tupple.Size);
			for (int i = 0; i < tupple.Size; i++)
			{
				list.Add(projection(tupple[i]));
			}
			return new ValueTupple<TItem>(list);
		}

		public static bool DifferentSign<T>(this ValueTupple<T> tupple)
		{
			if (tupple.Size == 1)
			{
				return false;
			}

			return Operator<T>.Sign(tupple[0]) * Operator<T>.Sign(tupple[1]) == -1;
		}

		/// <summary>
		/// Hvis tupple består af to elementer med forskelligt fortegn, så nedbringes det positive element
		/// med størrelsen af det negative element, dog under hensyn til at det nedbragte element ikke må 
		/// blive negativt.
		/// </summary>
		public static ValueTupple<T> NedbringPositivtMedEvtNegativt<T>(this ValueTupple<T> tupple)
		{
			if (tupple.DifferentSign() == false)
			{
				return tupple;
			}

			T first, second;
			if (Operator<T>.LessThanZero(tupple[0]))
			{
				T delta = Operator<T>.Min(Operator<T>.Negate(tupple[0]), tupple[1]);
				first = Operator<T>.Add(tupple[0], delta);
				second = Operator<T>.Subtract(tupple[1], delta);
			}
			else
			{
				T delta = Operator<T>.Min(tupple[0], Operator<T>.Negate(tupple[1]));
				first = Operator<T>.Subtract(tupple[0], delta);
				second = Operator<T>.Add(tupple[1], delta);
			}

			return new ValueTupple<T>(first, second);
		}
	}
}