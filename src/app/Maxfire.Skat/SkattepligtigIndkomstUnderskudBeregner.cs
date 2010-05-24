using System;
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
	public class SkattepligtigIndkomstUnderskudBeregner
	{
		/// <summary>
		/// Modregner f�rst �rets (eventuelle) underskud i egen og derefter �gtef�lles skattepligtige indkomst og skatter, dern�st bliver
		/// et (eventuelt) fremf�rt underskud modregnet p� samme m�de, og endelig bliver et eventuelt restunderskud fremf�rt til n�ste 
		/// indkomst�r.
		/// </summary>
		/// <param name="indkomster">De indkomster, hvor underskuddet bliver modregnet</param>
		/// <param name="skatter">De skatter, hvor underskud bliver modregnet</param>
		/// <param name="kommunaleSatser">Kommunale skattesatser</param>
		/// <returns>Resultatet</returns>
		public ValueTuple<ModregnUnderskudResult> ModregningAfUnderskud(ValueTuple<PersonligeBeloeb> indkomster, 
			ValueTuple<SkatterAfPersonligIndkomst> skatter, ValueTuple<KommunaleSatser> kommunaleSatser)
		{
			var skattepligtigeIndkomster = indkomster.Map(x => x.SkattepligtigIndkomst);
			
			// I. �rets underskud
			var aaretsUnderskud = +(-skattepligtigeIndkomster);

			var modregnAaretsUnderskudResult 
				= modregnEgenOgAegtefaelleUnderskudOgUnderskudsvaerdi(indkomster, skattepligtigeIndkomster,
						skatter, aaretsUnderskud, kommunaleSatser, (value, index) => indkomster[index].ModregnetUnderskudSkattepligtigIndkomst -= value);

			var modregningSkattepligtigIndkomster = modregnAaretsUnderskudResult.Map(x => x.ModregningSkattepligtigIndkomst);
			var modregningSkatter = modregnAaretsUnderskudResult.Map(x => x.ModregningSkatter);

			// II. Fremf�rt underskud
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
				    // Fremf�rsel af resterende underskud
				    indkomster[index].UnderskudSkattepligtigIndkomstTilFremfoersel += restunderskud[index];
				});

				return modregnEgetUnderskudResult;
			}

			// Modregn i �gtef�lles skattepligtige indkomst og skatter
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
			    // Fremf�rsel af resterende underskud
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

			// Modregn underskudsv�rdi i skatter
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