namespace Maxfire.Skat
{
	// TODO: Benyt denne i eksempler
	public class IndkomstOpgoerelseBeregner
	{
		private readonly ISkattelovRegistry _skattelovRegistry;

		public IndkomstOpgoerelseBeregner(ISkattelovRegistry skattelovRegistry)
		{
			_skattelovRegistry = skattelovRegistry;
		}

		// TODO: Mangler aktieindkomst, restskat, fremf�rte underskud i selvangivne bel�b
		public ValueTuple<PersonligeBeloeb> BeregnIndkomster(ValueTuple<ISelvangivneBeloeb> selvangivneBeloeb, int skatteAar)
		{
			// Personlig indkomst (herunder PBL � 16, stk. 1 indskud til kapitalpensionsordninger, 
			// hvor der maksimalt kan indbetales 46.000 kr �rligt i 2010)
			var amBidragBeregner = new AMBidragBeregner(_skattelovRegistry);
			var amIndkomster = selvangivneBeloeb.Map(x => x.PersonligIndkomstAMIndkomst);
			var amBidrag = amBidragBeregner.BeregnSkat(amIndkomster, skatteAar);
			var personligIndkomstFoerAMBidrag = selvangivneBeloeb.Map(x => 
				x.PersonligIndkomstAMIndkomst + x.PersonligIndkomstEjAMIndkomst - x.FradragPersonligIndkomst);
			var personligIndkomstEfterAMBidrag = personligIndkomstFoerAMBidrag - amBidrag;
			var kapitalPensionsindskud = selvangivneBeloeb.Map(x => x.KapitalPensionsindskud);

			// Kapital indkomst
			var nettoKapitalIndkomst = selvangivneBeloeb.Map(x => x.KapitalIndkomst - x.FradragKapitalIndkomst);

			// Ligningsm�ssige fradrag
			var beskaeftigelsesfradragBeregner = new BeskaeftigelsesfradragBeregner(_skattelovRegistry);
			var beskaeftigelsesfradrag = beskaeftigelsesfradragBeregner.BeregnFradrag(amIndkomster, skatteAar);
			var ligningsmaesigeFradrag = selvangivneBeloeb.Map(x => x.LigningsmaessigeFradragMinusBeskaeftigelsesfradrag) +
			                             beskaeftigelsesfradrag;

			// TODO: make PersonligeBeloeb immutable, men g�r det sidel�bende med et beregnet eksempel med alle slags modregninger af underskud
			return amBidrag.Map(index => new PersonligeBeloeb
			                                  	{
													PersonligIndkomstFoerAMBidrag = personligIndkomstFoerAMBidrag[index],
													AMIndkomst = amIndkomster[index],
													AMBidrag = amBidrag[index],
													PersonligIndkomst = personligIndkomstEfterAMBidrag[index],
													NettoKapitalIndkomst = nettoKapitalIndkomst[index],
													LigningsmaessigeFradrag = ligningsmaesigeFradrag[index],
													Beskaeftigelsesfradrag = beskaeftigelsesfradrag[index],
													KapitalPensionsindskud = kapitalPensionsindskud[index]
			                                  	});
		}
	}
}