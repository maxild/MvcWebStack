using System;

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
			// Modregning i egne skatter (PSL � 13 stk.1, pkt. 1 og 2)
			var modregningerEgetUnderskud = skatter.Map((skat, index) =>
				skatteModregner.BeregnModregninger(skat, underskudsvaerdier[index]));

			var udnyttetUnderskud = modregningerEgetUnderskud.Map((modregning, index) =>
					skattevaerdiOmregner[index].BeregnFradragsbeloeb(modregning.Sum()));
			var ikkeUdnyttetUnderskud = underskud - udnyttetUnderskud;
				
			if (indkomster.Size > 1)
			{
				var overfoertAfUnderskud = BeregnOverfoertAfUnderskud(skattepligtigeIndkomster, ikkeUdnyttetUnderskud);
				
				// TODO: Hvorfor overhovedet reberegne nu n�r vi bereregner skatter i to trin (dekomponeret)
				//reberegnedeSkatter = ReberegnSkatterAfOverfoertUnderskud(indkomster, skatter, overfoertAfUnderskud, kommunaleSatser);

				// TODO: BeregnModregningerAfOverfoertUnderskudsvaerdi
				// Note: Overf�rsel af underskud
				// Underskud, der ikke er modregnet, skal s� vidt muligt modregnes 
				// i �gtef�llens skattepligtige indkomst (PSL �13, stk 2 pkt. 1)
				
				// Note: Overf�rsel af underskudsv�rdi
				// Hvis der er overskydende skattev�rdi tilbage, s� omregn underskud til skattev�rdi og
				// Modregning i �gtef�lles skattepligtige indkomst
				//modregnResults = ModregnResterendeUnderskudPartnersEgneSkatter(modregnResults, skattevaerdiOmregner);
			}

			return reberegnedeSkatter.Map((skat, index) => 
				new ModregnResult<SkatterAfPersonligIndkomst>(skat, underskudsvaerdier[index], modregningerEgetUnderskud[index]).ToModregnResultEx(skattevaerdiOmregner[index]));
		}

		/// <summary>
		/// Denne tuple kan adderes til skattepligtig indkomst for at fremkalde skattepligtig indkomst efter overf�rsel af underskud
		/// </summary>
		public ValueTuple<decimal> BeregnOverfoertAfUnderskud(ValueTuple<decimal> skattepligtigeIndkomster, ValueTuple<decimal> ikkeUdnyttetUnderskud)
		{
			var ikkeUdnyttetUnderskudHosPartner = ikkeUdnyttetUnderskud.Swap();

			// Begge �gtef�ller kan ikke b�de have underskud, og plads til at rumme dette underskud, og derfor 
			// er denne tuple enten (x, 0), (0, x) eller (0, 0)
			var muligOverfoerselAfUnderskud = skattepligtigeIndkomster.Map((skattepligtigIndkomst, index) =>
				Math.Min(skattepligtigIndkomst, ikkeUdnyttetUnderskudHosPartner[index]).NonNegative());

			var underskudOverfoersel = muligOverfoerselAfUnderskud - muligOverfoerselAfUnderskud.Swap();

			return underskudOverfoersel;
		}

		// TODO: Denne sammenblander de to modregninger: 1) egne skatter, 2) overf�rsel af underskud til �gtef�lles skattepligtige indkomst
		// TODO: Dette er en anden modregning end i egne skatter, hvordan g�r vi det?
		public ValueTuple<Skatter> ReberegnSkatterAfOverfoertUnderskud(ValueTuple<PersonligeBeloeb> indkomster,
			ValueTuple<Skatter> skatter, ValueTuple<decimal> overfoerselAfUnderskud, ValueTuple<KommunaleSatser> kommunaleSatser)
		{
			if (overfoerselAfUnderskud.AllZero())
			{
				return skatter;
			}

			// Begge �gtef�ller kan ikke b�de have underskud, og plads til at rumme dette underskud, derfor 
			// kan vi entydigt overf�re underskud fra i til partnerOf(i)
			//decimal overfoerselAfUnderskud = muligOverfoerselAfUnderskud[i];
			int i = overfoerselAfUnderskud.IndexOf(x => x > 0);
			indkomster.PartnerOf(i).ModregnetUnderskudSkattepligtigIndkomst = overfoerselAfUnderskud.PartnerOf(i);
			indkomster[i].ModregnetUnderskudSkattepligtigIndkomst = overfoerselAfUnderskud[i];
			
			// Reberegn skatter (istedet for at beregne skattev�rdien)
			var skatBeregner = new SkatBeregner();
			var reberegnedeSkatter = skatBeregner.BeregnSkat(indkomster, kommunaleSatser);

			return reberegnedeSkatter;
			
			// NOTE: Modregningen kan udregnes som (modregning = skatter - reberegnedeSkatter)
			//var skatter = modregnEgneSkatterResults.Map(x => x.Skatter);
			//var modregningAfOverfoersel = skatter - reberegnedeSkatter;

			//// Note: Vi beregner ikke modregninger mht. reduktionen af �gtef�llens skattepligtige indkomst,
			//// og derfor bliver skattev�rdier og modregninger ikke korrigeret
			//var skattevaerdier = modregnEgneSkatterResults.Map(x => x.Skattevaerdi);
			//var modregninger = modregnEgneSkatterResults.Map(x => x.UdnyttedeSkattevaerdier);
			
			//// Underskuddet korrigeres for overf�rslen, hvilket g�lder b�de det samlede underskud og det udnyttede underskud
			//// NOTE: Pga den m�de som overf�rslen udregnes vil et overf�rt underskud altid blive udnyttet fuldt ud, s�dan at
			//// partnerOf(i) har et ikke-udnyttet underskud p� nul (i kan stadig have et uudnyttet underskud, hvis det ikke kan
			//// rummes i partnerOf(i)'s skattepligtige indkomst)
			//var underskudEgneSkatter = modregnEgneSkatterResults.Map(x => x.Fradrag);
			//var underskudOverfoersel = muligOverfoerselAfUnderskud - muligOverfoerselAfUnderskud.Swap();
			//var underskudEfterOverfoersel = underskudEgneSkatter + underskudOverfoersel;
			//var udnyttetUnderskudEgneSkatter = modregnEgneSkatterResults.Map(x => x.UdnyttetFradrag);
			//var udnyttetUnderskudEfterOverfoersel = udnyttetUnderskudEgneSkatter + muligOverfoerselAfUnderskud;

			//// TODO: modregninger er kun mht. egne skatter, og overf�rsel af underskud til �gtef�lle end del af reberegnede skatter
			//// TODO: Er sidstn�vnte en del af indkomstopg�relsen, eller en del af modregningen af underskud????
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