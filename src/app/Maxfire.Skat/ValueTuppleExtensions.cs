using System;
using System.Collections.Generic;
using System.Linq;
using Maxfire.Core;

namespace Maxfire.Skat
{
	public static class ValueTuppleExtensions
	{
		public static T First<T>(this ValueTupple<T> tupple)
		{
			return tupple[0];
		}
		
		public static T Second<T>(this ValueTupple<T> tupple)
		{
			return tupple[1];
		}
		
		public static bool IsIndividual<T>(this ValueTupple<T> list)
		{
			return list.Size == 1;
		}

		public static bool AreMarried<T>(this ValueTupple<T> list)
		{
			return list.Size == 2;
		}

		public static ValueTupple<T> AsIndividual<T>(this T value)
		{
			return new ValueTupple<T>(value);
		}

		public static ValueTupple<T> AsMarried<T>(this T value, T other)
		{
			return new ValueTupple<T>(value, other);
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

		public static ValueTupple<T> Modregn<T>(this ValueTupple<T> tupple)
		{
			if (tupple.DifferentSign() == false)
			{
				return tupple;
			}

			if (Operator<T>.LessThanZero(tupple[0]))
			{
				
			}
			else
			{
				
			}

			// TODO
			return new ValueTupple<T>(default(T));
		}
	}
}