namespace Maxfire.Skat
{
	// Note: Der indføres en kompensationsordning, så personer med særligt store fradrag (inkl. ligningsmæssige
	// Note: fradrag) ikke kan miste mere som følge af begrænsningerne af fradrag, end de får i indkomstskattelettelser.
	/////////////////////////////////////////////////////////////////////////////////////////
	//
	// PSL § 26. 
	// 
	// Stk. 1 For indkomstårene 2012-2019 beregnes en kompensation, hvis forskelsbeløbet 
	// beregnet efter stk. 2 og 3 er negativt. Kompensationen modregnes i skatterne efter 
	// §§ 6, 7, 8 og § 8 a, stk. 2, indkomstskat til kommunen og kirkeskat i den nævnte 
	// rækkefølge.
	//
	// Stk. 2. I beregningen af forskelsbeløbet indgår følgende beløb:
	//
	// 1) 1,5 pct. af grundlaget for bundskatten, jf. § 6,
	// i det omfang grundlaget overstiger et bundfradrag
	// på 44.800 kr. (2010-niveau). For personer, som ved indkomstårets udløb ikke
	// er fyldt 18 år og ikke har indgået ægteskab,
	// er bundfradraget 33.600 kr. (2010-niveau).
	//
	// 2) 6 pct. af den personlige indkomst med tillæg
	// af positiv nettokapitalindkomst, i det omfang
	// grundlaget overstiger et bundfradrag på
	// 362.800 kr. (2010-niveau).
	//
	// 3) 15 pct. af grundlaget for topskat, jf. § 7, stk.
	// 1, i det omfang grundlaget overstiger et
	// bundfradrag på 362.800 kr. (2010-niveau)
	// fratrukket 15 pct. af grundlaget for topskat,
	// jf. § 7, stk. 1, i det omfang grundlaget overstiger
	// bundfradraget anført i § 7, stk. 2.
	//
	// 4) 1 pct. af grundlaget for aktieindkomstskat,
	// jf. § 8 a, stk. 1 og 2.
	//
	// 5) 8 pct. plus procenten ved beregning af indkomstskat
	// til kommunen og kirkeskat af fradraget
	// beregnet efter ligningslovens § 9 J,
	// fratrukket et fradrag opgjort på samme
	// grundlag ved anvendelse af en fradragsprocent
	// på 4,25 og et grundbeløb på 14.200 kr.
	// (2010-niveau).
	//
	// 6) Skatteværdien opgjort efter § 12 af et grundbeløb
	// på 1.900 kr. (2010-niveau).
	//
	// 7) 8 pct. minus skatteprocenten for sundhedsbidraget,
	// jf. § 8, af summen af negativ nettokapitalindkomst,
	// der overstiger beløbsgrænsen i § 11, stk. 1, og udgifter 
	// af den art, der fradrages ved opgørelsen af den skattepligtige
	// indkomst, men ikke ved opgørelsen af personlig
	// indkomst og kapitalindkomst (ligningsmæssige fradrag).
	//
	/////////////////////////////////////////////////////////////////////////////////////////
	public class KompensationBeregner
	{
		private readonly ISkattelovRegistry _skattelovRegistry;

		public KompensationBeregner(ISkattelovRegistry skattelovRegistry)
		{
			_skattelovRegistry = skattelovRegistry;
		}

		/// <summary>
		/// Beregn det nedslag i skatten der gives som følge af PSL § 26 (kompensationsordning fra forårspakke 2.0)
		/// </summary>
		public ValueTuple<decimal> BeregnKompensation(ValueTuple<Person> personer, ValueTuple<PersonligeBeloeb> indkomster, int skatteAar)
		{
			// TODO: Satser er hårdkodede
			// Note: Med tidligere gældende lovgivning var satsen i 2010 5,26 pct.
			// TODO: Satsen er jo nedsat 5,26 pct. fra x til 3,67 pct. hvilket er en nedsættelse på 1,59 pct. point
			
			// 1) Skattelettelse ved nedsættelse af bundskattesatsen
			var bundskatBeregner = new BundskatBeregner(_skattelovRegistry);
			var bundLettelseBundfradrag 
				= personer.Map(person => _skattelovRegistry.GetBundLettelseBundfradrag(skatteAar, person.GetAlder(skatteAar), personer.Size > 1));
			var bundSkattelettelse = 0.015m * bundskatBeregner.BeregnBruttoGrundlag(indkomster).DifferencesGreaterThan(bundLettelseBundfradrag);

			// 2) Skattelettelse ved Fjernelse af mellemskatten
			var mellemskatBeregner = new MellemskatBeregner(_skattelovRegistry);
			var mellemLettelseBundfradrag = _skattelovRegistry.GetMellemLettelseBundfradrag(skatteAar);
			var mellemSkattelettelse = 0.06m * mellemskatBeregner.BeregnGrundlag(indkomster, mellemLettelseBundfradrag);

			// 3) Skattelettelse ved forøgelse af topskattegrænsen
			var topskatBeregner = new TopskatBeregner(_skattelovRegistry);
			var topLettelseBundfradrag = _skattelovRegistry.GetTopLettelseBundfradrag(skatteAar);
			var topskatBundfradrag = _skattelovRegistry.GetTopskatBundfradrag(skatteAar);
			var topskatteGrundlagMedTidligereBundfradrag = topskatBeregner.BeregnGrundlag(indkomster, topLettelseBundfradrag);
			var topskatteGrundlagMedNuvaerendeBundfradrag = topskatBeregner.BeregnGrundlag(indkomster, topskatBundfradrag);
			var topSkattelettelse = 0.15m * (topskatteGrundlagMedTidligereBundfradrag - topskatteGrundlagMedNuvaerendeBundfradrag);

			// 4) Skattelettelse ved nedsættelse af aktieindkomstskattesatsen
			var aktieSkattelettelse = 0; // TODO

			// 5) Skattelettelse ved forhøjelse af beskæftigelsesfradraget
			var beskaeftigelsesfradragSkattelettelse = 0; // TODO

			// 6) Skatteskærpelse ved nulregulering af personfradraget
			var personFradragSkatteskaerpelse = 0; // TODO;

			// 7)
			var fradragsSkatteskaerpelse = 0;

			var forskelsbeloeb =
				(+(bundSkattelettelse + mellemSkattelettelse + topSkattelettelse + aktieSkattelettelse +
				 beskaeftigelsesfradragSkattelettelse - personFradragSkatteskaerpelse)) - fradragsSkatteskaerpelse;

			return forskelsbeloeb;
		}

		private static SkatteModregner<Skatter> getSkatteModregner()
		{
			return new SkatteModregner<Skatter>(
				Modregning<Skatter>.Af(x => x.Bundskat),
				Modregning<Skatter>.Af(x => x.Topskat),
				Modregning<Skatter>.Af(x => x.Sundhedsbidrag),
				Modregning<Skatter>.Af(x => x.AktieindkomstskatOverGrundbeloebet),
				Modregning<Skatter>.Af(x => x.Kommuneskat),
				Modregning<Skatter>.Af(x => x.Kirkeskat)
			);
		}
	}
}