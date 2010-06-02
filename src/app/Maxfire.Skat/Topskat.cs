using System;
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
		/// <summary>
		/// Beregn den del af nettokapitalindkomsten, der overstiger det grundbeløb, som angiver 
		/// bundfradraget for positiv nettokapitalindkomst.
		/// </summary>
		public ValueTuple<decimal> BeregnNettoKapitalIndkomstTilBeskatning(ValueTuple<PersonligeBeloeb> input)
		{			
			var bundfradrag = BeregnBundfradragForPositivKapitalIndkomst(input);
			var nettokapitalindkomst = input.Map(x => x.NettoKapitalIndkomstSkattegrundlag);
			var nettokapitalindkomstTilBeskatning = nettokapitalindkomst - bundfradrag;
			return nettokapitalindkomstTilBeskatning;
		}

		/// <summary>
		/// Beregn størrelsen af hver persons bundfradrag for positiv nettokapitalindkomst under 
		/// hensyntagen til reglerne om overførsel af ikke-udnyttet bundfradreg mellem ægtefæller.
		/// </summary>
		public ValueTuple<decimal> BeregnBundfradragForPositivKapitalIndkomst(ValueTuple<PersonligeBeloeb> input)
		{
			if (input.Size == 1)
			{
				return Constants.BundfradragPositivKapitalIndkomst.ToTuple();
			}

			// §7, stk 3: ikke-udnyttet bundfradrag (grundbeløb på 40.000 kr, 2010) kan 
			// overføres mellem ægtefæller (sambeskating af ægtefæller).
			Func<PersonligeBeloeb, bool> fuldUdnyttelseAfBundfradrag = x => 
				x.NettoKapitalIndkomstSkattegrundlag >= Constants.BundfradragPositivKapitalIndkomst;


			// TODO: Kan optimeres til een og kun een har fuld udnyttelse => den anden med uudnyttet overfører til den med fuld udnyttelse, men skriv specs først

			if (input.All(fuldUdnyttelseAfBundfradrag))
			{
				return Constants.BundfradragPositivKapitalIndkomst.ToTuple(Constants.BundfradragPositivKapitalIndkomst);
			}

			// Der er uudnyttet bundfradrag hos mindst en ægtefælle, men kan det udnyttes hos den anden ægtefælle
			var overfoeresTil = input.FirstOrDefault(fuldUdnyttelseAfBundfradrag);
			if (overfoeresTil != null)
			{
				var overfoeresFra = input.PartnerOf(overfoeresTil);
				
				decimal fraBundfradrag = overfoeresFra.NettoKapitalIndkomstSkattegrundlag.NonNegative();
				decimal tilBundfradrag = 2 * Constants.BundfradragPositivKapitalIndkomst - fraBundfradrag;

				return input.IndexOf(overfoeresTil) == 0 ? tilBundfradrag.ToTuple(fraBundfradrag) : 
					fraBundfradrag.ToTuple(tilBundfradrag);
			}

			// Begge ægtefæller har et uudnyttet bundfradrag, så ingen overførsel
			return Constants.BundfradragPositivKapitalIndkomst.ToTuple(Constants.BundfradragPositivKapitalIndkomst);
		}
	}

	/// <summary>
	/// Implementering af §7, stk. 2-10, i LBK nr. 959, der beskriver regler for beregning af topskat.
	/// </summary>
	public class TopskatBeregner
	{
		public ValueTuple<decimal> BeregnSkat(ValueTuple<PersonligeBeloeb> indkomster)
		{
			ValueTuple<decimal> topskat;
			var topskatGrundlagBeregner = new TopskatGrundlagBeregner();

			var personligIndkomst = indkomster.Map(x => x.PersonligIndkomstSkattegrundlag);
			var nettokapitalindkomst = indkomster.Map(x => x.NettoKapitalIndkomstSkattegrundlag);
			var kapitalPensionsindskud = indkomster.Map(x => x.KapitalPensionsindskud);

			// TODO: Er dette ikke samme algoritme uanset Size
			if (indkomster.Size == 1)
			{
				// Beregningsgrundlaget for topskatten for en enlig person opgøres efter PSL § 7, stk. 1, 
				// som den personlige indkomst med tillæg af heri fradragne og ikke medregnede beløb omfattet 
				// af beløbsgrænsen i PBL § 16, stk. 1, og med tillæg af positiv nettokapitalindkomst. 
				// PBL § 16, stk. 1, omhandler indskud til kapitalpensionsordninger, kapitalforsikring og 
				// bidrag til supplerende engangsydelser fra pensionskasser. Fra og med indkomståret 2010 beregnes 
				// topskat af kapitalindkomst kun af kapitalindkomst ud over 40.000 kr. (2010) - for ægtefæller 
				// 80.000 kr. (2010), jf. Forårspakke 2.0.
				var kapitalIndkomstTilSkat = +(nettokapitalindkomst - Constants.BundfradragPositivKapitalIndkomst);

				var grundlag = +(personligIndkomst + kapitalIndkomstTilSkat
								 + kapitalPensionsindskud - Constants.TopskatBundfradrag);

				topskat = Constants.Topskattesats * grundlag;
				// Note: L 112 og 'nedsættelse på kapitalindkomst' er skippet
				return topskat;
			}
			
			// §7, stk 4: For hver ægtefælle beregnes skat med 15 pct. af vedkommendes personlige indkomst 
			// med tillæg af heri fradragne og ikke medregnede beløb omfattet af beløbsgrænsen i 
			// pensionsbeskatningslovens § 16, stk. 1, i det omfang dette beregningsgrundlag overstiger topskattegrænsen 
			var grundlagUdenNettokapitalindkomst = personligIndkomst + kapitalPensionsindskud - Constants.TopskatBundfradrag;
			var topskatUdenPositivNettoKapitalIndkomst = Constants.Topskattesats * grundlagUdenNettokapitalindkomst;

			// §7, stk 5: Der beregnes tillige skat af ægtefællernes samlede positive nettokapitalindkomst. Til 
			// dette formål beregnes en skat hos den af ægtefællerne, der har det højeste beregningsgrundlag 
			// efter stk. 4. Skatten beregnes med 15 pct. af denne ægtefælles beregningsgrundlag efter stk. 4 
			// med tillæg af ægtefællernes samlede positive nettokapitalindkomst. Skatten beregnes dog kun i 
			// det omfang, det samlede beløb overstiger topskattegrænsen. 
			var nettoKapitalIndkomstTilBeskatning = topskatGrundlagBeregner.BeregnNettoKapitalIndkomstTilBeskatning(indkomster);
			var samletNettoKapitalIndkomstTilBeskatning = nettoKapitalIndkomstTilBeskatning.Sum();

			if (samletNettoKapitalIndkomstTilBeskatning <= 0)
			{
				// Det andet led udgår
				topskat = +topskatUdenPositivNettoKapitalIndkomst;
			}
			else
			{
				// Det andet led af topskatten mht positiv nettokapitalindkomst beregnes
				var positivNettoKapitalIndkomstTilBeskatning = nettoKapitalIndkomstTilBeskatning.NedbringPositivtMedEvtNegativt();

				int indexOfMaxGrundlag;
				if (grundlagUdenNettokapitalindkomst[0] > grundlagUdenNettokapitalindkomst[1])
				{
					indexOfMaxGrundlag = 0;
				} 
				else if (grundlagUdenNettokapitalindkomst[0] < grundlagUdenNettokapitalindkomst[1])
				{
					indexOfMaxGrundlag = 1;
				}
				else
				{
					// Hvis ægtefællernes beregningsgrundlag er lige store anses den af ægtefællerne, som har 
					// de største udgifter af den art, der fradrages ved opgørelsen af den skattepligtige indkomst, 
					// men ikke ved opgørelsen af personlig indkomst og kapitalindkomst (ligningsmæssige fradrag), 
					// for at have det højeste beregningsgrundlag efter pkt. A.
					indexOfMaxGrundlag = indkomster[0].LigningsmaessigeFradrag > indkomster[1].LigningsmaessigeFradrag ? 0 : 1;
				}

				decimal topskatInklSamletPositivNettoKapitalIndkomst
					= Constants.Topskattesats * (grundlagUdenNettokapitalindkomst[indexOfMaxGrundlag] + samletNettoKapitalIndkomstTilBeskatning);

				// §7, Stk. 6: Forskellen mellem skatten efter stk. 5 og skatten efter stk. 4
				// for den ægtefælle, der har det højeste beregningsgrundlag efter stk. 4,
				// udgør skatten af ægtefællernes samlede positive nettokapitalindkomst.
				decimal samletTopskatAfPositivNettoKapitalIndkomst
					= topskatInklSamletPositivNettoKapitalIndkomst - topskatUdenPositivNettoKapitalIndkomst[indexOfMaxGrundlag];

				var fordelingsnoegle = positivNettoKapitalIndkomstTilBeskatning / samletNettoKapitalIndkomstTilBeskatning;

				topskat = +topskatUdenPositivNettoKapitalIndkomst + fordelingsnoegle * samletTopskatAfPositivNettoKapitalIndkomst;
			}

			return topskat.RoundMoney();
		}
	}
}