using System.Collections.Generic;
using Maxfire.Core.Extensions;

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
					Modregning.Af(x => x.Sundhedsbidrag),
					Modregning.Af(x => x.Bundskat),
					Modregning.Af(x => x.Mellemskat),
					Modregning.Af(x => x.Topskat),
					Modregning.Af(x => x.AktieindkomstskatOverGrundbeloebet),
					Modregning.Af(x => x.Kommuneskat),
					Modregning.Af(x => x.Kirkeskat)
				),
				// Tilsvarende, hvis skatteværdien af personfradraget mht. bundskat ikke kan fradrages i selve bundskatten, 
				// fragår den i nævnte rækkefølge i følgende skatter: sundhedsbidrag, mellemskat og topskat og skat af 
				// aktieindkomst, der overstiger 48.300 kr. (2009 og 2010). 
				new SkatteModregner(
					Modregning.Af(x => x.Bundskat),
					Modregning.Af(x => x.Sundhedsbidrag),
					Modregning.Af(x => x.Mellemskat),
					Modregning.Af(x => x.Topskat),
					Modregning.Af(x => x.AktieindkomstskatOverGrundbeloebet),
					Modregning.Af(x => x.Kommuneskat),
					Modregning.Af(x => x.Kirkeskat)
				),
				// Skatteværdi af kommuneskat
				new SkatteModregner(
					Modregning.Af(x => x.Kommuneskat),
					Modregning.Af(x => x.Sundhedsbidrag),
					Modregning.Af(x => x.Bundskat),
					Modregning.Af(x => x.Mellemskat),
					Modregning.Af(x => x.Topskat),
					Modregning.Af(x => x.AktieindkomstskatOverGrundbeloebet),
					Modregning.Af(x => x.Kirkeskat)
				),
				// Skatteværdi af kirkeskat
				new SkatteModregner(
					Modregning.Af(x => x.Kirkeskat),
					Modregning.Af(x => x.Sundhedsbidrag),
					Modregning.Af(x => x.Bundskat),
					Modregning.Af(x => x.Mellemskat),
					Modregning.Af(x => x.Topskat),
					Modregning.Af(x => x.AktieindkomstskatOverGrundbeloebet),
					Modregning.Af(x => x.Kommuneskat)
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
		public ValueTuple<ModregnResultEx> ModregningAfPersonfradrag(ValueTuple<Skatter> skatter, ValueTuple<KommunaleSatser> kommunaleSatser)
		{
			// Modregning af skatteværdier af personfradraget i egne indkomstskatter.
			var modregnEgneSkatterResults = ModregningAfPersonfradragEgneSkatter(skatter, kommunaleSatser);

			// Modregning af ....
			return ModregningAfOverfoertPersonfradragTilPartner(modregnEgneSkatterResults, kommunaleSatser);
		}

		/// <summary>
		/// Beregn skatter efter modregning af skatteværdier af personfradraget i egne indkomstskatter.
		/// </summary>
		public ValueTuple<ModregnResultEx> ModregningAfPersonfradragEgneSkatter(ValueTuple<Skatter> skatter, ValueTuple<KommunaleSatser> kommunaleSatser)
		{
			var skattevaerdier = BeregnSkattevaerdierAfPersonfradrag(kommunaleSatser);
			return skatter.Map((skat, index) => ModregningAfSkattevaerdier(skat, skattevaerdier[index], kommunaleSatser[index]));
		}

		/// <summary>
		/// Beregn skatter efter modregning af skatteværdier af personfradraget i egne indkomstskatter.
		/// </summary>
		public ModregnResultEx ModregningAfSkattevaerdier(Skatter skatter, Skatter skattevaerdier, KommunaleSatser kommunaleSatser)
		{
			var modregninger = new Skatter();
			
			// Hver af skatteværdierne modregnes i lovens nævnte rækkefølge
			_skatteModregnere.Each(skatteModregner =>
			{
				var accessor = skatteModregner.FirstAccessor();
				decimal vaerdiAfSkat = accessor.GetValue(skattevaerdier);
				Skatter modregningerAfSkat = skatteModregner.BeregnModregninger(skatter - modregninger, vaerdiAfSkat);
				modregninger += modregningerAfSkat;
			});

			var omregner = PersonfradragSkattevaerdiOmregner.Create(kommunaleSatser);

			var skattevaerdi = skattevaerdier.Sum();
			//var fradrag = omregner.BeregnFradragsbeloeb(skattevaerdi);
			//var udnyttetFradrag = omregner.BeregnFradragsbeloeb(modregninger.Sum());
			
			return new ModregnResult(skatter, skattevaerdi, modregninger).ToModregnResultEx(omregner);
		}

		// Er dette summen eller den ekstra overførsel? Lige nu er det summen
		public ValueTuple<ModregnResultEx> ModregningAfOverfoertPersonfradragTilPartner(ValueTuple<ModregnResultEx> modregnEgneSkatterResults, 
			ValueTuple<KommunaleSatser> kommunaleSatser)
		{
			if (modregnEgneSkatterResults.Size == 1)
			{
				return modregnEgneSkatterResults;
			}

			var ikkeUdnyttetFradragEgneSkatter = modregnEgneSkatterResults.Map(x => x.IkkeUdnyttetFradrag);
			int i = ikkeUdnyttetFradragEgneSkatter.IndexOf(skattevaerdi => skattevaerdi > 0);
			if (i == -1 || ikkeUdnyttetFradragEgneSkatter.PartnerOf(i) > 0)
			{
				return modregnEgneSkatterResults;
			}

			// index i har et resterende ikke udnyttet personfradrag og ægtefælle har fuld udnyttelse af sit personfradrag
			var skatter = modregnEgneSkatterResults.Map(x => x.Skatter);

			var skattevaerdiEgneSkatter = modregnEgneSkatterResults.Map(x => x.Skattevaerdi);
			var udnyttedeSkattevaerdierEgneSkatter = modregnEgneSkatterResults.Map(x => x.UdnyttedeSkattevaerdier); // modregninger
			var fradragEgneSkatter = modregnEgneSkatterResults.Map(x => x.Fradrag);
			var udnyttetFradragEgneSkatter = modregnEgneSkatterResults.Map(x => x.UdnyttetFradrag);

			var kommunaleSatserOfPartner = kommunaleSatser.PartnerOf(i);
			var skatterOfPartner = skatter.PartnerOf(i);
			
			// Skatteværdierne af det overførte fradragsbeløb bliver beregnet med ægtefællens egne skatteprocenter.
			var overfoertFradragTilPartner = ikkeUdnyttetFradragEgneSkatter[i];
			var overfoerteSkattevaerdierTilPartner = PersonfradragSkattevaerdiOmregner.Create(kommunaleSatserOfPartner)
				.BeregnSkattevaerdier(overfoertFradragTilPartner);
			
			// Modregning af skatteværdierne i ægtefællens egne indkomstskatter.
			var modregnPartnersEgneSkatterResult = ModregningAfSkattevaerdier(skatterOfPartner,
																				overfoerteSkattevaerdierTilPartner,
																				kommunaleSatserOfPartner);
			
			// Beregn evt. resterende ikke-udnyttet personfradrag, der 'tabes på gulvet'
			var udnyttedeSkattevaerdierOfPartner = modregnPartnersEgneSkatterResult.UdnyttedeSkattevaerdier;
			var udnyttetFradragOfPartner = modregnPartnersEgneSkatterResult.UdnyttetFradrag;

			// TODO: Refactor this shit
			ModregnResultEx first, second;
			if (i == 0)
			{
				// TODO: Hvad med reduktion af denne
				first = new ModregnResultEx(skatter[0], 
										   skattevaerdiEgneSkatter[0],
				                           udnyttedeSkattevaerdierEgneSkatter[0], 
										   fradragEgneSkatter[0],
				                           udnyttetFradragEgneSkatter[0]);
				second = new ModregnResultEx(skatter[1],
											skattevaerdiEgneSkatter[1],
											udnyttedeSkattevaerdierEgneSkatter[1] + udnyttedeSkattevaerdierOfPartner, 
											fradragEgneSkatter[1] + overfoertFradragTilPartner,
											udnyttetFradragEgneSkatter[1] + udnyttetFradragOfPartner);
			}
			else
			{
				// TODO: Hvad med reduktion af denne
				first = new ModregnResultEx(skatter[0], 
										   skattevaerdiEgneSkatter[0],
										   udnyttedeSkattevaerdierEgneSkatter[0] + udnyttedeSkattevaerdierOfPartner,
										   fradragEgneSkatter[0] + overfoertFradragTilPartner,
										   udnyttetFradragEgneSkatter[0] + udnyttetFradragOfPartner);
				second = new ModregnResultEx(skatter[1],
											skattevaerdiEgneSkatter[1],
											udnyttedeSkattevaerdierEgneSkatter[1], 
											fradragEgneSkatter[1],
											udnyttetFradragEgneSkatter[1]);
			}

			return new ValueTuple<ModregnResultEx>(first, second);
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

			return PersonfradragSkattevaerdiOmregner.Create(kommunaleSatser).BeregnSkattevaerdier(Constants.Personfradrag);
		}
	}
}