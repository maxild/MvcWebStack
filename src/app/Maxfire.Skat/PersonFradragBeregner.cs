using System.Collections.Generic;
using Maxfire.Core.Extensions;
using Maxfire.Core.Reflection;

namespace Maxfire.Skat
{
	// Fuldt skattepligtige personer her i landet har et personfradrag på 42.900 kr. (2009 og 2010), 
	// jf. PSL § 10, stk. 1. Dog udgør personfradraget et beløb på 32.200 kr. (2009 og 2010) for 
	// personer, som ved indkomstårets udløb ikke er fyldt 18 år og ikke har indgået ægteskab, jf. 
	// PSL 10, stk. 2.
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
	// § 9 stk 1.: Bundskat, mellemskat, topskat og sundhedsbidrag og skat af aktieindkomst over 48.300 kr 
	// (2009 og 2010), til staten skal, efter at skattebeløbene er reguleret efter § 13, nedsættes med 
	// skatteværdien af personfradrag.
	//
	// § 12 Stk. 2. Hvis skatteværdien af personfradraget ikke kan udnyttes ved beregningen af en eller 
	// flere af de i § 9 nævnte skatter (dvs. følgende indkomstskatter til staten: bundskat, mellemskat, 
	// topskat, sundhedsbidrag, skat af aktieindkomst, der overstiger 48.300 kr. (2009 og 2010)), anvendes 
	// den ikke udnyttede del til nedsættelse af de øvrige skatter (dvs. kommunale indkomstskatter og kirkeskat).
	//
	// § 10 Stk. 3.: I det omfang en gift person, der er samlevende med ægtefællen ved indkomstårets udløb, 
	// ikke kan udnytte skatteværdien af personfradragene, benyttes den ikke udnyttede del af skatteværdien 
	// til nedsættelse efter § 9 af den anden ægtefælles skatter.
	public class PersonfradragBeregner
	{
		private readonly List<SkatteModregner> _skatteModregnere;

		public PersonfradragBeregner()
		{
			_skatteModregnere = new List<SkatteModregner>
			{
				// I det omfang skatteværdien af personfradraget mht. sundhedsbidrag ikke kan fradrages i selve 
				// sundhedsbidraget, fragår den i nævnte rækkefølge i følgende skatter: bundskat, mellemskat, 
				// topskat og skat af aktieindkomst, der overstiger 48.300 kr. (2009 og 2010). 
				new SkatteModregner(
					IntrospectionOf<Skatter>.GetAccessorFor(x => x.Sundhedsbidrag),
					IntrospectionOf<Skatter>.GetAccessorFor(x => x.Bundskat),
					IntrospectionOf<Skatter>.GetAccessorFor(x => x.Mellemskat),
					IntrospectionOf<Skatter>.GetAccessorFor(x => x.Topskat),
					IntrospectionOf<Skatter>.GetAccessorFor(x => x.SkatAfAktieindkomst),
					IntrospectionOf<Skatter>.GetAccessorFor(x => x.Kommuneskat),
					IntrospectionOf<Skatter>.GetAccessorFor(x => x.Kirkeskat)
				),
				// Tilsvarende, hvis skatteværdien af personfradraget mht. bundskat ikke kan fradrages i selve bundskatten, 
				// fragår den i nævnte rækkefølge i følgende skatter: sundhedsbidrag, mellemskat og topskat og skat af 
				// aktieindkomst, der overstiger 48.300 kr. (2009 og 2010). 
				new SkatteModregner(
					IntrospectionOf<Skatter>.GetAccessorFor(x => x.Bundskat),
					IntrospectionOf<Skatter>.GetAccessorFor(x => x.Sundhedsbidrag),
					IntrospectionOf<Skatter>.GetAccessorFor(x => x.Mellemskat),
					IntrospectionOf<Skatter>.GetAccessorFor(x => x.Topskat),
					IntrospectionOf<Skatter>.GetAccessorFor(x => x.SkatAfAktieindkomst),
					IntrospectionOf<Skatter>.GetAccessorFor(x => x.Kommuneskat),
					IntrospectionOf<Skatter>.GetAccessorFor(x => x.Kirkeskat)
				),
				// Skatteværdi af kommuneskat
				new SkatteModregner(
					IntrospectionOf<Skatter>.GetAccessorFor(x => x.Kommuneskat),
					IntrospectionOf<Skatter>.GetAccessorFor(x => x.Sundhedsbidrag),
					IntrospectionOf<Skatter>.GetAccessorFor(x => x.Bundskat),
					IntrospectionOf<Skatter>.GetAccessorFor(x => x.Mellemskat),
					IntrospectionOf<Skatter>.GetAccessorFor(x => x.Topskat),
					IntrospectionOf<Skatter>.GetAccessorFor(x => x.SkatAfAktieindkomst),
					IntrospectionOf<Skatter>.GetAccessorFor(x => x.Kirkeskat)
				),
				// Skatteværdi af kirkeskat
				new SkatteModregner(
					IntrospectionOf<Skatter>.GetAccessorFor(x => x.Kirkeskat),
					IntrospectionOf<Skatter>.GetAccessorFor(x => x.Sundhedsbidrag),
					IntrospectionOf<Skatter>.GetAccessorFor(x => x.Bundskat),
					IntrospectionOf<Skatter>.GetAccessorFor(x => x.Mellemskat),
					IntrospectionOf<Skatter>.GetAccessorFor(x => x.Topskat),
					IntrospectionOf<Skatter>.GetAccessorFor(x => x.SkatAfAktieindkomst),
					IntrospectionOf<Skatter>.GetAccessorFor(x => x.Kommuneskat)
				)
			};
		}

