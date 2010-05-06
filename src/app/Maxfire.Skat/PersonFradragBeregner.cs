using System.Collections.Generic;

namespace Maxfire.Skat
{
	// Fuldt skattepligtige personer her i landet har et personfradrag på 42.900 kr. (2009 og 2010), 
	// jf. PSL § 10, stk. 1. Dog udgør personfradraget et beløb på 32.200 kr. (2009 og 2010) for 
	// personer, som ved indkomstårets udløb ikke er fyldt 18 år og ikke har indgået ægteskab, jf. 
	// PSL 10, stk. 2.
	//
	// Der gives personfradrag ved skatteberegningen, idet de beregnede skatter nedsættes med skatteværdien 
	// af personfradraget, jf. PSL § 9.
	//
	// Hvis skatteværdien af det statslige personfradrag ikke kan udnyttes ved beregningen af sundhedsbidrag, 
	// bundskat, (mellemskat i 2009), topskat eller skat af aktieindkomst over 48.300 kr (2009 og 2010), 
	// anvendes den ikke udnyttede del til nedsættelse af de øvrige skatter (dvs. de kommunale indkomstskatter 
	// og kirkeskatten, jf. min fortolkning), jf. PSL § 12, stk. 2.
	//
	// Hvis en gift person, der er samlevende med ægtefællen ved indkomstårets udløb, efter modregning af 
	// skatteværdier af personfradragene i egne indkomstskatter har uudnyttede skatteværdier, omregnes disse 
	// til fradragsbeløb og overføres til ægtefællen. Her beregnes skatteværdierne med ægtefællens egne 
	// skatteprocenter, og der foretages en lignende modregning som ovenfor anført. Et herefter uudnyttet 
	// personfradrag kan ikke fremføres til anvendelse i efterfølgende indkomstår.
	//
	// Indkomstskat til staten, dvs. sundhedsbidrag, bundskat, (mellemskat i 2009) og topskat, skal, efter 
	// at skattebeløbene er reguleret med evt. underskud, nedsættes med skatteværdien af personfradrag. 
	// 
	// I det omfang skatteværdien af personfradraget beregnet med skatteprocenten vedrørende sundhedsbidraget 
	// ikke kan fradrages i sundhedsbidraget, fragår den i rækkefølgen bundskat, (mellemskat i 2009), topskat 
	// og skat af aktieindkomst, der overstiger 48.300 kr. (2009 og 2010). Tilsvarende, hvis skatteværdien 
	// af personfradraget beregnet med bundskatteprocenten ikke kan fradrages i bundskatten, fragår den i 
	// rækkefølgen i sundhedsbidraget, (mellemskat i 2009), topskat og skat af aktieindkomst, der overstiger 
	// 48.300 kr. (2009 og 2010). Indkomstskat for begrænset skattepligtige m.v. og indkomstskat til kommunen 
	// samt kirkeskat nedsættes på tilsvarende måde.
	//
	// Hvis skatteværdien af personfradraget ved beregning af henholdsvis indkomstskat til staten og de 
	// kommunale indkomstskatter overstiger det beregnede skattebeløb, anvendes den uudnyttede del af 
	// skatteværdien til nedsættelse af de øvrige indkomstskatter (dvs. de kommunale indkomstskatter og 
	// kirkeskatten, jf. min fortolkning).
	public class PersonfradragBeregner
	{
		public ValueTuple<Skatter> BeregnSkatEfterPersonfradrag(ValueTuple<PersonligeBeloeb> indkomster,
		                                  ValueTuple<Skatter> skatter, ValueTuple<KommunaleSatser> kommunaleSatser)
		{
			var skattevaerdier = BeregnSkattevaerdierAfPersonfradrag(kommunaleSatser);

			// Sundhedsbidraget

				// Hvis ikke-udnyttet skatteværdi => fradrag i bundskat, topskat og skat af aktieindkomst (lav generel rutine)

			// Bundskat

				// Hvis ikke-udnyttet skatteværdi => fradrag i sundhedsbidrag, topskat og skat af aktieindkomst (lav generel rutine)

			// Kommuneskat

			
			// Kirkeskat


			ValueTuple<Skatter> modregnedeSkatter = null;

			return modregnedeSkatter;
		}

		/// <summary>
		/// Beregn skatteværdier af personfradraget for kommuneskat, kirkeskat, bundskat og sundhedsbidrag.
		/// </summary>
		/// <returns></returns>
		public ValueTuple<Skatter> BeregnSkattevaerdierAfPersonfradrag(ValueTuple<KommunaleSatser> kommunaleSatser)
		{
			var kommuneskattesats = kommunaleSatser.Map(x => x.Kommuneskattesats);
			var kirkeskattesats = kommunaleSatser.Map(x => x.Kirkeskattesats);
			
			decimal skattevaerdiBundskat = Constants.Bundskattesats * Constants.Personfradrag;
			decimal skattevaerdiSundhedsbidrag = Constants.Sundhedsbidragsats * Constants.Personfradrag;
			var skattevaerdiKommuneskat = kommuneskattesats * Constants.Personfradrag;
			var skattevaerdiKirkeskat = kirkeskattesats * Constants.Personfradrag;

			var list = new List<Skatter>(kommunaleSatser.Size);
			for (int i = 0; i < kommunaleSatser.Size; i++)
			{
				list.Add(new Skatter
				         	{
				         		Bundskat = skattevaerdiBundskat,
				         		Sundhedsbidrag = skattevaerdiSundhedsbidrag,
				         		Kommuneskat = skattevaerdiKommuneskat[i],
				         		Kirkeskat = skattevaerdiKirkeskat[i]
				         	});
			}

			return new ValueTuple<Skatter>(list);
		}
	}
}