using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Maxfire.Core.Extensions;

namespace Maxfire.Skat
{
	/// <summary>
	/// §7, stk 1: Topskattegrundlaget er den personlige indkomst med tillæg af heri fradragne og ikke medregnede 
	/// beløb omfattet af beløbsgrænsen i pensionsbeskatningslovens § 16, stk. 1, og med tillæg af positiv kapitalindkomst. 
	/// </summary>
	/// <remarks>
	/// Beløb omfattet af beløbsgrænsen i pensionsbeskatningslovens § 16, stk. 1, er pensionsindbetalinger til kapitalpension
	/// og opsparing i pensionsøjemed.
	/// </remarks>
	public class TopskatGrundlagBeregner
	{
		public ValueTupple<decimal> BeregnNettoKapitalIndkomstTilBeskatning(ValueTupple<Indkomster> input)
		{			
			var nettokapitalindkomst = input.Map(x => x.NettoKapitalIndkomst);
			var bundfradrag = BeregnBundfradragForPositivKapitalIndkomst(input);
			var nettokapitalindkomstTilBeskatning = nettokapitalindkomst - bundfradrag;
			return nettokapitalindkomstTilBeskatning;
		}

		public ValueTupple<decimal> BeregnBundfradragForPositivKapitalIndkomst(ValueTupple<Indkomster> input)
		{
			if (input.Size == 1)
			{
				return Constants.BundfradragPositivKapitalIndkomst.ToTupple();
			}

			// §7, stk 3: Uudnyttet bundfradrag (grundbeløb på 40.000 kr, 2010) kan 
			// overføres mellem ægtefæller (sambeskating af ægtefæller).
			Func<Indkomster, bool> fuldUdnyttelseAfBundfradrag = x => 
				x.NettoKapitalIndkomst >= Constants.BundfradragPositivKapitalIndkomst;


			// TODO: Kan optimeres til een og kun een har fuld udnyttelse => den anden med uudnyttet overfører til den med fuld udnyttelse, men skriv specs først

			if (input.All(fuldUdnyttelseAfBundfradrag))
			{
				return Constants.BundfradragPositivKapitalIndkomst.ToTupple(Constants.BundfradragPositivKapitalIndkomst);
			}

			// Der er uudnyttet bundfradrag hos mindst en ægtefælle, men kan det udnyttes hos den anden ægtefælle
			var overfoeresTil = input.FirstOrDefault(fuldUdnyttelseAfBundfradrag);
			if (overfoeresTil != null)
			{
				var overfoeresFra = input.PartnerOf(overfoeresTil);
				
				decimal fraBundfradrag = overfoeresFra.NettoKapitalIndkomst.NonNegative();
				decimal tilBundfradrag = 2 * Constants.BundfradragPositivKapitalIndkomst - fraBundfradrag;

				return input.IndexOf(overfoeresTil) == 0 ? tilBundfradrag.ToTupple(fraBundfradrag) : 
					fraBundfradrag.ToTupple(tilBundfradrag);
			}

			// Begge ægtefæller har et uudnyttet bundfradrag, så ingen overførsel
			return Constants.BundfradragPositivKapitalIndkomst.ToTupple(Constants.BundfradragPositivKapitalIndkomst);
		}

		public ValueTupple<decimal> BeregnGrundlag(ValueTupple<Indkomster> input)
		{
			var personligIndkomst = input.Map(x => x.PersonligIndkomst);
			var nettokapitalindkomst = input.Map(x => x.NettoKapitalIndkomst);
			var kapitalPensionsindskud = input.Map(x => x.KapitalPensionsindskud);

			if (input.Size == 1)
			{
				var kapitalIndkomstTilSkat = +(nettokapitalindkomst - Constants.BundfradragPositivKapitalIndkomst);

				var grundlag = +(personligIndkomst + kapitalIndkomstTilSkat
				                 + kapitalPensionsindskud - Constants.TopskatBeloebsgraense);

				return grundlag;
			}

			// Note: grundlagene ved gifte er mere komplicerede
			// 1) grundlag uden nettokapitalindkomst
			// 2) grundlag med nettokapitalindkomst

			// OBS: TFS beregner nogle grundlag til brug ved beregning af kompensation

			return null;
		}
	}

	/// <summary>
	/// Implementering af §7, stk. 2-10, i LBK nr. 959, der beskriver regler for beregning af topskat.
	/// </summary>
	public class TopskatBeregner
	{
		public ValueTupple<decimal> BeregnSkat(ValueTupple<Indkomster> indkomster)
		{
			var topskatGrundlagBeregner = new TopskatGrundlagBeregner();

			if (indkomster.Size == 1)
			{
				var grundlag = topskatGrundlagBeregner.BeregnGrundlag(indkomster);
				var topskat = Constants.Topskattesats * grundlag;
				// Note: L 112 og 'nedsættelse på kapitalindkomst' er skippet 
				return topskat;
			}

			var personligIndkomst = indkomster.Map(x => x.PersonligIndkomst);
			var kapitalPensionsindskud = indkomster.Map(x => x.KapitalPensionsindskud);

			// §7, stk 4: For hver ægtefælle beregnes skat med 15 pct. af vedkommendes personlige indkomst 
			// med tillæg af heri fradragne og ikke medregnede beløb omfattet af beløbsgrænsen i 
			// pensionsbeskatningslovens § 16, stk. 1, i det omfang dette beregningsgrundlag overstiger topskattegrænsen 
			var grundlagUdenNettokapitalindkomst = personligIndkomst + kapitalPensionsindskud;
			var topskatStk4 = Constants.Topskattesats * grundlagUdenNettokapitalindkomst;

			// §7, stk 5: Der beregnes tillige skat af ægtefællernes samlede positive nettokapitalindkomst. Til 
			// dette formål beregnes en skat hos den af ægtefællerne, der har det højeste beregningsgrundlag 
			// efter stk. 4. Skatten beregnes med 15 pct. af denne ægtefælles beregningsgrundlag efter stk. 4 
			// med tillæg af ægtefællernes samlede positive nettokapitalindkomst. Skatten beregnes dog kun i 
			// det omfang, det samlede beløb overstiger topskattegrænsen. 
			var nettoKapitalIndkomstTilBeskatning = topskatGrundlagBeregner.BeregnNettoKapitalIndkomstTilBeskatning(indkomster);
			var samletNettoKapitalIndkomstTilBeskatning = nettoKapitalIndkomstTilBeskatning.Sum();

			if (samletNettoKapitalIndkomstTilBeskatning <= 0)
			{
				// TODO: Gør som individuelt her pga sambeskatning, altså ingen beskatning af kapital indkomst for nogen af ægtefællerne
			
			}
			else
			{
				nettoKapitalIndkomstTilBeskatning = nettoKapitalIndkomstTilBeskatning.NedbringPositivtMedEvtNegativt();

				// TODO
				var max  = grundlagUdenNettokapitalindkomst.Max();
				if (grundlagUdenNettokapitalindkomst[0] > grundlagUdenNettokapitalindkomst[1])
				{
					
				}
				else
				{
					
				}
			}

			// TODO
			return null;
		}
	}
}