		/// <summary>
		/// De beregnede indkomstskatter til stat, kommune samt kirke nedsættes hver for sig 
		/// med en procentdel af personfradraget.
		/// </summary>
		/// <remarks>
		/// For gifte personer kan uudnyttet personfradrag overføres til den anden ægtefælle.
		/// Et slutteligt ikke udnyttet personfradrag kan ikke overføres til det efterfølgende skatteår.
		/// </remarks>
		// TODO: Vi giver ikke oplysninger om resterende skatteværdi af personfradrag, der fortabes, det bør vi i denne low-level beregner
		public ValueTuple<ModregnResult2> BeregnSkatEfterPersonfradrag(ValueTuple<Skatter> skatter, ValueTuple<KommunaleSatser> kommunaleSatser)
		{
			// Modregning af skatteværdier af personfradraget i egne indkomstskatter.
			var modregnPersonfradragResults = BeregnSkatEfterPersonfradragEgneSkatter(skatter, kommunaleSatser);
			var modregnedeSkatter = modregnPersonfradragResults.Map(x => x.ModregnedeSkatter);
			var ikkeUdnyttedeSkattevaerdier = modregnPersonfradragResults.Map(x => x.IkkeUdnyttedeSkattevaerdier);
			
			// Modregning af ikke udnyttede skatteværdier af personfradraget i ægtefælles indkomstskatter.
			if (ikkeUdnyttedeSkattevaerdier.Size > 1)
			{
				int i = ikkeUdnyttedeSkattevaerdier.IndexOf(x => x.Sum() > 0);
				if (i >= 0 && ikkeUdnyttedeSkattevaerdier.PartnerOf(i).Sum() == 0)
				{
					// index i har et resterende ikke udnyttet personfradrag og ægtefælle har fuld udnyttelæse af sit personfradrag
					var kommunaleSatserOfPartner = kommunaleSatser.PartnerOf(i);
					var skatterOfPartner = modregnedeSkatter.PartnerOf(i);
					// Uudnyttede skatteværdier, omregnes til fradragsbeløb.
					var ikkeUdnyttetFradragsbeloeb = BeregnFradragsbeloeb(ikkeUdnyttedeSkattevaerdier[i], 
					                                                      kommunaleSatserOfPartner);
					// Skatteværdierne af fradragsbeløbet bliver beregnet med ægtefællens egne skatteprocenter.
					var overfoerteSkattevaerdierTilPartner = BeregnSkattevaerdier(ikkeUdnyttetFradragsbeloeb, 
					                                                    kommunaleSatserOfPartner);
					// Modregning af skatteværdierne  i ægtefællens egne indkomstskatter.
					var modregningPersonfradragResult = BeregnSkatEfterModregningAfSkattevaerdier(skatterOfPartner,
					                                                                              overfoerteSkattevaerdierTilPartner);
					// Beregn evt. resterende ikke-udnyttet personfradrag, der 'tabes på gulvet'
					var modregnedeSkatterOfPartner = modregningPersonfradragResult.ModregnedeSkatter;
					var ikkeUdnyttedeSkattevaerdierOfPartner = modregningPersonfradragResult.IkkeUdnyttedeSkattevaerdier;
					if (i == 0)
					{
						ikkeUdnyttedeSkattevaerdier = new ValueTuple<Skatter>(ikkeUdnyttedeSkattevaerdier[i],
						                                                      ikkeUdnyttedeSkattevaerdierOfPartner);
						modregnedeSkatter = new ValueTuple<Skatter>(modregnedeSkatter[i], modregnedeSkatterOfPartner);
					}
					else
					{
						ikkeUdnyttedeSkattevaerdier = new ValueTuple<Skatter>(ikkeUdnyttedeSkattevaerdierOfPartner,
						                                                      ikkeUdnyttedeSkattevaerdier[i]);
						modregnedeSkatter = new ValueTuple<Skatter>(modregnedeSkatterOfPartner, modregnedeSkatter[i]);
					}
				}
			}

			var ikkeUdnyttedeFradrag = ikkeUdnyttedeSkattevaerdier.Map((skattevaerdi, index) => 
				BeregnFradragsbeloeb(skattevaerdi, kommunaleSatser[index]));

			return modregnedeSkatter.Map((modregnetSkat, index) => 
				new ModregnResult2(modregnetSkat, ikkeUdnyttedeSkattevaerdier[index].Sum(), ikkeUdnyttedeFradrag[index]));
		}

