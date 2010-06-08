using System;
using System.Linq;

namespace Maxfire.Skat
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////
	//
	// PSL § 7: Topskatteberegningen
	// =============================
	//
	// Stk. 1. Skatten efter § 5, nr. 3, beregnes af den personlige indkomst med tillæg af heri fradragne 
	// og ikke medregnede beløb omfattet af beløbsgrænsen i pensionsbeskatningslovens § 16, stk. 1, og 
	// med tillæg af positiv kapitalindkomst.
	// Stk. 2. Skatten udgør 15 pct. af det i stk. 1 nævnte beregningsgrundlag, i det omfang det overstiger 
	// et bundfradrag på 347.200 kr (2009 og 2010).
	// Stk. 3. For ægtefæller beregnes skatten efter stk. 4-9, når de er samlevende i hele indkomståret 
	// og dette udgør en periode af et helt år.
	// Stk. 4. For hver ægtefælle beregnes skat med 15 pct. af vedkommendes personlige indkomst med tillæg 
	// af heri fradragne og ikke medregnede beløb omfattet af beløbsgrænsen i pensionsbeskatningslovens § 16, stk. 1, 
	// i det omfang dette beregningsgrundlag overstiger et bundfradrag på 347.200 kr (2009 og 2010).
	// Stk. 5. Der beregnes tillige skat af ægtefællernes samlede positive nettokapitalindkomst. Til dette 
	// formål beregnes en skat hos den af ægtefællerne, der har det højeste beregningsgrundlag efter stk. 4. 
	// Skatten beregnes med 15 pct. af denne ægtefælles beregningsgrundlag efter stk. 4 med tillæg af 
	// ægtefællernes samlede positive nettokapitalindkomst. Skatten beregnes dog kun i det omfang, det 
	// samlede beløb overstiger bundfradraget på 347.200 kr (2009 og 2010).
	// Stk. 6. Forskellen mellem skatten efter stk. 5 og skatten efter stk. 4 for den ægtefælle, der har 
	// det højeste beregningsgrundlag efter stk. 4, udgør skatten af ægtefællernes samlede positive nettokapitalindkomst.
	// Stk. 7. Har kun den ene ægtefælle positiv kapitalindkomst, påhviler den samlede skat af ægtefællernes 
	// nettokapitalindkomst efter stk. 6 denne ægtefælle.
	// Stk. 8. Hvis begge ægtefæller har positiv kapitalindkomst, fordeles skatten af ægtefællernes samlede 
	// nettokapitalindkomst imellem dem efter forholdet mellem hver enkelt ægtefælles kapitalindkomst.
	// Stk. 9. Hvis ægtefællernes beregningsgrundlag efter stk. 4 er lige store, anses den af ægtefællerne, 
	// som har de største udgifter af den art, der fradrages ved opgørelsen af den skattepligtige indkomst, 
	// men ikke ved opgørelsen af personlig indkomst og kapitalindkomst, for at have det højeste 
	// beregningsgrundlag efter stk. 4.
	// Stk. 10. Bundfradraget i stk. 2, 4 og 5 reguleres efter § 20.
	//
	// Forårspakke 2.0
	// ===============
	// 
	// Fra og med indkomståret 2010 beregnes topskat af kapitalindkomst kun af kapitalindkomst ud over 
	// 40.000 kr. (2010) - for ægtefæller 80.000 kr. (2010), jf. Forårspakke 2.0.
	//
	// PBL § 16, stk. 1
	// ================
	//
	// Dette beløb omhandler indskud til kapitalpensionsordninger, kapitalforsikring og bidrag til supplerende 
	// engangsydelser fra pensionskasser.
	//
	// PSL § 19: Skatteloftet
	// ======================
	//
	// Hvis summen af skatteprocenterne efter §§ 6, 6 a og 7 og 8 tillagt den skattepligtiges kommunale 
	// indkomstskatteprocent henholdsvis skatteprocenten efter § 8 c overstiger 59 pct. (51,5 pct i 2010), 
	// beregnes et nedslag i statsskatten (topskatten helt præcist) svarende til, at skatteprocenten 
	// efter § 7 blev nedsat med de overskydende procenter.
	//
	//////////////////////////////////////////////////////////////////////////////////////////////////////////
	public class TopskatBeregner
	{
		public ValueTuple<decimal> BeregnSkat(ValueTuple<PersonligeBeloeb> indkomster, ValueTuple<KommunaleSatser> kommunaleSatser=null)
		{
			var personligIndkomst = indkomster.Map(x => x.PersonligIndkomstSkattegrundlag);
			var nettokapitalindkomst = indkomster.Map(x => x.NettoKapitalIndkomstSkattegrundlag);
			var kapitalPensionsindskud = indkomster.Map(x => x.KapitalPensionsindskudSkattegrundlag);

			ValueTuple<decimal> topskatteGrundlag;

			if (indkomster.Size == 1)
			{
				var positivNettoKapitalIndkomstTilBeskatning = +(nettokapitalindkomst - Constants.BundfradragPositivKapitalIndkomst);

				topskatteGrundlag = +(personligIndkomst + positivNettoKapitalIndkomstTilBeskatning
								 + kapitalPensionsindskud - Constants.TopskatBundfradrag);
			}
			else
			{
				var grundlagUdenPositivNettoKapitalIndkomst = personligIndkomst + kapitalPensionsindskud - Constants.TopskatBundfradrag;
				var nettoKapitalIndkomstTilBeskatning = BeregnNettoKapitalIndkomstTilBeskatning(indkomster);
				var samletNettoKapitalIndkomstTilBeskatning = nettoKapitalIndkomstTilBeskatning.Sum();

				if (samletNettoKapitalIndkomstTilBeskatning <= 0)
				{
					topskatteGrundlag = +grundlagUdenPositivNettoKapitalIndkomst;
				}
				else
				{
					var positivNettoKapitalIndkomstTilBeskatning = nettoKapitalIndkomstTilBeskatning.NedbringPositivtMedEvtNegativt();

					int indexOfMaxGrundlag;
					if (grundlagUdenPositivNettoKapitalIndkomst[0] > grundlagUdenPositivNettoKapitalIndkomst[1])
					{
						indexOfMaxGrundlag = 0;
					}
					else if (grundlagUdenPositivNettoKapitalIndkomst[0] < grundlagUdenPositivNettoKapitalIndkomst[1])
					{
						indexOfMaxGrundlag = 1;
					}
					else
					{
						indexOfMaxGrundlag = indkomster[0].LigningsmaessigeFradrag > indkomster[1].LigningsmaessigeFradrag ? 0 : 1;
					}

					decimal grundlagInklSamletPositivNettoKapitalIndkomst
						= (grundlagUdenPositivNettoKapitalIndkomst[indexOfMaxGrundlag] + samletNettoKapitalIndkomstTilBeskatning).NonNegative();

					decimal samletGrundlagAfPositivNettoKapitalIndkomst
						= grundlagInklSamletPositivNettoKapitalIndkomst - grundlagUdenPositivNettoKapitalIndkomst[indexOfMaxGrundlag].NonNegative();

					var fordelingsnoegle = positivNettoKapitalIndkomstTilBeskatning / samletNettoKapitalIndkomstTilBeskatning;

					topskatteGrundlag = (+grundlagUdenPositivNettoKapitalIndkomst) + fordelingsnoegle * samletGrundlagAfPositivNettoKapitalIndkomst;
				}
			}

			return beregnSkatUnderSkatteloft(topskatteGrundlag, kommunaleSatser);
		}

		private static ValueTuple<decimal> beregnSkatUnderSkatteloft(ValueTuple<decimal> grundlag, ValueTuple<KommunaleSatser> kommunaleSatser)
		{
			kommunaleSatser = kommunaleSatser ?? new ValueTuple<KommunaleSatser>(grundlag.Size, () => new KommunaleSatser());
			var kommunaleskattesatser = kommunaleSatser.Map(x => x.Kommuneskattesats);
			decimal fastsats = Constants.Bundskattesats + Constants.Mellemskattesats 
				+ Constants.Topskattesats + Constants.Sundhedsbidragsats;
			var skattesatser = fastsats + kommunaleskattesatser;
			var nedslagssatsen = +(skattesatser - Constants.Skatteloftsats);
			var topskattesatsen = +(Constants.Topskattesats - nedslagssatsen);
			var topskat = topskattesatsen * grundlag;
			return topskat.RoundMoney();
		}

		/// <summary>
		/// Beregn den del af nettokapitalindkomsten, der overstiger det grundbeløb, som angiver 
		/// bundfradraget for positiv nettokapitalindkomst.
		/// </summary>
		public static ValueTuple<decimal> BeregnNettoKapitalIndkomstTilBeskatning(ValueTuple<PersonligeBeloeb> indkomster)
		{
			var bundfradrag = BeregnBundfradragForPositivKapitalIndkomst(indkomster);
			var nettokapitalindkomst = indkomster.Map(x => x.NettoKapitalIndkomstSkattegrundlag);
			var nettokapitalindkomstTilBeskatning = nettokapitalindkomst - bundfradrag;
			return nettokapitalindkomstTilBeskatning;
		}

		/// <summary>
		/// Beregn størrelsen af hver persons bundfradrag for positiv nettokapitalindkomst under 
		/// hensyntagen til reglerne om overførsel af ikke-udnyttet bundfradreg mellem ægtefæller.
		/// </summary>
		public static ValueTuple<decimal> BeregnBundfradragForPositivKapitalIndkomst(ValueTuple<PersonligeBeloeb> indkomster)
		{
			if (indkomster.Size == 1)
			{
				return Constants.BundfradragPositivKapitalIndkomst.ToTuple();
			}

			// §7, stk 3: ikke-udnyttet bundfradrag (grundbeløb på 40.000 kr, 2010) kan 
			// overføres mellem ægtefæller (sambeskating af ægtefæller).
			Func<PersonligeBeloeb, bool> fuldUdnyttelseAfBundfradrag = x =>
				x.NettoKapitalIndkomstSkattegrundlag >= Constants.BundfradragPositivKapitalIndkomst;

			// TODO: Kan optimeres til een og kun een har fuld udnyttelse => den anden med uudnyttet overfører til den med fuld udnyttelse, men skriv specs først

			if (indkomster.All(fuldUdnyttelseAfBundfradrag))
			{
				return Constants.BundfradragPositivKapitalIndkomst.ToTuple(Constants.BundfradragPositivKapitalIndkomst);
			}

			// Der er uudnyttet bundfradrag hos mindst en ægtefælle, men kan det udnyttes hos den anden ægtefælle
			var overfoeresTil = indkomster.FirstOrDefault(fuldUdnyttelseAfBundfradrag);
			if (overfoeresTil != null)
			{
				var overfoeresFra = indkomster.PartnerOf(overfoeresTil);

				decimal fraBundfradrag = overfoeresFra.NettoKapitalIndkomstSkattegrundlag.NonNegative();
				decimal tilBundfradrag = 2 * Constants.BundfradragPositivKapitalIndkomst - fraBundfradrag;

				return indkomster.IndexOf(overfoeresTil) == 0 ? tilBundfradrag.ToTuple(fraBundfradrag) :
					fraBundfradrag.ToTuple(tilBundfradrag);
			}

			// Begge ægtefæller har et uudnyttet bundfradrag, så ingen overførsel
			return Constants.BundfradragPositivKapitalIndkomst.ToTuple(Constants.BundfradragPositivKapitalIndkomst);
		}
	}
}