using Maxfire.Core.Extensions;

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

		public static ValueTuple<ModregnIndkomstResult> ModregnUnderskud(ValueTuple<decimal> indkomster, ValueTuple<decimal> underskud)
		{
			ValueTuple<decimal> modregninger = indkomster.GetMuligModregning(underskud);
			return modregninger.Map((modregning, index) => new ModregnIndkomstResult(underskud[index], indkomster[index], modregning));
		}

		public static ValueTuple<ModregnSkatterResultEx<SkatterAfPersonligIndkomst>> ModregnUnderskudsvaerdi(ValueTuple<SkatterAfPersonligIndkomst> skatter, 
			ValueTuple<decimal> underskud, ValueTuple<SkattevaerdiOmregner> skattevaerdiOmregnere)
		{
			var skatteModregner = GetSkattepligtigIndkomstUnderskudModregner();
			var underskudsvaerdier = underskud.Map((beloeb, index) => skattevaerdiOmregnere[index].BeregnSkattevaerdi(beloeb));
			var modregnResult = skatteModregner.Modregn(skatter, underskudsvaerdier);
			return modregnResult.ToModregnSkatterResultEx(skattevaerdiOmregnere.Cast<SkattevaerdiOmregner, ISkattevaerdiOmregner>());
		}

		// Beregner modregninger i SkatterAfPersonligIndkomst (bundskat, mellemskat, topskat og aktieindkomstskat over grundbeløbet)
		// og modregninger af den skattepligtige indkomst, samt evt. restunderskud til fremførsel.
		// TODO: Resultat type skal indeholde alle disse værdier, og altså ingen skatter
		// NOTE: Den nuværende implementering af modregninger i skattepligtig indkomst sker ved at ændre 'ValueTuple<PersonligeBeloeb> indkomster' (side-effekt)
		// TODO: Fjern den uheldige side-effekt, hvor vi skriver til PersonligeBeloeb.ModregnetUnderskudSkattepligtigIndkomst!!!!!
		// TODO: : II. Fremført underskud
		public ValueTuple<ModregnUnderskudResult> ModregningAfUnderskud(ValueTuple<PersonligeBeloeb> indkomster, 
			ValueTuple<SkatterAfPersonligIndkomst> skatter, ValueTuple<KommunaleSatser> kommunaleSatser)
		{
			var skattepligtigeIndkomster = indkomster.Map(x => x.SkattepligtigIndkomst);
			
			// I. Årets underskud
			var aaretsUnderskud = +(-skattepligtigeIndkomster);

			// Egen indkomst og skatter
			var modregnEgetUnderskudResult = ModregnUnderskudOgUnderskudsvaerdi(skattepligtigeIndkomster, skatter, aaretsUnderskud, kommunaleSatser);
			var restunderskud = modregnEgetUnderskudResult.Map((x, index) => x.GetRestunderskud(aaretsUnderskud[index]));

			if (skatter.Size == 1)
			{
				// Note: Side-effekter...skal fjernes
				var modregningIndkomster = modregnEgetUnderskudResult.Map(x => x.ModregningSkattepligtigIndkomst);
				modregningIndkomster.Each((modregningIndkomst, index) =>
				{
					// Nulstilling hos den der leverer underskuddet enten til modregning eller fremførsel
					indkomster[index].ModregnetUnderskudSkattepligtigIndkomst -= aaretsUnderskud[index]; // TODO: Virker kun mht årets underskud
					// Modregning hos den der benytter underskuddet
					indkomster[index].ModregnetUnderskudSkattepligtigIndkomst += modregningIndkomst;
					// Fremførsel af resterende underskud
					indkomster[index].UnderskudSkattepligtigIndkomstFremfoersel += restunderskud[index];
				});

				return modregnEgetUnderskudResult.ToModregnResult(skattepligtigeIndkomster, skatter, aaretsUnderskud);
			}

			// Ægtefælles indkomst og skatter (via overførsel/sambeskatning)
			var modregningSkatter = modregnEgetUnderskudResult.Map(x => x.ModregningSkatter);
			var overfoertUnderskud = restunderskud.Swap();
			var modregnOverfoertUnderskudResult = ModregnUnderskudOgUnderskudsvaerdi(skattepligtigeIndkomster,
			                                   skatter - modregningSkatter, overfoertUnderskud, kommunaleSatser);
			var tilbagefoertUnderskud = modregnOverfoertUnderskudResult.Map((x, index) => x.GetRestunderskud(aaretsUnderskud[index])).Swap();
			
			// Note: Side-effekter...skal fjernes
			var modregningIndkomsterEgetUnderskud = modregnEgetUnderskudResult.Map(x => x.ModregningSkattepligtigIndkomst);
			var modregningIndkomsterOverfoertUnderskud = modregnOverfoertUnderskudResult.Map(x => x.ModregningSkattepligtigIndkomst);
			(modregningIndkomsterEgetUnderskud + modregningIndkomsterOverfoertUnderskud).Each((modregningIndkomst, index) =>
			{
				// Nulstilling hos den der leverer underskuddet enten til modregning eller fremførsel
				indkomster[index].ModregnetUnderskudSkattepligtigIndkomst -= aaretsUnderskud[index]; // TODO: Virker kun mht årets underskud
				// Modregning hos den der benytter underskuddet
				indkomster[index].ModregnetUnderskudSkattepligtigIndkomst += modregningIndkomst;
				// Fremførsel af resterende underskud
				indkomster[index].UnderskudSkattepligtigIndkomstFremfoersel += tilbagefoertUnderskud[index];
			});

			return (modregnEgetUnderskudResult + modregnOverfoertUnderskudResult)
				.ToModregnResult(skattepligtigeIndkomster, skatter, aaretsUnderskud);
		}

		public ValueTuple<BeregnModregningerResult> ModregnUnderskudOgUnderskudsvaerdi(ValueTuple<decimal> skattepligtigeIndkomster, 
			ValueTuple<SkatterAfPersonligIndkomst> skatter, ValueTuple<decimal> underskud, ValueTuple<KommunaleSatser> kommunaleSatser) 
		{
			var skattevaerdiOmregnere = getUnderskudSkattevaerdiBeregnere(kommunaleSatser);

			// Modregn underskud i positiv skattepligtige indkomst
			var modregnIndkomstResults = ModregnUnderskud(skattepligtigeIndkomster, underskud);
			var modregningSkattepligtigIndkomst = modregnIndkomstResults.Map(x => x.UdnyttetUnderskud);
			var restunderskud = modregnIndkomstResults.Map(x => x.IkkeUdnyttetUnderskud);

			// Modregn underskudsværdi i skatter
			var modregningSkatterResult = ModregnUnderskudsvaerdi(skatter, restunderskud, skattevaerdiOmregnere);
			var modregningSkatter = modregningSkatterResult.Map(x => x.UdnyttedeSkattevaerdier);
			restunderskud = modregningSkatterResult.Map(x => x.IkkeUdnyttetFradrag);
			
			return skattepligtigeIndkomster.Map(index => 
				new BeregnModregningerResult(modregningSkattepligtigIndkomst[index], modregningSkatter[index], 
					underskud[index] - restunderskud[index]));
		}

		// TODO: Denne sammenblander de to modregninger: 1) egne skatter, 2) overførsel af underskud til ægtefælles skattepligtige indkomst
		// TODO: Dette er en anden modregning end i egne skatter, hvordan gør vi det?
		//public ValueTuple<Skatter> ReberegnSkatterAfOverfoertUnderskud(ValueTuple<PersonligeBeloeb> indkomster,
		//    ValueTuple<Skatter> skatter, ValueTuple<decimal> overfoerselAfUnderskud, ValueTuple<KommunaleSatser> kommunaleSatser)
		//{
		//    if (overfoerselAfUnderskud.AllZero())
		//    {
		//        return skatter;
		//    }

		//    // Begge ægtefæller kan ikke både have underskud, og plads til at rumme dette underskud, derfor 
		//    // kan vi entydigt overføre underskud fra i til partnerOf(i)
		//    //decimal overfoerselAfUnderskud = muligOverfoerselAfUnderskud[i];
		//    int i = overfoerselAfUnderskud.IndexOf(x => x > 0);
		//    indkomster.PartnerOf(i).ModregnetUnderskudSkattepligtigIndkomst = overfoerselAfUnderskud.PartnerOf(i);
		//    indkomster[i].ModregnetUnderskudSkattepligtigIndkomst = overfoerselAfUnderskud[i];
			
		//    // Reberegn skatter (istedet for at beregne skatteværdien)
		//    var skatBeregner = new SkatBeregner();
		//    var reberegnedeSkatter = skatBeregner.BeregnSkat(indkomster, kommunaleSatser);

		//    return reberegnedeSkatter;
			
		//    // NOTE: Modregningen kan udregnes som (modregning = skatter - reberegnedeSkatter)
		//    //var skatter = modregnEgneSkatterResults.Map(x => x.Skatter);
		//    //var modregningAfOverfoersel = skatter - reberegnedeSkatter;

		//    //// Note: Vi beregner ikke modregninger mht. reduktionen af ægtefællens skattepligtige indkomst,
		//    //// og derfor bliver skatteværdier og modregninger ikke korrigeret
		//    //var skattevaerdier = modregnEgneSkatterResults.Map(x => x.Skattevaerdi);
		//    //var modregninger = modregnEgneSkatterResults.Map(x => x.UdnyttedeSkattevaerdier);
			
		//    //// Underskuddet korrigeres for overførslen, hvilket gælder både det samlede underskud og det udnyttede underskud
		//    //// NOTE: Pga den måde som overførslen udregnes vil et overført underskud altid blive udnyttet fuldt ud, sådan at
		//    //// partnerOf(i) har et ikke-udnyttet underskud på nul (i kan stadig have et uudnyttet underskud, hvis det ikke kan
		//    //// rummes i partnerOf(i)'s skattepligtige indkomst)
		//    //var underskudEgneSkatter = modregnEgneSkatterResults.Map(x => x.Fradrag);
		//    //var underskudOverfoersel = muligOverfoerselAfUnderskud - muligOverfoerselAfUnderskud.Swap();
		//    //var underskudEfterOverfoersel = underskudEgneSkatter + underskudOverfoersel;
		//    //var udnyttetUnderskudEgneSkatter = modregnEgneSkatterResults.Map(x => x.UdnyttetFradrag);
		//    //var udnyttetUnderskudEfterOverfoersel = udnyttetUnderskudEgneSkatter + muligOverfoerselAfUnderskud;

		//    //// TODO: modregninger er kun mht. egne skatter, og overførsel af underskud til ægtefælle end del af reberegnede skatter
		//    //// TODO: Er sidstnævnte en del af indkomstopgørelsen, eller en del af modregningen af underskud????
		//    //return reberegnedeSkatter.Map((reberegnetSkat, index) => new ModregnResultEx(reberegnetSkat, skattevaerdier[index], 
		//    //    modregninger[index], underskudEfterOverfoersel[index], udnyttetUnderskudEfterOverfoersel[index]));
		//}

		public ValueTuple<ModregnSkatterResultEx<SkatterAfPersonligIndkomst>> ModregnResterendeUnderskudPartnersEgneSkatter(
			ValueTuple<ModregnSkatterResultEx<SkatterAfPersonligIndkomst>> modregnResults, ValueTuple<SkattevaerdiOmregner> underskudSkattevaerdiBeregnere)
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
				new ModregnSkatterResultEx<SkatterAfPersonligIndkomst>(skat, skattevaerdier[index],
								  modregningerEgetUnderskudEgneSkatter[index] + modregningerPartnersUnderskudEgneSkatter[index],
								  underskud[index] + overfoertIkkeUdnyttetUnderskud[index], 
								  udnyttetUnderskud[index] + udnyttetOverfoertUnderskud[index]));
		}

		private static ValueTuple<SkattevaerdiOmregner> getUnderskudSkattevaerdiBeregnere(ValueTuple<KommunaleSatser> kommunaleSatser)
		{
			return kommunaleSatser.Map(getUnderskudSkattevaerdiBeregner);
		}

		private static SkattevaerdiOmregner getUnderskudSkattevaerdiBeregner(KommunaleSatser kommunaleSatser)
		{
			return new SkattevaerdiOmregner(kommunaleSatser.KommuneOgKirkeskattesats + Constants.Sundhedsbidragsats);
		}
	}
}