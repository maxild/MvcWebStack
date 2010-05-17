using System;

namespace Maxfire.Skat
{
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
	// TODO: Ingen fremførte underskud mellem indkomst år, hverken værdi fra forrige år, eller overførsel til kommende skatteår.
	// TODO: Skal hedde UnderskudSkattepligtigIndkomstBeregner

	
	
	public class UnderskudBeregner
	{
		public static SkatteModregner GetSkattepligtigIndkomstUnderskudModregner()
		{
			return new SkatteModregner(
				Modregning.Af(x => x.Bundskat),
				Modregning.Af(x => x.Mellemskat),
				Modregning.Af(x => x.Topskat),
				Modregning.Af(x => x.AktieindkomstskatOverGrundbeloebet)
			);
		}

		public ValueTuple<ModregnResultEx> Beregn(ValueTuple<PersonligeBeloeb> indkomster, 
			ValueTuple<Skatter> skatter, ValueTuple<KommunaleSatser> kommunaleSatser)
		{
			var skattepligtigIndkomster = indkomster.Map(x => x.SkattepligtigIndkomst);
			var underskudSkattevaerdiBeregnere = GetUnderskudSkattevaerdiBeregnere(kommunaleSatser);
			// TODO: Sondring mellem skatteværdi af fremført underskud og skatteværdi af årets underskud
			
			// Modregning i egne skatter (PSL § 13 stk.1, pkt. 1 og 2)
			var modregnResults = BeregnSkatEfterModregningEgneSkatter(skatter, underskudSkattevaerdiBeregnere, skattepligtigIndkomster);
			
			// Underskud, der ikke er modregnet, skal så vidt muligt modregnes 
			// i ægtefællens skattepligtige indkomst (PSL §13, stk 2 pkt. 1)
			if (indkomster.Size > 1)
			{
				modregnResults = NedbringPartnersSkattepligtigeIndkomst(indkomster, modregnResults, kommunaleSatser);
			}

			// Hvis der er overskydende skatteværdi tilbage, så omregn underskud til skatteværdi og
			// Modregning i ægtefælles skattepligtige indkomst

			// Modregn ægtefælles egne skatter

			return modregnResults;
		}

