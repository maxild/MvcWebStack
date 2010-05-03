﻿using System;
using System.Collections.Generic;
using System.Linq;
using Maxfire.Core;

namespace Maxfire.Skat
{
	public static class ValueTupleExtensions
	{
		public static ValueTuple<T> ToTuple<T>(this T value)
		{
			return new ValueTuple<T>(value);
		}

		public static ValueTuple<T> ToTuple<T>(this T first, T second)
		{
			return new ValueTuple<T>(first, second);
		}

		public static ValueTuple<T> ToTuple<T>(this IEnumerable<T> coll)
		{
			return new ValueTuple<T>(coll.ToList());
		}

		/// <summary>
		/// Map performs a projection
		/// </summary>
		// TODO: Svarer til Select i LINQ
		public static ValueTuple<TItem> Map<TParent, TItem>(this ValueTuple<TParent> tuple, Func<TParent, TItem> projection)
		{
			var list = new List<TItem>(tuple.Size);
			for (int i = 0; i < tuple.Size; i++)
			{
				list.Add(projection(tuple[i]));
			}
			return new ValueTuple<TItem>(list);
		}

		public static bool DifferentSign<T>(this ValueTuple<T> tuple)
		{
			if (tuple.Size == 1)
			{
				return false;
			}

			return Operator<T>.Sign(tuple[0]) * Operator<T>.Sign(tuple[1]) == -1;
		}

		/// <summary>
		/// Hvis tuple består af to elementer med forskelligt fortegn, så nedbringes det positive element
		/// med størrelsen af det negative element, dog under hensyn til at det nedbragte element ikke må 
		/// blive negativt.
		/// </summary>
		public static ValueTuple<T> NedbringPositivtMedEvtNegativt<T>(this ValueTuple<T> tuple)
		{
			if (tuple.DifferentSign() == false)
			{
				return tuple;
			}

			T first, second;
			if (Operator<T>.LessThanZero(tuple[0]))
			{
				T delta = Operator<T>.Min(Operator<T>.Negate(tuple[0]), tuple[1]);
				first = Operator<T>.Add(tuple[0], delta);
				second = Operator<T>.Subtract(tuple[1], delta);
			}
			else
			{
				T delta = Operator<T>.Min(tuple[0], Operator<T>.Negate(tuple[1]));
				first = Operator<T>.Subtract(tuple[0], delta);
				second = Operator<T>.Add(tuple[1], delta);
			}

			return new ValueTuple<T>(first, second);
		}
	}
}