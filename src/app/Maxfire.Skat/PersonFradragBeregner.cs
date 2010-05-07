using System.Collections.Generic;
using System.Linq;
using Maxfire.Core.Extensions;
using Maxfire.Core.Reflection;

namespace Maxfire.Skat
{
	public class ModregnPersonfradragResult
	{
		public ModregnPersonfradragResult(Skatter modregnedeSkatter, Skatter ikkeUdnyttedeSkattevaerdier)
		{
			ModregnedeSkatter = modregnedeSkatter;
			IkkeUdnyttedeSkattevaerdier = ikkeUdnyttedeSkattevaerdier;
		}

		public Skatter ModregnedeSkatter { get; private set; }
		public Skatter IkkeUdnyttedeSkattevaerdier { get; private set; }
	}

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
		public ValueTuple<Skatter> BeregnSkatEfterPersonfradrag(ValueTuple<Skatter> skatter, ValueTuple<KommunaleSatser> kommunaleSatser)
		{
			// Modregning af skatteværdier af personfradraget i egne indkomstskatter.
			var modregnPersonfradragResults = BeregnSkatEfterPersonfradragEgneSkatter(skatter, kommunaleSatser);
			var ikkeUdnyttedeSkattevaerdier = modregnPersonfradragResults.Map(x => x.IkkeUdnyttedeSkattevaerdier);
			var modregnedeSkatter = modregnPersonfradragResults.Map(x => x.ModregnedeSkatter);

			// Modregning af ikke udnyttede skatteværdier af personfradraget i ægtefælles indkomstskatter.
			for (int i = 0; i < skatter.Size; i++)
			{
				if (ikkeUdnyttedeSkattevaerdier[i].Sum() > 0)
				{
					// Der er opsamlet ikke udnyttedde skatteværdier af personfradraget hos person no. i

					// Uudnyttede skatteværdier, omregnes til fradragsbeløb og overføres til ægtefællen.
					// Her beregnes skatteværdierne med ægtefællens egne (betydning ved forskellige satser, f.eks. kirkesats)
					// skatteprocenter, og der foretages en lignende modregning som ovenfor anført.
				}
			}

			return modregnedeSkatter;
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
			// TODO: Personfradrag er individuelt bestemt af alder og civilstand
			// Personfradrag opgøres efter § 10 og skatteværdien heraf efter § 12.
			
			// § 12 stk. 1: Skatteværdien af de i § 10 nævnte personfradrag opgøres som en procentdel af fradragene. 
			// Ved opgørelsen anvendes samme procent som ved beregningen af indkomstskat til kommunen samt kirkeskat. 
			// Ved beregningen af indkomstskat til staten beregnes skatteværdien af personfradraget med beskatnings-
			// procenterne for bundskat efter § 5, nr. 1, og for sundhedsbidrag efter § 5, nr. 4.

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