		// TODO: Denne sammenblander de to modregninger: 1) egne skatter, 2) overførsel af underskud til ægtefælles skattepligtige indkomst
		// TODO: Dette er en anden modregning end i egne skatter, hvordan gør vi det?
		public ValueTuple<ModregnResultEx> NedbringPartnersSkattepligtigeIndkomst(ValueTuple<PersonligeBeloeb> indkomster, 
			ValueTuple<ModregnResultEx> modregnEgneSkatterResults, ValueTuple<KommunaleSatser> kommunaleSatser)
		{
			var ikkeUdnyttetUnderskud = modregnEgneSkatterResults.Map(x => x.IkkeUdnyttetFradrag);
			var ikkeUdnyttetUnderskudHosPartner = ikkeUdnyttetUnderskud.Swap();

			var muligOverfoerselAfUnderskud = indkomster.Map((indkomst, index) => 
				Math.Min(indkomst.SkattepligtigIndkomst, ikkeUdnyttetUnderskudHosPartner[index]).NonNegative());

			int i = muligOverfoerselAfUnderskud.IndexOf(x => x > 0);
			if (i == -1)
			{
				return modregnEgneSkatterResults;
			}

			// Begge ægtefæller kan ikke både have underskud, og plads til at rumme dette underskud, derfor 
			// kan vi entydigt overføre underskud fra i til partnerOf(i)
			decimal overfoerselAfUnderskud = muligOverfoerselAfUnderskud[i];
			indkomster.PartnerOf(i).ModregnetUnderskudSkattepligtigIndkomst = -overfoerselAfUnderskud;
			indkomster[i].ModregnetUnderskudSkattepligtigIndkomst = overfoerselAfUnderskud;
			
			// Reberegn skatter (istedet for at beregne skatteværdien)
			var skatBeregner = new SkatBeregner();
			var reberegnedeSkatter = skatBeregner.BeregnSkat(indkomster, kommunaleSatser);
			// NOTE: Modregningen kan udregnes som (modregning = skatter - reberegnedeSkatter)
			//var skatter = modregnEgneSkatterResults.Map(x => x.Skatter);
			//var modregningAfOverfoersel = skatter - reberegnedeSkatter;

			// Note: Vi beregner ikke modregninger mht. reduktionen af ægtefællens skattepligtige indkomst,
			// og derfor bliver skatteværdier og modregninger ikke korrigeret
			var skattevaerdier = modregnEgneSkatterResults.Map(x => x.Skattevaerdi);
			var modregninger = modregnEgneSkatterResults.Map(x => x.UdnyttedeSkattevaerdier);
			
			// Underskuddet korrigeres for overførslen, hvilket gælder både det samlede underskud og det udnyttede underskud
			// NOTE: Pga den måde som overførslen udregnes vil et overført underskud altid blive udnyttet fuldt ud, sådan at
			// partnerOf(i) har et ikke-udnyttet underskud på nul (i kan stadig have et uudnyttet underskud, hvis det ikke kan
			// rummes i partnerOf(i)'s skattepligtige indkomst)
			var underskudEgneSkatter = modregnEgneSkatterResults.Map(x => x.Fradrag);
			var underskudOverfoersel = muligOverfoerselAfUnderskud - muligOverfoerselAfUnderskud.Swap();
			var underskudEfterOverfoersel = underskudEgneSkatter + underskudOverfoersel;
			var udnyttetUnderskudEgneSkatter = modregnEgneSkatterResults.Map(x => x.UdnyttetFradrag);
			var udnyttetUnderskudEfterOverfoersel = udnyttetUnderskudEgneSkatter + muligOverfoerselAfUnderskud;

			// TODO: modregninger er kun mht egne skatter, og overførsel af underskud til ægtefælle end del af reberegnede skatter
			// TODO: Er sidstnævnte en del af indkomstopgørelsen, eller en del af modregningen af underskud????
			return reberegnedeSkatter.Map((reberegnetSkat, index) => new ModregnResultEx(reberegnetSkat, skattevaerdier[index], 
				modregninger[index], underskudEfterOverfoersel[index], udnyttetUnderskudEfterOverfoersel[index]));
		}

		// TODO: Overvej at benytte ValueTuple<Skatter> BeregnModregningEgneSkater, der kun beregninger modregninger (udnyttede skatteværdier)
		public ValueTuple<ModregnResultEx> BeregnSkatEfterModregningEgneSkatter(ValueTuple<Skatter> skatter, 
			ValueTuple<SkattevaerdiOmregner> underskudSkattevaerdiBeregnere, ValueTuple<decimal> skattepligtigeIndkomster)
		{
			return skatter.Map(
				(skat, index) => BeregnSkatEfterModregningEgneSkatter(skat, underskudSkattevaerdiBeregnere[index], skattepligtigeIndkomster[index]));
		}

		/// <summary>
		/// Beregn skatter efter modregning af skatteværdi i den del af egne indkomstskatter, der ikke beregnes af skattepligtig indkomst.
		/// </summary>
		public ModregnResultEx BeregnSkatEfterModregningEgneSkatter(Skatter skatter, 
			SkattevaerdiOmregner skattevaerdiOmregner, decimal skattepligtigIndkomst)
		{
			if (skattepligtigIndkomst >= 0)
			{
				// Ingen skatteværdi, modregning ej mulig.
				return ModregnResultEx.Nul(skatter);
			}

			var underskud = -skattepligtigIndkomst;
			var skattevaerdiAfUnderskud = skattevaerdiOmregner.BeregnSkattevaerdi(underskud);

			var skatteModregner = GetSkattepligtigIndkomstUnderskudModregner();
			var modregnResult = skatteModregner.Modregn(skatter, skattevaerdiAfUnderskud);

			return modregnResult.ToModregnResultEx(skattevaerdiOmregner);
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