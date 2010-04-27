using System.Collections.Generic;
using System.Linq;

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
		public IList<decimal> BeregnGrundlagUdenNettokapitalindkomst(IList<Indkomster> input)
		{
			return input.Select(x => x.PersonligIndkomst + x.KapitalPensionsindskud).ToList();
		}

		public IList<decimal> BeregnBundfradragForPositivKapitalIndkomst(IList<Indkomster> input)
		{
			const decimal bundfradragPositivkapitalIndkomst = 40000; // 2010 niveau
			
			if (input.IsIndividual())
			{
				return bundfradragPositivkapitalIndkomst.AsIndividual();
			}

			// §7, stk 3: Uudnyttet bundfradrag (grundbeløb på 40.000 kr, 2010) kan 
			// overføres mellem ægtefæller (sambeskating af ægtefæller).
			var first = input.First(); var second = input.Second();
			
			// Hvis begge benytter bundfradraget fuldt ud kan der ikke ske overførsel
			if (first.NettoKapitalIndkomst >= bundfradragPositivkapitalIndkomst 
			    && second.NettoKapitalIndkomst >= bundfradragPositivkapitalIndkomst)
			{
				return bundfradragPositivkapitalIndkomst.AsMarried(bundfradragPositivkapitalIndkomst);
			}

			// Der er uudnyttet bundfradrag hos mindst en ægtefælle, men kan det udnyttes hos den anden ægtefælle
			var overfoeresTil = input.FirstOrDefault(x => x.NettoKapitalIndkomst >= bundfradragPositivkapitalIndkomst);
			if (overfoeresTil != null)
			{
				var overfoeresFra = input.PartnerOf(overfoeresTil);
				
				decimal fraBundfradrag = overfoeresFra.NettoKapitalIndkomst.NonNegative();
				decimal tilBundfradrag = 2 * bundfradragPositivkapitalIndkomst - fraBundfradrag;

				return input.IndexOf(overfoeresTil) == 0 ? tilBundfradrag.AsMarried(fraBundfradrag) : 
				                                                                                    	fraBundfradrag.AsMarried(tilBundfradrag);
			}

			// Begge ægtefæller har et uudnyttet bundfradrag, så ingen overførsel
			return bundfradragPositivkapitalIndkomst.AsMarried(bundfradragPositivkapitalIndkomst);
		}

		public IList<decimal> BeregnGrundlag(IList<Indkomster> input)
		{
			var first = input.First();

			if (input.IsIndividual())
			{
				decimal kapitalIndkomstTilSkat = (first.NettoKapitalIndkomst - Constants.BundfradragPositivKapitalIndkomst).NonNegative();

				decimal grundlag = (first.PersonligIndkomst + kapitalIndkomstTilSkat
				                    + first.KapitalPensionsindskud - Constants.TopskatBeloebsgraense).NonNegative();

				return grundlag.AsIndividual();
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
		public IList<decimal> BeregnSkat(IList<Indkomster> input)
		{
			TopskatGrundlagBeregner topskatGrundlagBeregner = new TopskatGrundlagBeregner();

			if (input.IsIndividual())
			{
				var grundlag = topskatGrundlagBeregner.BeregnGrundlag(input);
				decimal topskat = grundlag.First() * Constants.Topskattesats;
				// Note: Jeg forstår ikke TFS's beregning af 'nedsættelse på kapitalindkomst' 
				return topskat.AsIndividual();
			}

			// §7, stk 4: For hver ægtefælle beregnes skat med 15 pct. af vedkommendes personlige indkomst 
			// med tillæg af heri fradragne og ikke medregnede beløb omfattet af beløbsgrænsen i 
			// pensionsbeskatningslovens § 16, stk. 1, i det omfang dette beregningsgrundlag overstiger et bundfradrag på 190.000 kr. 
			var grundlagUdenNettokapitalindkomst = topskatGrundlagBeregner.BeregnGrundlagUdenNettokapitalindkomst(input);

			//decimal topskat1 = grundlagUdenNettokapitalindkomst * Constants.Topskattesats;


			// TODO
			return null;
		}
	}

}