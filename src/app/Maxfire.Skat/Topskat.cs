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
		private readonly ISkattelovRegistry _skattelovRegistry;

		public TopskatBeregner(ISkattelovRegistry skattelovRegistry)
		{
			_skattelovRegistry = skattelovRegistry;
		}

		public ValueTuple<decimal> BeregnSkat(ValueTuple<PersonligeBeloeb> indkomster, int skatteAar, ValueTuple<KommunaleSatser> kommunaleSatser=null)
		{
			var personligIndkomst = indkomster.Map(x => x.PersonligIndkomstSkattegrundlag);
			var nettokapitalindkomst = indkomster.Map(x => x.NettoKapitalIndkomstSkattegrundlag);
			var kapitalPensionsindskud = indkomster.Map(x => x.KapitalPensionsindskudSkattegrundlag);
			var topskatBundfradrag = _skattelovRegistry.GetTopskatBundfradrag(skatteAar);

			ValueTuple<decimal> topskatteGrundlag;

			if (indkomster.Size == 1)
			{
				var positivNettoKapitalIndkomstTilBeskatning = +(nettokapitalindkomst - _skattelovRegistry.GetPositivNettoKapitalIndkomstBundfradrag(skatteAar));

				topskatteGrundlag = +(personligIndkomst + positivNettoKapitalIndkomstTilBeskatning
								 + kapitalPensionsindskud - topskatBundfradrag);
			}
			else
			{
				var grundlagUdenPositivNettoKapitalIndkomst = personligIndkomst + kapitalPensionsindskud - topskatBundfradrag;
				var nettoKapitalIndkomstTilBeskatning = BeregnNettoKapitalIndkomstTilBeskatning(indkomster, skatteAar);
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

			return beregnSkatUnderSkatteloft(topskatteGrundlag, kommunaleSatser, skatteAar);
		}

		private ValueTuple<decimal> beregnSkatUnderSkatteloft(ValueTuple<decimal> grundlag, ValueTuple<KommunaleSatser> kommunaleSatser, int skatteAar)
		{
			kommunaleSatser = kommunaleSatser ?? new ValueTuple<KommunaleSatser>(grundlag.Size, () => new KommunaleSatser());
			decimal bundskattesats = _skattelovRegistry.GetBundSkattesats(skatteAar);
			decimal mellemskattesats = _skattelovRegistry.GetMellemSkattesats(skatteAar);
			decimal topskattesats = _skattelovRegistry.GetTopSkattesats(skatteAar);
			decimal sundhedsbidragsats = _skattelovRegistry.GetSundhedsbidragSkattesats(skatteAar);
			decimal skatteloftsats = _skattelovRegistry.GetSkatteloftSkattesats(skatteAar);
			var kommunaleskattesatser = kommunaleSatser.Map(x => x.Kommuneskattesats);
			decimal fastsats = bundskattesats + mellemskattesats + topskattesats + sundhedsbidragsats;
			var skattesatser = fastsats + kommunaleskattesatser;
			var nedslagssatsen = +(skattesatser - skatteloftsats);
			var topskattesatsen = +(topskattesats - nedslagssatsen);
			var topskat = topskattesatsen * grundlag;
			return topskat.RoundMoney();
		}

		/// <summary>
		/// Beregn den del af nettokapitalindkomsten, der overstiger det grundbeløb, som angiver 
		/// bundfradraget for positiv nettokapitalindkomst.
		/// </summary>
		public ValueTuple<decimal> BeregnNettoKapitalIndkomstTilBeskatning(ValueTuple<PersonligeBeloeb> indkomster, int skatteAar)
		{
			var bundfradrag = BeregnBundfradragForPositivKapitalIndkomst(indkomster, skatteAar);
			var nettokapitalindkomst = indkomster.Map(x => x.NettoKapitalIndkomstSkattegrundlag);
			var nettokapitalindkomstTilBeskatning = nettokapitalindkomst - bundfradrag;
			return nettokapitalindkomstTilBeskatning;
		}

		/// <summary>
		/// Beregn størrelsen af hver persons bundfradrag for positiv nettokapitalindkomst under 
		/// hensyntagen til reglerne om overførsel af ikke-udnyttet bundfradreg mellem ægtefæller.
		/// </summary>
		public ValueTuple<decimal> BeregnBundfradragForPositivKapitalIndkomst(ValueTuple<PersonligeBeloeb> indkomster, int skatteAar)
		{
			decimal bundfradragPositivNettoKapitalIndkomst = _skattelovRegistry.GetPositivNettoKapitalIndkomstBundfradrag(skatteAar);
			if (indkomster.Size == 1)
			{
				return bundfradragPositivNettoKapitalIndkomst.ToTuple();
			}

			// §7, stk 3: ikke-udnyttet bundfradrag (grundbeløb på 40.000 kr, 2010) kan 
			// overføres mellem ægtefæller (sambeskating af ægtefæller).
			Func<PersonligeBeloeb, bool> fuldUdnyttelseAfBundfradrag = x =>
				x.NettoKapitalIndkomstSkattegrundlag >= bundfradragPositivNettoKapitalIndkomst;

			// TODO: Kan optimeres til een og kun een har fuld udnyttelse => den anden med uudnyttet overfører til den med fuld udnyttelse, men skriv specs først

			if (indkomster.All(fuldUdnyttelseAfBundfradrag))
			{
				return bundfradragPositivNettoKapitalIndkomst.ToTuple(bundfradragPositivNettoKapitalIndkomst);
			}

			// Der er uudnyttet bundfradrag hos mindst en ægtefælle, men kan det udnyttes hos den anden ægtefælle
			var overfoeresTil = indkomster.FirstOrDefault(fuldUdnyttelseAfBundfradrag);
			if (overfoeresTil != null)
			{
				var overfoeresFra = indkomster.PartnerOf(overfoeresTil);

				decimal fraBundfradrag = overfoeresFra.NettoKapitalIndkomstSkattegrundlag.NonNegative();
				decimal tilBundfradrag = 2 * bundfradragPositivNettoKapitalIndkomst - fraBundfradrag;

				return indkomster.IndexOf(overfoeresTil) == 0 ? tilBundfradrag.ToTuple(fraBundfradrag) :
					fraBundfradrag.ToTuple(tilBundfradrag);
			}

			// Begge ægtefæller har et uudnyttet bundfradrag, så ingen overførsel
			return bundfradragPositivNettoKapitalIndkomst.ToTuple(bundfradragPositivNettoKapitalIndkomst);
		}
	}
}