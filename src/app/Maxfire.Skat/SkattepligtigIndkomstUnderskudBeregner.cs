using Maxfire.Core.Extensions;

namespace Maxfire.Skat
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////
	// � 13. stk. 1: Ugifte
	//
	// Pkt. 1: Hvis den skattepligtige indkomst udviser underskud, beregnes skattev�rdien af underskuddet 
	// med beskatningsprocenten for sundhedsbidrag, jf. � 8, og beskatningsprocenterne for kommunal 
	// indkomstskat og kirkeskat henholdsvis med beskatningsprocenten efter � 8 c. 
	// Pkt. 2: Skattev�rdien af underskuddet modregnes i den n�vnte r�kkef�lge i skatterne efter �� 6, 
	// 6 a og 7 og � 8 a, stk. 2. 
	// Pkt. 3: Et herefter resterende underskud fremf�res til fradrag i den skattepligtige indkomst 
	// for de f�lgende indkomst�r. 
	// Pkt. 4: Fradraget for underskud i skattepligtig indkomst kan kun fremf�res til et senere indkomst�r, 
	// hvis det ikke kan rummes i skattepligtig indkomst eller modregnes med skattev�rdien i skat efter 
	// �� 6, 6 a og 7 og � 8 a, stk. 2, for et tidligere indkomst�r.
	//
	// � 13. stk. 2: Gifte
	//
	// Pkt. 1: Hvis en gift persons skattepligtige indkomst udviser underskud, og �gtef�llerne er samlevende 
	// ved indkomst�rets udl�b, skal underskud, der ikke er modregnet efter stk. 1, 2. pkt., i st�rst 
	// muligt omfang fradrages i den anden �gtef�lles skattepligtige indkomst. 
	// Pkt. 2: Derefter modregnes skattev�rdien af uudnyttet underskud i �gtef�llens beregnede skatter 
	// efter �� 6, 6 a, 7 og 8 a, stk. 2. 
	// Pkt. 3: Modregning sker, f�r �gtef�llens egne uudnyttede underskud fra tidligere indkomst�r fremf�res 
	// efter 4.-6. pkt. 
	// Pkt. 4: Et herefter overskydende bel�b fremf�res til fradrag i f�lgende indkomst�r efter 
	// stk. 1, 4. pkt.
	// Pkt. 5: Hvert �r fradrages underskud f�rst i den skattepligtiges indkomst og i �vrigt efter samme
	// regler, som g�lder for underskuds�ret.
	// Pkt. 6: Hvis �gtef�llen ligeledes har uudnyttet underskud vedr�rende tidligere indkomst�r, skal 
	// �gtef�llens egne underskud modregnes f�rst.
	//
	//////////////////////////////////////////////////////////////////////////////////////////////////////
	// TODO: Ingen fremf�rte underskud mellem indkomst �r, hverken v�rdi fra forrige �r, eller overf�rsel til kommende skatte�r.
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

		// Beregner modregninger i SkatterAfPersonligIndkomst (bundskat, mellemskat, topskat og aktieindkomstskat over grundbel�bet)
		// og modregninger af den skattepligtige indkomst, samt evt. restunderskud til fremf�rsel.
		// TODO: Resultat type skal indeholde alle disse v�rdier, og alts� ingen skatter
		// NOTE: Den nuv�rende implementering af modregninger i skattepligtig indkomst sker ved at �ndre 'ValueTuple<PersonligeBeloeb> indkomster' (side-effekt)
		// TODO: Fjern den uheldige side-effekt, hvor vi skriver til PersonligeBeloeb.ModregnetUnderskudSkattepligtigIndkomst!!!!!
		public ValueTuple<ModregnSkatterResultEx<SkatterAfPersonligIndkomst>> ModregningAfUnderskud(ValueTuple<PersonligeBeloeb> indkomster, 
			ValueTuple<SkatterAfPersonligIndkomst> skatter, ValueTuple<KommunaleSatser> kommunaleSatser)
		{
			var skattevaerdiOmregnere = getUnderskudSkattevaerdiBeregnere(kommunaleSatser);
			var skatteModregner = GetSkattepligtigIndkomstUnderskudModregner();

			var skattepligtigeIndkomster = indkomster.Map(x => x.SkattepligtigIndkomst);
			
			// I. �rets underskud
			var underskud = +(-skattepligtigeIndkomster);
			var underskudsvaerdier = underskud.Map((fradragsbeloeb, index) =>
				skattevaerdiOmregnere[index].BeregnSkattevaerdi(fradragsbeloeb));
			
			// Trin 1: Modregn underskud i egen positive skattepligtige indkomst
			var modregnIndkomstResults = ModregnUnderskud(skattepligtigeIndkomster, underskud);
			var modregningSkattepligtigIndkomstEgetUnderskud = modregnIndkomstResults.Map(x => x.UdnyttetUnderskud);
			var restunderskud = modregnIndkomstResults.Map(x => x.IkkeUdnyttetUnderskud);
			
			// Trin 2: Modregn underskudsv�rdi i egne skatter
			var modregningSkatterResult = ModregnUnderskudsvaerdi(skatter, restunderskud, skattevaerdiOmregnere);
			var modregningerEgetUnderskud = modregningSkatterResult.Map(x => x.UdnyttedeSkattevaerdier);
			restunderskud = modregningSkatterResult.Map(x => x.IkkeUdnyttetFradrag);
			
			skattepligtigeIndkomster = modregnIndkomstResults.Map(x => x.ModregnetIndkomst);

			ValueTuple<decimal> modregningPersonligIndkomstOverfoertUnderskud 
				= new ValueTuple<decimal>(skatter.Size, () => 0);
			ValueTuple<SkatterAfPersonligIndkomst> modregningerOverfoertUnderskud 
				= new ValueTuple<SkatterAfPersonligIndkomst>(skatter.Size, () => SkatterAfPersonligIndkomst.Nul);

			if (skatter.Size > 1)
			{
				// Trin 3: Overf�r restunderskud til �gtef�lle (hvis det kan rummes i enten �gtef�lles P.I. eller �gtef�lles SkatterAfPersonligIndkomst)
				ValueTuple<decimal> overfoertUnderskud = restunderskud.Swap();

				// Trin 4: Modregn overf�rt underskud i �gtef�lles positive skattepligtige indkomst
				modregningPersonligIndkomstOverfoertUnderskud = skattepligtigeIndkomster.GetMuligModregning(overfoertUnderskud);
				var restOverfoertUnderskud = overfoertUnderskud - modregningPersonligIndkomstOverfoertUnderskud;

				// Trin 5: Modregn overf�rt underskudsv�rdi (pba �gtef�lles skattesatser) i �gtef�lles skatter
				var restOverfoertUnderskudsvaerdier = restOverfoertUnderskud.Map((fradragsbeloeb, index) =>
				                                       skattevaerdiOmregnere[index].BeregnSkattevaerdi(fradragsbeloeb));
				modregningerOverfoertUnderskud = skatteModregner.BeregnModregninger(skatter - modregningerEgetUnderskud, restOverfoertUnderskudsvaerdier);
				restOverfoertUnderskudsvaerdier -= modregningerOverfoertUnderskud.Map(m => m.Sum());
				
				// Trin 6: Tilbagef�r restunderskud til kilden, og s�rg for at rapportere det til fremf�rsel
				ValueTuple<decimal> tilbagefoertUnderskud = restOverfoertUnderskudsvaerdier.Map((underskudsvaerdi, index) =>
				                                       skattevaerdiOmregnere[index].BeregnFradragsbeloeb(underskudsvaerdi));
			}
			
			// II. Fremf�rt underskud

			// Trin 1 til trin 6 som ovenfor

			// Note: Side-effekt...skal fjernes
			(modregningSkattepligtigIndkomstEgetUnderskud + modregningPersonligIndkomstOverfoertUnderskud)
				.Each((modregningSkattepligtigIndkomst, index) =>
				      indkomster[index].ModregnetUnderskudSkattepligtigIndkomst = modregningSkattepligtigIndkomst);

			// TODO: Da der er tale om en sammenblanding af skattev�rdier/underskudsv�rdier og modregninger i skattepligtig indkomst, giver dette resultat ikke mening
			// Dette resultat viser kun v�rdien af modregningerne i bundskat, mellemskat mv. og ikke hele modregningen
			return skatter.Map((skat, index) => 
				new ModregnSkatterResult<SkatterAfPersonligIndkomst>(skat, underskudsvaerdier[index], 
					modregningerEgetUnderskud[index] + modregningerOverfoertUnderskud[index]).ToModregnSkatterResultEx(skattevaerdiOmregnere[index]));
		}

		// TODO: Denne sammenblander de to modregninger: 1) egne skatter, 2) overf�rsel af underskud til �gtef�lles skattepligtige indkomst
		// TODO: Dette er en anden modregning end i egne skatter, hvordan g�r vi det?
		//public ValueTuple<Skatter> ReberegnSkatterAfOverfoertUnderskud(ValueTuple<PersonligeBeloeb> indkomster,
		//    ValueTuple<Skatter> skatter, ValueTuple<decimal> overfoerselAfUnderskud, ValueTuple<KommunaleSatser> kommunaleSatser)
		//{
		//    if (overfoerselAfUnderskud.AllZero())
		//    {
		//        return skatter;
		//    }

		//    // Begge �gtef�ller kan ikke b�de have underskud, og plads til at rumme dette underskud, derfor 
		//    // kan vi entydigt overf�re underskud fra i til partnerOf(i)
		//    //decimal overfoerselAfUnderskud = muligOverfoerselAfUnderskud[i];
		//    int i = overfoerselAfUnderskud.IndexOf(x => x > 0);
		//    indkomster.PartnerOf(i).ModregnetUnderskudSkattepligtigIndkomst = overfoerselAfUnderskud.PartnerOf(i);
		//    indkomster[i].ModregnetUnderskudSkattepligtigIndkomst = overfoerselAfUnderskud[i];
			
		//    // Reberegn skatter (istedet for at beregne skattev�rdien)
		//    var skatBeregner = new SkatBeregner();
		//    var reberegnedeSkatter = skatBeregner.BeregnSkat(indkomster, kommunaleSatser);

		//    return reberegnedeSkatter;
			
		//    // NOTE: Modregningen kan udregnes som (modregning = skatter - reberegnedeSkatter)
		//    //var skatter = modregnEgneSkatterResults.Map(x => x.Skatter);
		//    //var modregningAfOverfoersel = skatter - reberegnedeSkatter;

		//    //// Note: Vi beregner ikke modregninger mht. reduktionen af �gtef�llens skattepligtige indkomst,
		//    //// og derfor bliver skattev�rdier og modregninger ikke korrigeret
		//    //var skattevaerdier = modregnEgneSkatterResults.Map(x => x.Skattevaerdi);
		//    //var modregninger = modregnEgneSkatterResults.Map(x => x.UdnyttedeSkattevaerdier);
			
		//    //// Underskuddet korrigeres for overf�rslen, hvilket g�lder b�de det samlede underskud og det udnyttede underskud
		//    //// NOTE: Pga den m�de som overf�rslen udregnes vil et overf�rt underskud altid blive udnyttet fuldt ud, s�dan at
		//    //// partnerOf(i) har et ikke-udnyttet underskud p� nul (i kan stadig have et uudnyttet underskud, hvis det ikke kan
		//    //// rummes i partnerOf(i)'s skattepligtige indkomst)
		//    //var underskudEgneSkatter = modregnEgneSkatterResults.Map(x => x.Fradrag);
		//    //var underskudOverfoersel = muligOverfoerselAfUnderskud - muligOverfoerselAfUnderskud.Swap();
		//    //var underskudEfterOverfoersel = underskudEgneSkatter + underskudOverfoersel;
		//    //var udnyttetUnderskudEgneSkatter = modregnEgneSkatterResults.Map(x => x.UdnyttetFradrag);
		//    //var udnyttetUnderskudEfterOverfoersel = udnyttetUnderskudEgneSkatter + muligOverfoerselAfUnderskud;

		//    //// TODO: modregninger er kun mht. egne skatter, og overf�rsel af underskud til �gtef�lle end del af reberegnede skatter
		//    //// TODO: Er sidstn�vnte en del af indkomstopg�relsen, eller en del af modregningen af underskud????
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