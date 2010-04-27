using System;
using System.Collections.Generic;

namespace Maxfire.Skat
{
	// Noter:
	// Beregnerens API benytter altid en kollektion (IList<T> via ReadOnlyCollection wrapper)
	// til repræsentere en eller to personer. Konventionen er
	//		Count == 1: Beregning af ugift person
	//      Count == 2: beregning af gifte personer (ægtefæller med sambeskatning)
	// Beregning af to ugifte personer kan laves via facade API
	//

	public static class ListExtensions // TODO: Maybe use types for input etc instead of unconditioned T
	{
		public static T First<T>(this IList<T> list)
		{
			return list[0];
		}
		
		public static T FirstOrDefault<T>(this IList<T> list)
		{
			return list.Count > 0 ? list[0] : default(T);
		}

		public static int IndexOf<T>(this IList<T> list, Func<T, bool> predicate)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (predicate(list[i]))
				{
					return i;
				}
			}
			return -1;
		}

		public static T Second<T>(this IList<T> list)
		{
			return list[1];
		}
		
		public static T SecondOrDefault<T>(this IList<T> list)
		{
			return list.Count > 1 ? list[1] : default(T);
		}

		public static bool IsIndividualOrAreMarried<T>(this IList<T> list)
		{
			return list.Count == 1 && list.Count == 2;
		}

		public static bool IsIndividual<T>(this IList<T> list)
		{
			return list.Count == 1;
		}

		public static bool AreMarried<T>(this IList<T> list)
		{
			return list.Count == 2;
		}

		public static IList<T> AsIndividual<T>(this T value)
		{
			return new List<T> {value};
		}

		public static IList<T> AsMarried<T>(this T value, T other)
		{
			return new List<T> { value, other };
		}

		public static T PartnerOf<T>(this IList<T> list, T partner)
		{
			int i = list.IndexOf(partner);
			if (i == 0 || i == 1)
			{
				return list[i == 0 ? 1 : 0];
			}
			return default(T);
		}
	}

	public static class DecimalExtensions
	{
		public static int Sign(this decimal number)
		{
			if (number < 0)
			{
				return -1;
			}
			return number > 0 ? 1 : 0;
		}

		public static bool DifferentSign(this decimal lhs, decimal rhs)
		{
			return lhs.Sign() * rhs.Sign() == -1;
		}

		public static decimal NonNegative(this decimal number)
		{
			return Math.Max(0, number);
		}

		
	}

	public interface IInputBeloeb
	{
		/// <summary>
		/// Lønindkomst mv.
		/// </summary>
		/// <remarks>
		/// Skal være efter fradrag af eget bidrag til arbejdsgiverpensionsordning.
		/// </remarks>
		decimal AMIndkomst { get; }
		
		/// <summary>
		/// Pension, sociale ydelser og arbejdsløshedsunderstøttelse mv.
		/// </summary>
		/// <remarks>
		/// Der betales ikke arbejdsmarkedsbidrag af denne del af den personlige indkomst.
		/// </remarks>
		decimal IkkeAMIndkomst { get;}

		/// <summary>
		/// Bidrag og præmier til privattegnede pensionsordninger med løbende udbetalinger og ratepension 
		/// samt privattegnet kapitalpension dog højest 43.100 kr. i 2007.
		/// </summary>
		decimal PrivatPension { get; }
	}

	public interface IInputSatser
	{
		decimal JobFradragSats { get; }
		decimal AMBidragSats { get; }
	}

	public static class Skatteberegner
	{
		public static SkatteberegningResult Beregn(SkatteberegningInput input)
		{
			IInputBeloeb beloeb = null;
			IInputSatser satser = null;
			//
			// 1: Indkomst
			//

			// TODO: Sondring mellem intern indkomst context (f.eks. skal beskæftigelsesfradrag beregnes, og andre fradrag begrænses) og input-context

			// TODO: Simpelt interface på de mange rubrikker (kun det højest nødvendige, mindste fællesnævner). Ved at definere dette interface er klienten selv ansvarlig for mapping

			// Beregn beskæftigelsesfradraget (der er et ligningsmæssigt fradrag)
			decimal jobFradrag = (beloeb.AMIndkomst - beloeb.PrivatPension) * satser.JobFradragSats;
			
			// Beregn de ligningsmæssige fradrag


			// Beregn arbejdsmarkedsbidrag
			decimal arbejdsmarkedsBidrag = beloeb.AMIndkomst * satser.AMBidragSats;

			// Beregn personlig indkomst 
			// (Løn - AMBidrag - AllePensionsindbetalinger)
			// (OBS: AMIndkomst er opgjort efter fradrag af evt. _egne_ bidrag til arbejdsgiver administrerede pensionsordninger,
			// _MEN_ det er ulogisk!!!!!
			decimal personligIndkomst = (beloeb.AMIndkomst + beloeb.IkkeAMIndkomst) - beloeb.PrivatPension 
				- arbejdsmarkedsBidrag;

			// Beregn kapital indkomst
			decimal kapitalIndkomst;

			// Beregn skattepligtig indkomst (TODO: Med ægtefælle overførsel)
			decimal skattepligtigIndkomst;

			//
			// 2: Ejendomsværdiskat (springes over, da den kan insættes hvorsomhelst)
			//

			//
			// 3: Skatter uden progression (kommune, bund og sundhedsbidrag)
			//

			//
			// 4: Topskat
			//

			//
			// 5: Aktieskat
			//

			//
			// 6: Regulering af skattebeløbene, jf. § 13 i personskatteloven
			//

			// Hvis den skattepligtige indkomst udviser underskud....

			//
			// 7: Skatteværdien af personfradraget opgøres pr. skattetype
			//

			//
			// 8: Værdien af skatteloftet beregnes
			//

			//
			// 9: Bestem nedslaget i (forårspakke 2.0)
			//

			//
			// 10: Beregn kompensation (forårspakke 2.0) for skatteår 2012, 2013, etc.
			//

			//
			// 11: Beregn grøn check
			//

			return null;
		}
	}
}