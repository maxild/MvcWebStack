using System;
using System.Linq;
using Maxfire.Core.Reflection;

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
				IntrospectionOf<Skatter>.GetAccessorFor(x => x.Bundskat),
				IntrospectionOf<Skatter>.GetAccessorFor(x => x.Mellemskat),
				IntrospectionOf<Skatter>.GetAccessorFor(x => x.Topskat),
				IntrospectionOf<Skatter>.GetAccessorFor(x => x.AktieindkomstskatOverGrundbeloebet));
		}

		public ValueTuple<ModregnResult2> Beregn(ValueTuple<PersonligeBeloeb> indkomster, 
			ValueTuple<Skatter> skatter, ValueTuple<KommunaleSatser> kommunaleSatser)
		{
			var skattepligtigIndkomster = indkomster.Map(x => x.SkattepligtigIndkomst);
			var underskudSkattevaerdiBeregnere = GetUnderskudSkattevaerdiBeregnere(kommunaleSatser);
			// TODO: Sondring mellem skattev�rdi af fremf�rt underskud og skattev�rdi af �rets underskud
			
			// Modregning i egne skatter (PSL � 13 stk.1, pkt. 1 og 2)
			var modregnResults = BeregnSkatEfterModregningEgneSkatter(skatter, underskudSkattevaerdiBeregnere, skattepligtigIndkomster);
			var ikkeUdnyttetUnderskud = modregnResults.Map(x => x.IkkeUdnyttetFradrag);

			// Underskud, der ikke er modregnet, skal s� vidt muligt modregnes 
			// i �gtef�llens skattepligtige indkomst (PSL �13, stk 2 pkt. 1)
			if (indkomster.Size > 0)
			{
				modregnResults = NedbringPartnersSkattepligtigeIndkomst(indkomster, skatter, ikkeUdnyttetUnderskud, kommunaleSatser);
			}

			// Hvis der er overskydende skattev�rdi tilbage, s� omregn underskud til skattev�rdi og
			// Modregning i �gtef�lles skattepligtige indkomst

			// Modregn �gtef�lles egne skatter)

			return modregnResults;
		}

		public ValueTuple<ModregnResult2> NedbringPartnersSkattepligtigeIndkomst(ValueTuple<PersonligeBeloeb> indkomster, ValueTuple<Skatter> skatter,
			ValueTuple<decimal> ikkeUdnyttedeUnderskud, ValueTuple<KommunaleSatser> kommunaleSatser)
		{
			var modregnedeSkatter = skatter;
			var ikkeUdnyttedeUnderskudHosPartner = ikkeUdnyttedeUnderskud.Swap();

			// Underskud der kan rummes i �gtef�llens skattepligtige indkomst
			var underskudOverfoertTil = indkomster.Map((indkomst, index) => 
				Math.Min(indkomst.SkattepligtigIndkomst, ikkeUdnyttedeUnderskudHosPartner[index]).NonNegative());

			// Begge �gtef�ller kan ikke b�de have underskud, og plads til at rumme dette underskud
			int i = underskudOverfoertTil.IndexOf(underskud => underskud > 0);
			if (i >= 0)
			{
				// Overf�r underskud fra partnerOf(i)...
				indkomster.PartnerOf(i).ModregnetUnderskudSkattepligtigIndkomst = -underskudOverfoertTil[i];
				// ...til i, s�dan at den skattepligtige indkomst bliver reduceret for i
				indkomster[i].ModregnetUnderskudSkattepligtigIndkomst = underskudOverfoertTil[i];
				// Og opg�r evt. ikke udnyttet underskud, der skal f�res videre til reduktion af �gtef�llens egne skatter
				var underskudOverfoertFra = underskudOverfoertTil.Swap();
				ikkeUdnyttedeUnderskud -= underskudOverfoertFra;
				// Reberegn skatter (=> fejl pga side-effekt i form af overskrivelse af modregninger i bundskatten)
				var skatBeregner = new SkatBeregner();
				modregnedeSkatter = skatBeregner.BeregnSkat(indkomster, kommunaleSatser);
			}

			// TODO: Skattev�rdi mangler
			return modregnedeSkatter.Map((modregnetSkat, index) => new ModregnResult2(modregnetSkat, 0, ikkeUdnyttedeUnderskud[index]));
		}
		
		public ValueTuple<ModregnResult2> BeregnSkatEfterModregningEgneSkatter(ValueTuple<Skatter> skatter, 
			ValueTuple<UnderskudSkattevaerdiBeregner> underskudSkattevaerdiBeregnere, ValueTuple<decimal> skattepligtigeIndkomster)
		{
			return skatter.Map(
				(skat, index) => BeregnSkatEfterModregningEgneSkatter(skat, underskudSkattevaerdiBeregnere[index], skattepligtigeIndkomster[index]));
		}

		/// <summary>
		/// Beregn skatter efter modregning af skattev�rdi i den del af egne indkomstskatter, der ikke beregnes af skattepligtig indkomst.
		/// </summary>
		public ModregnResult2 BeregnSkatEfterModregningEgneSkatter(Skatter skatter, 
			UnderskudSkattevaerdiBeregner underskudSkattevaerdiBeregner, decimal skattepligtigIndkomst)
		{
			var skattevaerdiAfUnderskud = underskudSkattevaerdiBeregner.BeregnSkattevaerdiAfUnderskud(skattepligtigIndkomst);
			var skatteModregner = GetSkattepligtigIndkomstUnderskudModregner();
			var modregnResult = skatteModregner.Modregn(skatter, skattevaerdiAfUnderskud);
			decimal ikkeUdnyttetSkattevaerdi = modregnResult.IkkeUdnyttetSkattevaerdi;
			decimal ikkeUdnyttetUnderskud = underskudSkattevaerdiBeregner.BeregnUnderskudAfSkattevaerdi(ikkeUdnyttetSkattevaerdi);
			return new ModregnResult2(modregnResult.ModregnedeSkatter, ikkeUdnyttetSkattevaerdi, ikkeUdnyttetUnderskud);
		}

		public ValueTuple<UnderskudSkattevaerdiBeregner> GetUnderskudSkattevaerdiBeregnere(ValueTuple<KommunaleSatser> kommunaleSatser)
		{
			return kommunaleSatser.Map(satser => GetUnderskudSkattevaerdiBeregner(satser));
		}

		public UnderskudSkattevaerdiBeregner GetUnderskudSkattevaerdiBeregner(KommunaleSatser kommunaleSatser)
		{
			return new UnderskudSkattevaerdiBeregner(kommunaleSatser);
		}
	}
}