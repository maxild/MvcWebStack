using System;

namespace Maxfire.Skat
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////
	// § 13. stk. 1: Ugifte
	//
	// Pkt. 1: Hvis den skattepligtige indkomst udviser underskud, beregnes skatteværdien af underskuddet 
	// med beskatningsprocenten for sundhedsbidrag, jf. § 8, og beskatningsprocenterne for kommunal 
	// indkomstskat og kirkeskat henholdsvis med beskatningsprocenten efter § 8 c. 
	// Pkt. 2: Skatteværdien af underskuddet modregnes i den nævnte rækkefølge i skatterne efter §§ 6, 
	// 6 a og 7 og § 8 a, stk. 2. 
	// Pkt. 3: Et herefter resterende underskud fremføres til fradrag i den skattepligtige indkomst 
	// for de følgende indkomstår. 
	// Pkt. 4: Fradraget for underskud i skattepligtig indkomst kan kun fremføres til et senere indkomstår, 
	// hvis det ikke kan rummes i skattepligtig indkomst eller modregnes med skatteværdien i skat efter 
	// §§ 6, 6 a og 7 og § 8 a, stk. 2, for et tidligere indkomstår.
	//
	// § 13. stk. 2: Gifte
	//
	// Pkt. 1: Hvis en gift persons skattepligtige indkomst udviser underskud, og ægtefællerne er samlevende 
	// ved indkomstårets udløb, skal underskud, der ikke er modregnet efter stk. 1, 2. pkt., i størst 
	// muligt omfang fradrages i den anden ægtefælles skattepligtige indkomst. 
	// Pkt. 2: Derefter modregnes skatteværdien af uudnyttet underskud i ægtefællens beregnede skatter 
	// efter §§ 6, 6 a, 7 og 8 a, stk. 2. 
	// Pkt. 3: Modregning sker, før ægtefællens egne uudnyttede underskud fra tidligere indkomstår fremføres 
	// efter 4.-6. pkt. 
	// Pkt. 4: Et herefter overskydende beløb fremføres til fradrag i følgende indkomstår efter 
	// stk. 1, 4. pkt.
	// Pkt. 5: Hvert år fradrages underskud først i den skattepligtiges indkomst og i øvrigt efter samme
	// regler, som gælder for underskudsåret.
	// Pkt. 6: Hvis ægtefællen ligeledes har uudnyttet underskud vedrørende tidligere indkomstår, skal 
	// ægtefællens egne underskud modregnes først.
	//
	//////////////////////////////////////////////////////////////////////////////////////////////////////
	// TODO: Ingen fremførte underskud mellem indkomst år, hverken værdi fra forrige år, eller overførsel til kommende skatteår.
	public class SkattepligtigIndkomstUnderskudBeregner
	{
		public static SkatteModregner<SkatterAfPersonligIndkomst> GetSkattepligtigIndkomstUnderskudModregner()
		{
			return new SkatteModregner<SkatterAfPersonligIndkomst>(
				Modregning<SkatterAfPersonligIndkomst>.Af(x => x.Bundskat),
				Modregning<SkatterAfPersonligIndkomst>.Af(x => x.Mellemskat),
				Modregning<SkatterAfPersonligIndkomst>.Af(x => x.Topskat),
				Modregning<SkatterAfPersonligIndkomst>.Af(x => x.AktieindkomstskatOverGrundbeloebet)
			);
		}

		public ValueTuple<ModregnResultEx<SkatterAfPersonligIndkomst>> ModregningAfUnderskud(ValueTuple<PersonligeBeloeb> indkomster, 
			ValueTuple<SkatterAfPersonligIndkomst> skatter, ValueTuple<KommunaleSatser> kommunaleSatser)
		{
			var reberegnedeSkatter = skatter;
			var skattevaerdiOmregner = GetUnderskudSkattevaerdiBeregnere(kommunaleSatser);
			var skatteModregner = GetSkattepligtigIndkomstUnderskudModregner();

			var skattepligtigeIndkomster = indkomster.Map(x => x.SkattepligtigIndkomst);
			
			var underskud = +(-skattepligtigeIndkomster);
			var underskudsvaerdier = underskud.Map((fradragsbeloeb, index) => 
				skattevaerdiOmregner[index].BeregnSkattevaerdi(fradragsbeloeb));

			// TODO: BeregnModregningerAfUnderskudsvaerdi
			// Modregning i egne skatter (PSL § 13 stk.1, pkt. 1 og 2)
			var modregningerEgetUnderskud = skatter.Map((skat, index) =>
				skatteModregner.BeregnModregninger(skat, underskudsvaerdier[index]));

			var udnyttetUnderskud = modregningerEgetUnderskud.Map((modregning, index) =>
					skattevaerdiOmregner[index].BeregnFradragsbeloeb(modregning.Sum()));
			var ikkeUdnyttetUnderskud = underskud - udnyttetUnderskud;
				
			if (indkomster.Size > 1)
			{
				var overfoertAfUnderskud = BeregnOverfoertAfUnderskud(skattepligtigeIndkomster, ikkeUdnyttetUnderskud);
				
				// TODO: Hvorfor overhovedet reberegne nu når vi bereregner skatter i to trin (dekomponeret)
				//reberegnedeSkatter = ReberegnSkatterAfOverfoertUnderskud(indkomster, skatter, overfoertAfUnderskud, kommunaleSatser);

				// TODO: BeregnModregningerAfOverfoertUnderskudsvaerdi
				// Note: Overførsel af underskud
				// Underskud, der ikke er modregnet, skal så vidt muligt modregnes 
				// i ægtefællens skattepligtige indkomst (PSL §13, stk 2 pkt. 1)
				
				// Note: Overførsel af underskudsværdi
				// Hvis der er overskydende skatteværdi tilbage, så omregn underskud til skatteværdi og
				// Modregning i ægtefælles skattepligtige indkomst
				//modregnResults = ModregnResterendeUnderskudPartnersEgneSkatter(modregnResults, skattevaerdiOmregner);
			}

			return reberegnedeSkatter.Map((skat, index) => 
				new ModregnResult<SkatterAfPersonligIndkomst>(skat, underskudsvaerdier[index], modregningerEgetUnderskud[index]).ToModregnResultEx(skattevaerdiOmregner[index]));
		}

		/// <summary>
		/// Denne tuple kan adderes til skattepligtig indkomst for at fremkalde skattepligtig indkomst efter overførsel af underskud
		/// </summary>
		public ValueTuple<decimal> BeregnOverfoertAfUnderskud(ValueTuple<decimal> skattepligtigeIndkomster, ValueTuple<decimal> ikkeUdnyttetUnderskud)
		{
			var ikkeUdnyttetUnderskudHosPartner = ikkeUdnyttetUnderskud.Swap();

			// Begge ægtefæller kan ikke både have underskud, og plads til at rumme dette underskud, og derfor 
			// er denne tuple enten (x, 0), (0, x) eller (0, 0)
			var muligOverfoerselAfUnderskud = skattepligtigeIndkomster.Map((skattepligtigIndkomst, index) =>
				Math.Min(skattepligtigIndkomst, ikkeUdnyttetUnderskudHosPartner[index]).NonNegative());

			var underskudOverfoersel = muligOverfoerselAfUnderskud - muligOverfoerselAfUnderskud.Swap();

			return underskudOverfoersel;
		}

		// TODO: Denne sammenblander de to modregninger: 1) egne skatter, 2) overførsel af underskud til ægtefælles skattepligtige indkomst
		// TODO: Dette er en anden modregning end i egne skatter, hvordan gør vi det?
		public ValueTuple<Skatter> ReberegnSkatterAfOverfoertUnderskud(ValueTuple<PersonligeBeloeb> indkomster,
			ValueTuple<Skatter> skatter, ValueTuple<decimal> overfoerselAfUnderskud, ValueTuple<KommunaleSatser> kommunaleSatser)
		{
			if (overfoerselAfUnderskud.AllZero())
			{
				return skatter;
			}

			// Begge ægtefæller kan ikke både have underskud, og plads til at rumme dette underskud, derfor 
			// kan vi entydigt overføre underskud fra i til partnerOf(i)
			//decimal overfoerselAfUnderskud = muligOverfoerselAfUnderskud[i];
			int i = overfoerselAfUnderskud.IndexOf(x => x > 0);
			indkomster.PartnerOf(i).ModregnetUnderskudSkattepligtigIndkomst = overfoerselAfUnderskud.PartnerOf(i);
			indkomster[i].ModregnetUnderskudSkattepligtigIndkomst = overfoerselAfUnderskud[i];
			
			// Reberegn skatter (istedet for at beregne skatteværdien)
			var skatBeregner = new SkatBeregner();
			var reberegnedeSkatter = skatBeregner.BeregnSkat(indkomster, kommunaleSatser);

			return reberegnedeSkatter;
			
			// NOTE: Modregningen kan udregnes som (modregning = skatter - reberegnedeSkatter)
			//var skatter = modregnEgneSkatterResults.Map(x => x.Skatter);
			//var modregningAfOverfoersel = skatter - reberegnedeSkatter;

			//// Note: Vi beregner ikke modregninger mht. reduktionen af ægtefællens skattepligtige indkomst,
			//// og derfor bliver skatteværdier og modregninger ikke korrigeret
			//var skattevaerdier = modregnEgneSkatterResults.Map(x => x.Skattevaerdi);
			//var modregninger = modregnEgneSkatterResults.Map(x => x.UdnyttedeSkattevaerdier);
			
			//// Underskuddet korrigeres for overførslen, hvilket gælder både det samlede underskud og det udnyttede underskud
			//// NOTE: Pga den måde som overførslen udregnes vil et overført underskud altid blive udnyttet fuldt ud, sådan at
			//// partnerOf(i) har et ikke-udnyttet underskud på nul (i kan stadig have et uudnyttet underskud, hvis det ikke kan
			//// rummes i partnerOf(i)'s skattepligtige indkomst)
			//var underskudEgneSkatter = modregnEgneSkatterResults.Map(x => x.Fradrag);
			//var underskudOverfoersel = muligOverfoerselAfUnderskud - muligOverfoerselAfUnderskud.Swap();
			//var underskudEfterOverfoersel = underskudEgneSkatter + underskudOverfoersel;
			//var udnyttetUnderskudEgneSkatter = modregnEgneSkatterResults.Map(x => x.UdnyttetFradrag);
			//var udnyttetUnderskudEfterOverfoersel = udnyttetUnderskudEgneSkatter + muligOverfoerselAfUnderskud;

			//// TODO: modregninger er kun mht. egne skatter, og overførsel af underskud til ægtefælle end del af reberegnede skatter
			//// TODO: Er sidstnævnte en del af indkomstopgørelsen, eller en del af modregningen af underskud????
			//return reberegnedeSkatter.Map((reberegnetSkat, index) => new ModregnResultEx(reberegnetSkat, skattevaerdier[index], 
			//    modregninger[index], underskudEfterOverfoersel[index], udnyttetUnderskudEfterOverfoersel[index]));
		}

		public ValueTuple<ModregnResultEx<SkatterAfPersonligIndkomst>> ModregnResterendeUnderskudPartnersEgneSkatter(
			ValueTuple<ModregnResultEx<SkatterAfPersonligIndkomst>> modregnResults, ValueTuple<SkattevaerdiOmregner> underskudSkattevaerdiBeregnere)
		{
			var skatter = modregnResults.Map(x => x.Skatter);
			var modregningerEgetUnderskudEgneSkatter = modregnResults.Map(x => x.UdnyttedeSkattevaerdier);
			var overfoertIkkeUdnyttetUnderskud = modregnResults.Map(x => x.IkkeUdnyttetFradrag).Swap();
			var skattevaerdierAfOverfoertUnderskud = overfoertIkkeUdnyttetUnderskud.Map((overfoertUnderskud, index) => 
				underskudSkattevaerdiBeregnere[index].BeregnSkattevaerdi(overfoertUnderskud));

			var skatteModregner = GetSkattepligtigIndkomstUnderskudModregner();
			var modregningerPartnersUnderskudEgneSkatter = skattevaerdierAfOverfoertUnderskud.Map((skattevaerdi, index) => 
				skatteModregner.BeregnModregninger(skatter[index], skattevaerdi));

			var skattevaerdier = modregnResults.Map(x => x.Skattevaerdi);
			var underskud = modregnResults.Map(x => x.Fradrag);
			var udnyttetUnderskud = modregnResults.Map(x => x.UdnyttetFradrag);

			var udnyttetOverfoertUnderskudsvaerdi = modregningerPartnersUnderskudEgneSkatter.Map(x => x.Sum());
			var udnyttetOverfoertUnderskud = udnyttetOverfoertUnderskudsvaerdi.Map((skattevaerdi, index) => 
				underskudSkattevaerdiBeregnere[index].BeregnFradragsbeloeb(skattevaerdi));
			
			// TODO: udnyttet underskud mangler at blive opdateret
			return skatter.Map((skat, index) =>
				new ModregnResultEx<SkatterAfPersonligIndkomst>(skat, skattevaerdier[index],
								  modregningerEgetUnderskudEgneSkatter[index] + modregningerPartnersUnderskudEgneSkatter[index],
								  underskud[index] + overfoertIkkeUdnyttetUnderskud[index], 
								  udnyttetUnderskud[index] + udnyttetOverfoertUnderskud[index]));
		}

		public ValueTuple<SkattevaerdiOmregner> GetUnderskudSkattevaerdiBeregnere(ValueTuple<KommunaleSatser> kommunaleSatser)
		{
			return kommunaleSatser.Map(satser => GetUnderskudSkattevaerdiBeregner(satser));
		}

		public SkattevaerdiOmregner GetUnderskudSkattevaerdiBeregner(KommunaleSatser kommunaleSatser)
		{
			return new SkattevaerdiOmregner(kommunaleSatser.KommuneOgKirkeskattesats + Constants.Sundhedsbidragsats);
		}
	}
}