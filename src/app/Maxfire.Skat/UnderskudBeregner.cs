using System;

namespace Maxfire.Skat
{
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
	// TODO: Ingen fremf�rte underskud mellem indkomst �r, hverken v�rdi fra forrige �r, eller overf�rsel til kommende skatte�r.
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
			// TODO: Sondring mellem skattev�rdi af fremf�rt underskud og skattev�rdi af �rets underskud
			
			// Modregning i egne skatter (PSL � 13 stk.1, pkt. 1 og 2)
			var modregnResults = BeregnSkatEfterModregningEgneSkatter(skatter, underskudSkattevaerdiBeregnere, skattepligtigIndkomster);
			
			// Underskud, der ikke er modregnet, skal s� vidt muligt modregnes 
			// i �gtef�llens skattepligtige indkomst (PSL �13, stk 2 pkt. 1)
			if (indkomster.Size > 1)
			{
				modregnResults = NedbringPartnersSkattepligtigeIndkomst(indkomster, modregnResults, kommunaleSatser);
			}

			// Hvis der er overskydende skattev�rdi tilbage, s� omregn underskud til skattev�rdi og
			// Modregning i �gtef�lles skattepligtige indkomst

			// Modregn �gtef�lles egne skatter

			return modregnResults;
		}

		// TODO: Denne sammenblander de to modregninger: 1) egne skatter, 2) overf�rsel af underskud til �gtef�lles skattepligtige indkomst
		// TODO: Dette er en anden modregning end i egne skatter, hvordan g�r vi det?
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

			// Begge �gtef�ller kan ikke b�de have underskud, og plads til at rumme dette underskud, derfor 
			// kan vi entydigt overf�re underskud fra i til partnerOf(i)
			decimal overfoerselAfUnderskud = muligOverfoerselAfUnderskud[i];
			indkomster.PartnerOf(i).ModregnetUnderskudSkattepligtigIndkomst = -overfoerselAfUnderskud;
			indkomster[i].ModregnetUnderskudSkattepligtigIndkomst = overfoerselAfUnderskud;
			
			// Reberegn skatter (istedet for at beregne skattev�rdien)
			var skatBeregner = new SkatBeregner();
			var reberegnedeSkatter = skatBeregner.BeregnSkat(indkomster, kommunaleSatser);
			// NOTE: Modregningen kan udregnes som (modregning = skatter - reberegnedeSkatter)
			//var skatter = modregnEgneSkatterResults.Map(x => x.Skatter);
			//var modregningAfOverfoersel = skatter - reberegnedeSkatter;

			// Note: Vi beregner ikke modregninger mht. reduktionen af �gtef�llens skattepligtige indkomst,
			// og derfor bliver skattev�rdier og modregninger ikke korrigeret
			var skattevaerdier = modregnEgneSkatterResults.Map(x => x.Skattevaerdi);
			var modregninger = modregnEgneSkatterResults.Map(x => x.UdnyttedeSkattevaerdier);
			
			// Underskuddet korrigeres for overf�rslen, hvilket g�lder b�de det samlede underskud og det udnyttede underskud
			// NOTE: Pga den m�de som overf�rslen udregnes vil et overf�rt underskud altid blive udnyttet fuldt ud, s�dan at
			// partnerOf(i) har et ikke-udnyttet underskud p� nul (i kan stadig have et uudnyttet underskud, hvis det ikke kan
			// rummes i partnerOf(i)'s skattepligtige indkomst)
			var underskudEgneSkatter = modregnEgneSkatterResults.Map(x => x.Fradrag);
			var underskudOverfoersel = muligOverfoerselAfUnderskud - muligOverfoerselAfUnderskud.Swap();
			var underskudEfterOverfoersel = underskudEgneSkatter + underskudOverfoersel;
			var udnyttetUnderskudEgneSkatter = modregnEgneSkatterResults.Map(x => x.UdnyttetFradrag);
			var udnyttetUnderskudEfterOverfoersel = udnyttetUnderskudEgneSkatter + muligOverfoerselAfUnderskud;

			// TODO: modregninger er kun mht egne skatter, og overf�rsel af underskud til �gtef�lle end del af reberegnede skatter
			// TODO: Er sidstn�vnte en del af indkomstopg�relsen, eller en del af modregningen af underskud????
			return reberegnedeSkatter.Map((reberegnetSkat, index) => new ModregnResultEx(reberegnetSkat, skattevaerdier[index], 
				modregninger[index], underskudEfterOverfoersel[index], udnyttetUnderskudEfterOverfoersel[index]));
		}

		// TODO: Overvej at benytte ValueTuple<Skatter> BeregnModregningEgneSkater, der kun beregninger modregninger (udnyttede skattev�rdier)
		public ValueTuple<ModregnResultEx> BeregnSkatEfterModregningEgneSkatter(ValueTuple<Skatter> skatter, 
			ValueTuple<SkattevaerdiOmregner> underskudSkattevaerdiBeregnere, ValueTuple<decimal> skattepligtigeIndkomster)
		{
			return skatter.Map(
				(skat, index) => BeregnSkatEfterModregningEgneSkatter(skat, underskudSkattevaerdiBeregnere[index], skattepligtigeIndkomster[index]));
		}

		/// <summary>
		/// Beregn skatter efter modregning af skattev�rdi i den del af egne indkomstskatter, der ikke beregnes af skattepligtig indkomst.
		/// </summary>
		public ModregnResultEx BeregnSkatEfterModregningEgneSkatter(Skatter skatter, 
			SkattevaerdiOmregner skattevaerdiOmregner, decimal skattepligtigIndkomst)
		{
			if (skattepligtigIndkomst >= 0)
			{
				// Ingen skattev�rdi, modregning ej mulig.
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