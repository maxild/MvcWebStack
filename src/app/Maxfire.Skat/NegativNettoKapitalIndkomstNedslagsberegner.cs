namespace Maxfire.Skat
{
	// Note: Skal ses i sammenh�ng med udfasningen af sundhedsbidraget (sammenl�gning med bundskatten) 
	// Note: og den deraf forringelse af v�rdien af rentefradraget
	// Note: Det vil sige at renteudgifter p� op til 50.000 (100.000 for �gtepar) ikke ber�reres af neds�ttelsen af v�rdien af rentefradraget
	///////////////////////////////////////////////////////////////////////////////////////////
	// PSL � 11: 
	//
	// Stk. 1. For den del af den skattepligtiges negative nettokapitalindkomst, der ikke 
	// overstiger et bel�b p� 50.000 kr., beregnes et nedslag i skatten med den procent, 
	// der anf�res i stk. 2. Nedslaget modregnes i skatterne efter �� 6, 7, 8 og 8 a, stk. 2,
	// indkomstskat til kommunen og kirkeskat i den n�vnte r�kkef�lge.
	// 
	// (Stk 2 viser blot nedslagsprocenten fra 2012-2019)
	//
	// Stk. 3. For en gift person modregnes den anden �gtef�lles positive nettokapitalindkomst 
	// ved opg�relsen af negativ nettokapitalindkomst efter stk. 1, hvis �gtef�llerne er 
	// samlevende ved indkomst�rets udl�b.
	//
	// Stk. 4. Hvis en gift person ikke kan udnytte nedslaget beregnet efter stk. 1 og 2, 
	// modregnes den ikke udnyttede del i den anden �gtef�lles skatter, hvis �gtef�llerne 
	// er samlevende ved indkomst�rets udl�b.
	//
	///////////////////////////////////////////////////////////////////////////////////////////
	public class NegativNettoKapitalIndkomstNedslagBeregner
	{
		private readonly ISkattelovRegistry _skattelovRegistry;

		public NegativNettoKapitalIndkomstNedslagBeregner(ISkattelovRegistry skattelovRegistry)
		{
			_skattelovRegistry = skattelovRegistry;
		}

		public ValueTuple<ModregnSkatterResult<Skatter>> ModregnMedNedslag(ValueTuple<PersonligeBeloeb> indkomster, ValueTuple<Skatter> skatter, int skatteAar)
		{
			var nettoKapitalIndkomst = indkomster.Map(x => x.NettoKapitalIndkomst);
			var nettoKapitalIndkomstEfterModregning = nettoKapitalIndkomst.NedbringPositivtMedEvtNegativt();
			var grundbeloeb = _skattelovRegistry.GetNegativNettoKapitalIndkomstGrundbeloeb(skatteAar);
			var negativNettoKapitalIndkomstEfterModregningDerIkkeOverstigerGrundbeloeb 
				= (+(-nettoKapitalIndkomstEfterModregning)).Loft(grundbeloeb);

			var sats = _skattelovRegistry.GetNegativNettoKapitalIndkomstSats(skatteAar);
			var nedslag = sats * negativNettoKapitalIndkomstEfterModregningDerIkkeOverstigerGrundbeloeb;

			var skatteModregner = getSkatteModregner();

			// Modregn nedslag i egne skatter
			var modregningerFraEgetNedslag = skatteModregner.BeregnModregninger(skatter, nedslag);
			
			if (nettoKapitalIndkomst.Size == 1)
			{
				return new ModregnSkatterResult<Skatter>(skatter[0], nedslag[0], modregningerFraEgetNedslag[0]).ToTuple();
			}
			
			// Modregn evt. uudnyttet nedslag i �gtef�lles skatter
			var udnyttetNedslag = modregningerFraEgetNedslag.Map(x => x.Sum());
			var ikkeUdnyttetNedslag = nedslag - udnyttetNedslag;
			var overfoertNedslag = ikkeUdnyttetNedslag.Swap();
			
			var modregningerFraOverfoertNedslag 
				= skatteModregner.BeregnModregninger(skatter - modregningerFraEgetNedslag, overfoertNedslag);
			var udnyttetOverfortNedslag = modregningerFraOverfoertNedslag.Map(x => x.Sum());

			// Denne tuple repr�senter overf�rslen af udnyttet nedslag (skattev�rdi) mellem �gtef�llerne...
			var overfortUdnyttetNedslag = udnyttetOverfortNedslag - udnyttetOverfortNedslag.Swap();

			//...s�dan at IkkeUdnyttetSkattevaerdi er korrekt p� den returnerede v�rdi
			return skatter.Map((skat, index) => 
				new ModregnSkatterResult<Skatter>(skat, nedslag[index] + overfortUdnyttetNedslag[index],
					modregningerFraEgetNedslag[index] + modregningerFraOverfoertNedslag[index]));
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