using System;
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
		public static ValueTuple<TItem> Map<TParent, TItem>(this ValueTuple<TParent> tuple, Func<TParent, TItem> projection)
		{
			var list = new List<TItem>(tuple.Size);
			for (int i = 0; i < tuple.Size; i++)
			{
				list.Add(projection(tuple[i]));
			}
			return new ValueTuple<TItem>(list);
		}

		public static ValueTuple<TItem> Map<TParent, TItem>(this ValueTuple<TParent> tuple, Func<TParent, int, TItem> projectionWithIndex)
		{
			var list = new List<TItem>(tuple.Size);
			for (int i = 0; i < tuple.Size; i++)
			{
				list.Add(projectionWithIndex(tuple[i], i));
			}
			return new ValueTuple<TItem>(list);
		}

		public static ValueTuple<TItem> Map<TParent, TItem>(this ValueTuple<TParent> tuple, Func<int, TItem> projectionWithIndex)
		{
			var list = new List<TItem>(tuple.Size);
			for (int i = 0; i < tuple.Size; i++)
			{
				list.Add(projectionWithIndex(i));
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

		/// <summary>
		/// Hvis tuple (bruttoGrundlag) består af to elementer, hvor det ene element er mindre end bundfradraget, og 
		/// det andet element er større end bundfradraget, så er det muligt at overføre det uudnyttede bundfradrag 
		/// fra den ene ægtefælle til den anden ægtefælle. Metoden returnerer det modregnede nettogrundlag, hvor 
		/// bruttogrundlaget er nedbragt med ægtefællernes 'sambeskattede' bundfradrag, efter der evt. er sket overførsel 
		/// af bundfradrag mellem ægtefællerne.
		/// </summary>
		/// <remarks>
		/// Dette princip bliver benyttet ved beregning af mellemskattegrundlaget og topskattegrundlaget.
		/// </remarks>
		public static ValueTuple<decimal> NedbringMedSambeskattetBundfradrag(this ValueTuple<decimal> bruttoGrundlag, decimal bundfradrag)
		{
			var nettoGrundlag = bruttoGrundlag - bundfradrag;
			var modregnetNettoGrundlag = nettoGrundlag.NedbringPositivtMedEvtNegativt();
			return modregnetNettoGrundlag;
		}

		/// <summary>
		/// Hvis tuple (bruttoGrundlag) består af to elementer, hvor det ene element er mindre end bundfradraget, og 
		/// det andet element er større end bundfradraget, så er det muligt at overføre det uudnyttede bundfradrag 
		/// fra den ene ægtefælle til den anden ægtefælle. Metoden returnerer det 'effektive' bundfradrag, der summer 
		/// til personernes samlede bundfradrag, men hvor sådan overførsel har fundet sted.
		/// </summary>
		/// <remarks>
		/// Dette princip bliver benyttet ved beregning af mellemskattegrundlaget og topskattegrundlaget.
		/// </remarks>
		public static ValueTuple<decimal> BeregnSambeskattetBundfradrag(this ValueTuple<decimal> bruttoGrundlag, decimal bundfradrag)
		{
			var modregnetNettoGundlag = bruttoGrundlag.NedbringMedSambeskattetBundfradrag(bundfradrag);
			var overfoertBundfradrag = bruttoGrundlag - modregnetNettoGundlag;
			return overfoertBundfradrag;
		}

		public static ValueTuple<decimal> RoundMoney(this ValueTuple<decimal> tuple)
		{
			IList<decimal> list = new List<decimal>(tuple.Size);
			for (int i = 0; i < tuple.Size; i++)
			{
				list.Add(Math.Round(tuple[i], 2, MidpointRounding.ToEven));
			}
			return new ValueTuple<decimal>(list);
		}
	}
}