		/// <summary>
		/// Beregn skatter efter modregning af skatteværdier af personfradraget i egne indkomstskatter.
		/// </summary>
		public ValueTuple<ModregnPersonfradragResult> BeregnSkatEfterPersonfradragEgneSkatter(ValueTuple<Skatter> skatter, ValueTuple<KommunaleSatser> kommunaleSatser)
		{
			var skattevaerdier = BeregnSkattevaerdierAfPersonfradrag(kommunaleSatser);
			return skatter.Map((skat, index) => BeregnSkatEfterModregningAfSkattevaerdier(skat, skattevaerdier[index]));
		}

		/// <summary>
		/// Beregn skatter efter modregning af skatteværdier af personfradraget i egne indkomstskatter.
		/// </summary>
		public ModregnPersonfradragResult BeregnSkatEfterModregningAfSkattevaerdier(Skatter skatter, Skatter skattevaerdier)
		{
			var modregnedeSkatter = skatter.Clone();
			var ikkeUdnyttedeSkattevaerdier = new Skatter();
			
			_skatteModregnere.Each(skatteModregner =>
			{
				var accessor = skatteModregner.First();
				decimal skattevaerdi = accessor.GetValue(skattevaerdier);
				var modregnResult = skatteModregner.Modregn(modregnedeSkatter, skattevaerdi);
				modregnedeSkatter = modregnResult.ModregnedeSkatter;
				accessor.SetValue(ikkeUdnyttedeSkattevaerdier, modregnResult.IkkeUdnyttetSkattevaerdi);
			});

			return new ModregnPersonfradragResult(modregnedeSkatter, ikkeUdnyttedeSkattevaerdier);
		}

		/// <summary>
		/// Beregn skatteværdier af personfradraget for kommuneskat, kirkeskat, bundskat og sundhedsbidrag.
		/// </summary>
		public ValueTuple<Skatter> BeregnSkattevaerdierAfPersonfradrag(ValueTuple<KommunaleSatser> kommunaleSatser)
		{
			return kommunaleSatser.Map(kommunaleSats => BeregnSkattevaerdierAfPersonfradrag(kommunaleSats));
		}

		public Skatter BeregnSkattevaerdierAfPersonfradrag(KommunaleSatser kommunaleSatser)
		{
			// TODO: Personfradrag er individuelt bestemt af alder og civilstand
			// Personfradrag opgøres efter § 10 og skatteværdien heraf efter § 12.

			// § 12 stk. 1: Skatteværdien af de i § 10 nævnte personfradrag opgøres som en procentdel af fradragene. 
			// Ved opgørelsen anvendes samme procent som ved beregningen af indkomstskat til kommunen samt kirkeskat. 
			// Ved beregningen af indkomstskat til staten beregnes skatteværdien af personfradraget med beskatnings-
			// procenterne for bundskat efter § 5, nr. 1, og for sundhedsbidrag efter § 5, nr. 4.

			return BeregnSkattevaerdier(Constants.Personfradrag, kommunaleSatser);
		}

		public Skatter BeregnSkattevaerdier(decimal personfradrag, KommunaleSatser kommunaleSatser)
		{
			decimal skattevaerdiBundskat = Constants.Bundskattesats * personfradrag;
			decimal skattevaerdiSundhedsbidrag = Constants.Sundhedsbidragsats * personfradrag;
			decimal skattevaerdiKommuneskat = kommunaleSatser.Kommuneskattesats * personfradrag;
			decimal skattevaerdiKirkeskat = kommunaleSatser.Kirkeskattesats * personfradrag;

			return new Skatter
			{
				Bundskat = skattevaerdiBundskat,
				Sundhedsbidrag = skattevaerdiSundhedsbidrag,
				Kommuneskat = skattevaerdiKommuneskat,
				Kirkeskat = skattevaerdiKirkeskat
			};
		}

		public static decimal BeregnFradragsbeloeb(Skatter skattevaerdier, KommunaleSatser kommunaleSatser)
		{
			// TODO: Refactor this ugly piece of code
			// Denne metode tror jeg er forkert....
			//decimal fradragsbeloeb = skattevaerdier.Bundskat / Constants.Bundskattesats;
			//fradragsbeloeb += skattevaerdier.Sundhedsbidrag/ Constants.Sundhedsbidragsats;
			//fradragsbeloeb += skattevaerdier.Kommuneskat / kommunaleSatser.Kommuneskattesats;
			//if (kommunaleSatser.Kirkeskattesats > 0)
			//{
			//    fradragsbeloeb += skattevaerdier.Kirkeskat / kommunaleSatser.Kirkeskattesats;
			//}

			decimal sumSatser = Constants.Bundskattesats + Constants.Sundhedsbidragsats + kommunaleSatser.Kommuneskattesats +
			                    kommunaleSatser.Kirkeskattesats;

			decimal fradragsbeloeb = skattevaerdier.Sum() / sumSatser;

			return fradragsbeloeb;
		}
	}
}