using System;
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
	public class SkattepligtigIndkomstUnderskudBeregner
	{
		/// <summary>
		/// Modregner først årets (eventuelle) underskud i egen og derefter ægtefælles skattepligtige indkomst og skatter, dernæst bliver
		/// et (eventuelt) fremført underskud modregnet på samme måde, og endelig bliver et eventuelt restunderskud fremført til næste 
		/// indkomstår.
		/// </summary>
		/// <param name="indkomster">De indkomster, hvor underskuddet bliver modregnet</param>
		/// <param name="skatter">De skatter, hvor underskud bliver modregnet</param>
		/// <param name="kommunaleSatser">Kommunale skattesatser</param>
		/// <returns>Resultatet</returns>
		public ValueTuple<ModregnUnderskudResult> ModregningAfUnderskud(ValueTuple<PersonligeBeloeb> indkomster, 
			ValueTuple<SkatterAfPersonligIndkomst> skatter, ValueTuple<KommunaleSatser> kommunaleSatser)
		{
			var skattepligtigeIndkomster = indkomster.Map(x => x.SkattepligtigIndkomst);
			
			// I. Årets underskud
			var aaretsUnderskud = +(-skattepligtigeIndkomster);

			var modregnAaretsUnderskudResult 
				= modregnEgenOgAegtefaelleUnderskudOgUnderskudsvaerdi(indkomster, skattepligtigeIndkomster,
						skatter, aaretsUnderskud, kommunaleSatser, (value, index) => indkomster[index].ModregnetUnderskudSkattepligtigIndkomst -= value);

			var modregningSkattepligtigIndkomster = modregnAaretsUnderskudResult.Map(x => x.ModregningSkattepligtigIndkomst);
			var modregningSkatter = modregnAaretsUnderskudResult.Map(x => x.ModregningSkatter);

			// II. Fremført underskud
			var fremfoertUnderskud = indkomster.Map(x => x.FremfoertUnderskudSkattepligtigIndkomst);
			var modregnFremfoertUnderskudResult
				= modregnEgenOgAegtefaelleUnderskudOgUnderskudsvaerdi(indkomster, skattepligtigeIndkomster - modregningSkattepligtigIndkomster, 
						skatter - modregningSkatter, fremfoertUnderskud, kommunaleSatser, (value, index) => indkomster[index].FremfoertUnderskudSkattepligtigIndkomst -= value);

			return (modregnAaretsUnderskudResult + modregnFremfoertUnderskudResult)
				.ToModregnResult(skattepligtigeIndkomster, skatter, aaretsUnderskud + fremfoertUnderskud);
		}

		private static SkatteModregner<SkatterAfPersonligIndkomst> getSkattepligtigIndkomstUnderskudModregner()
		{
			return new SkatteModregner<SkatterAfPersonligIndkomst>(
				Modregning<SkatterAfPersonligIndkomst>.Af(x => x.Bundskat),
				Modregning<SkatterAfPersonligIndkomst>.Af(x => x.Mellemskat),
				Modregning<SkatterAfPersonligIndkomst>.Af(x => x.Topskat),
				Modregning<SkatterAfPersonligIndkomst>.Af(x => x.AktieindkomstskatOverGrundbeloebet)
			);
		}

		private static ValueTuple<ModregnIndkomstResult> modregnUnderskud(ValueTuple<decimal> indkomster, ValueTuple<decimal> underskud)
		{
			ValueTuple<decimal> modregninger = indkomster.GetMuligModregning(underskud);
			return modregninger.Map((modregning, index) => new ModregnIndkomstResult(underskud[index], indkomster[index], modregning));
		}

		private static ValueTuple<ModregnSkatterResultEx<SkatterAfPersonligIndkomst>> modregnUnderskudsvaerdi(ValueTuple<SkatterAfPersonligIndkomst> skatter,
			ValueTuple<decimal> underskud, ValueTuple<SkattevaerdiOmregner> skattevaerdiOmregnere)
		{
			var skatteModregner = getSkattepligtigIndkomstUnderskudModregner();
			var underskudsvaerdier = underskud.Map((beloeb, index) => skattevaerdiOmregnere[index].BeregnSkattevaerdi(beloeb));
			var modregnResult = skatteModregner.Modregn(skatter, underskudsvaerdier);
			return modregnResult.ToModregnSkatterResultEx(skattevaerdiOmregnere.Cast<SkattevaerdiOmregner, ISkattevaerdiOmregner>());
		}

		private static ValueTuple<BeregnModregningerResult> modregnEgenOgAegtefaelleUnderskudOgUnderskudsvaerdi(
			ValueTuple<PersonligeBeloeb> indkomster, 
			ValueTuple<decimal> skattepligtigeIndkomster, 
			ValueTuple<SkatterAfPersonligIndkomst> skatter, 
			ValueTuple<decimal> underskud, 
			ValueTuple<KommunaleSatser> kommunaleSatser,
			Action<decimal, int> nulstilHandler)
		{
			// Modregn i egen skattepligtige indkomst og skatter
			var modregnEgetUnderskudResult = modregnUnderskudOgUnderskudsvaerdi(skattepligtigeIndkomster, skatter, underskud, kommunaleSatser);
			var restunderskud = modregnEgetUnderskudResult.Map((x, index) => x.GetRestunderskud(underskud[index]));

			if (skatter.Size == 1)
			{
				// Note: Side-effekter...skal fjernes
				var modregningIndkomster = modregnEgetUnderskudResult.Map(x => x.ModregningSkattepligtigIndkomst);
				modregningIndkomster.Each((modregningIndkomst, index) =>
				{
					nulstilHandler(underskud[index], index);
				    // Modregning hos den der benytter underskuddet
				    indkomster[index].ModregnetUnderskudSkattepligtigIndkomst += modregningIndkomst;
				    // Fremførsel af resterende underskud
				    indkomster[index].UnderskudSkattepligtigIndkomstTilFremfoersel += restunderskud[index];
				});

				return modregnEgetUnderskudResult;
			}

			// Modregn i ægtefælles skattepligtige indkomst og skatter
			var modregningSkatter = modregnEgetUnderskudResult.Map(x => x.ModregningSkatter);
			var overfoertUnderskud = restunderskud.Swap();
			var modregnOverfoertUnderskudResult = modregnUnderskudOgUnderskudsvaerdi(skattepligtigeIndkomster,
			                                             skatter - modregningSkatter, overfoertUnderskud, kommunaleSatser);
			var tilbagefoertUnderskud = modregnOverfoertUnderskudResult.Map((x, index) => x.GetRestunderskud(underskud[index])).Swap();
			
			// Note: Side-effekter...skal fjernes
			var modregningIndkomsterEgetUnderskud = modregnEgetUnderskudResult.Map(x => x.ModregningSkattepligtigIndkomst);
			var modregningIndkomsterOverfoertUnderskud = modregnOverfoertUnderskudResult.Map(x => x.ModregningSkattepligtigIndkomst);
			(modregningIndkomsterEgetUnderskud + modregningIndkomsterOverfoertUnderskud).Each((modregningIndkomst, index) =>
			{
				nulstilHandler(underskud[index], index);
			    // Modregning hos den der benytter underskuddet
			    indkomster[index].ModregnetUnderskudSkattepligtigIndkomst += modregningIndkomst;
			    // Fremførsel af resterende underskud
			    indkomster[index].UnderskudSkattepligtigIndkomstTilFremfoersel += tilbagefoertUnderskud[index];
			});

			return modregnEgetUnderskudResult + modregnOverfoertUnderskudResult;
		}

		private static ValueTuple<BeregnModregningerResult> modregnUnderskudOgUnderskudsvaerdi(ValueTuple<decimal> skattepligtigeIndkomster, 
			ValueTuple<SkatterAfPersonligIndkomst> skatter, ValueTuple<decimal> underskud, ValueTuple<KommunaleSatser> kommunaleSatser) 
		{
			var skattevaerdiOmregnere = getUnderskudSkattevaerdiBeregnere(kommunaleSatser);

			// Modregn underskud i positiv skattepligtige indkomst
			var modregnIndkomstResults = modregnUnderskud(skattepligtigeIndkomster, underskud);
			var modregningSkattepligtigIndkomst = modregnIndkomstResults.Map(x => x.UdnyttetUnderskud);
			var restunderskud = modregnIndkomstResults.Map(x => x.IkkeUdnyttetUnderskud);

			// Modregn underskudsværdi i skatter
			var modregningSkatterResult = modregnUnderskudsvaerdi(skatter, restunderskud, skattevaerdiOmregnere);
			var modregningSkatter = modregningSkatterResult.Map(x => x.UdnyttedeSkattevaerdier);
			restunderskud = modregningSkatterResult.Map(x => x.IkkeUdnyttetFradrag);
			
			return skattepligtigeIndkomster.Map(index => 
				new BeregnModregningerResult(modregningSkattepligtigIndkomst[index], modregningSkatter[index], 
					underskud[index] - restunderskud[index]));
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