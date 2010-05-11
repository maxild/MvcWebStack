using System;
using System.Linq;
using Maxfire.Core.Reflection;

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
			// TODO: Sondring mellem skatteværdi af fremført underskud og skatteværdi af årets underskud
			
			// Modregning i egne skatter (PSL § 13 stk.1, pkt. 1 og 2)
			var modregnResults = BeregnSkatEfterModregningEgneSkatter(skatter, underskudSkattevaerdiBeregnere, skattepligtigIndkomster);
			var ikkeUdnyttetUnderskud = modregnResults.Map(x => x.IkkeUdnyttetFradrag);

			// Underskud, der ikke er modregnet, skal så vidt muligt modregnes 
			// i ægtefællens skattepligtige indkomst (PSL §13, stk 2 pkt. 1)
			if (indkomster.Size > 0)
			{
				modregnResults = NedbringPartnersSkattepligtigeIndkomst(indkomster, skatter, ikkeUdnyttetUnderskud, kommunaleSatser);
			}

			// Hvis der er overskydende skatteværdi tilbage, så omregn underskud til skatteværdi og
			// Modregning i ægtefælles skattepligtige indkomst

			// Modregn ægtefælles egne skatter)

			return modregnResults;
		}

		public ValueTuple<ModregnResult2> NedbringPartnersSkattepligtigeIndkomst(ValueTuple<PersonligeBeloeb> indkomster, ValueTuple<Skatter> skatter,
			ValueTuple<decimal> ikkeUdnyttedeUnderskud, ValueTuple<KommunaleSatser> kommunaleSatser)
		{
			var modregnedeSkatter = skatter;
			var ikkeUdnyttedeUnderskudHosPartner = ikkeUdnyttedeUnderskud.Swap();

			// Underskud der kan rummes i ægtefællens skattepligtige indkomst
			var underskudOverfoertTil = indkomster.Map((indkomst, index) => 
				Math.Min(indkomst.SkattepligtigIndkomst, ikkeUdnyttedeUnderskudHosPartner[index]).NonNegative());

			// Begge ægtefæller kan ikke både have underskud, og plads til at rumme dette underskud
			int i = underskudOverfoertTil.IndexOf(underskud => underskud > 0);
			if (i >= 0)
			{
				// Overfør underskud fra partnerOf(i)...
				indkomster.PartnerOf(i).ModregnetUnderskudSkattepligtigIndkomst = -underskudOverfoertTil[i];
				// ...til i, sådan at den skattepligtige indkomst bliver reduceret for i
				indkomster[i].ModregnetUnderskudSkattepligtigIndkomst = underskudOverfoertTil[i];
				// Og opgør evt. ikke udnyttet underskud, der skal føres videre til reduktion af ægtefællens egne skatter
				var underskudOverfoertFra = underskudOverfoertTil.Swap();
				ikkeUdnyttedeUnderskud -= underskudOverfoertFra;
				// Reberegn skatter (=> fejl pga side-effekt i form af overskrivelse af modregninger i bundskatten)
				var skatBeregner = new SkatBeregner();
				modregnedeSkatter = skatBeregner.BeregnSkat(indkomster, kommunaleSatser);
			}

			// TODO: Skatteværdi mangler
			return modregnedeSkatter.Map((modregnetSkat, index) => new ModregnResult2(modregnetSkat, 0, ikkeUdnyttedeUnderskud[index]));
		}
		
		public ValueTuple<ModregnResult2> BeregnSkatEfterModregningEgneSkatter(ValueTuple<Skatter> skatter, 
			ValueTuple<UnderskudSkattevaerdiBeregner> underskudSkattevaerdiBeregnere, ValueTuple<decimal> skattepligtigeIndkomster)
		{
			return skatter.Map(
				(skat, index) => BeregnSkatEfterModregningEgneSkatter(skat, underskudSkattevaerdiBeregnere[index], skattepligtigeIndkomster[index]));
		}

		/// <summary>
		/// Beregn skatter efter modregning af skatteværdi i den del af egne indkomstskatter, der ikke beregnes af skattepligtig indkomst.
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