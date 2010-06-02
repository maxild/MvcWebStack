using System;
using System.Linq;
using Maxfire.Core.Extensions;

namespace Maxfire.Skat
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////
	//
	// § 13, Stk. 3.
	//
	// Pkt. 1: Hvis den personlige indkomst er negativ, modregnes den inden opgørelsen af beregningsgrundlaget 
	// efter §§ 6, 6 a og 7 i indkomstårets positive kapitalindkomst.
	// Pkt. 2: Et resterende negativt beløb fremføres til modregning først i kapitalindkomst og derefter i 
	// personlig indkomst med tillæg af heri fradragne og ikke medregnede beløb omfattet af beløbsgrænsen 
	// i pensionsbeskatningslovens § 16, stk. 1, for de følgende indkomstår inden opgørelsen af 
	// beregningsgrundlagene efter §§ 6, 6 a og 7. 
	// Pkt. 3: Negativ personlig indkomst kan kun fremføres, i det omfang den ikke kan modregnes efter 
	// 1. eller 2. pkt. for et tidligere indkomstår.
	//
	// § 13, Stk. 4.
	//
	// Pkt. 1: Hvis en gift persons personlige indkomst er negativ og ægtefællerne er samlevende ved 
	// indkomstårets udløb, skal det negative beløb inden opgørelse af beregningsgrundlagene efter 
	// §§ 6, 6 a og 7 modregnes i den anden ægtefælles personlige indkomst med tillæg af heri 
	// fradragne og ikke medregnede beløb omfattet af beløbsgrænsen i pensionsbeskatningslovens § 16, stk. 1. 
	// Pkt. 2: Et overskydende negativt beløb modregnes i ægtefællernes positive kapitalindkomst opgjort 
	// under ét.
	// Pkt. 3: Har begge ægtefæller positiv kapitalindkomst, modregnes det negative beløb fortrinsvis 
	// i den skattepligtiges kapitalindkomst og derefter i ægtefællens kapitalindkomst.
	// Pkt. 4: Modregning sker, før ægtefællens egne uudnyttede underskud fra tidligere indkomstår 
	// fremføres efter 5.-8. pkt.
	// Pkt. 5: Et negativt beløb, der herefter ikke er modregnet, fremføres inden for de følgende 
	// indkomstår, i det omfang beløbet ikke kan modregnes for et tidligere indkomstår, til modregning 
	// inden opgørelsen af beregningsgrundlagene efter §§ 6, 6 a og 7.
	// Pkt. 6: Hvert år modregnes negativ personlig indkomst først i ægtefællernes positive kapitalindkomst 
	// opgjort under ét, derefter i den skattepligtiges personlige indkomst med tillæg af heri fradragne 
	// og ikke medregnede beløb omfattet af beløbsgrænsen i pensionsbeskatningslovens § 16, stk. 1, og 
	// endelig i ægtefællens personlige indkomst med tillæg af heri fradragne og ikke medregnede beløb 
	// omfattet af beløbsgrænsen i pensionsbeskatningslovens § 16, stk. 1. 3. pkt. finder tilsvarende 
	// anvendelse. 
	// Pkt. 7: Hvis ægtefællen ligeledes har uudnyttet underskud vedrørende tidligere indkomstår, skal 
	// ægtefællens egne underskud modregnes først. 
	//
	//////////////////////////////////////////////////////////////////////////////////////////////////////////
	public class PersonligIndkomstUnderskudBeregner
	{
		public void ModregningAfUnderskud(ValueTuple<PersonligeBeloeb> indkomster)
		{
			var personligeIndkomster = indkomster.Map(x => x.PersonligIndkomst);
			var kapitalPensionsindskud = indkomster.Map(x => x.KapitalPensionsindskud);
			var aaretsUnderskud = +(-personligeIndkomster);

			ValueTuple<decimal> restunderskud = aaretsUnderskud;

			if (indkomster.Size == 2)
			{
				// Modregning i ægtefælles (positive) personlige indkomst
				var overfoertUnderskud = aaretsUnderskud.Swap();
				var modregnIndkomstResults = personligeIndkomster.ModregnUnderskud(overfoertUnderskud);
				var overfoertRestunderskud = modregnIndkomstResults.Map(x => x.IkkeUdnyttetUnderskud);

				// TODO: Side-effekt
				modregnIndkomstResults.Each((modregnIndkomstResult, index) => 
					indkomster[index].ModregnetUnderskudPersonligIndkomst += modregnIndkomstResult.UdnyttetUnderskud);
				
				// Modregning i ægtefælles kapitalpensionsindskud
				var modregnKapitalPensionsindskudResults = kapitalPensionsindskud.ModregnUnderskud(overfoertRestunderskud);
				overfoertRestunderskud = modregnKapitalPensionsindskudResults.Map(x => x.IkkeUdnyttetUnderskud);
				restunderskud = overfoertRestunderskud.Swap();

				var udnyttetUnderskud = aaretsUnderskud - restunderskud;
				// TODO: Side-effekt
				// Nulstilling af årets underskud
				udnyttetUnderskud.Each((underskud, index) => indkomster[index].ModregnetUnderskudPersonligIndkomst -= underskud);
			}

			var nettokapitalindkomst = indkomster.Map(x => x.NettoKapitalIndkomst); // TODO: Skattegrundlag

			//
			// modregning i ægtefællers samlede (positive) nettokapitalindkomst
			//

			// Modregn underskud først i egen (positive) nettokapitalindkomst, hvor mulig modregning er
			// bestemt som: Min(egen, egen+ægte, 0)
			var modregningEgenNettoKapitalIndkomst = getMuligModregning(nettokapitalindkomst, restunderskud);
			nettokapitalindkomst -= modregningEgenNettoKapitalIndkomst;
			restunderskud -= modregningEgenNettoKapitalIndkomst;

			// TODO: Side-effekt
			modregningEgenNettoKapitalIndkomst.Each((modregning, index) => 
			{
				// Nulstilling af årets underskud
				indkomster[index].ModregnetUnderskudPersonligIndkomst -= modregning;
				indkomster[index].ModregnetUnderskudNettoKapitalIndkomst += modregning;
			});

			// Modregn dernæst restunderskud i ægtefælles (positive) nettokapitalindkomst, hvor mulig modregning er
			// bestemt som: Min(ægte, egen+ægte, 0).)
			if (indkomster.Size == 2)
			{
				var overfoertUnderskud = restunderskud.Swap();
				var modregningPartnersNettoKapitalIndkomst = getMuligModregning(nettokapitalindkomst, overfoertUnderskud);

				// TODO: Side-effekt
				modregningPartnersNettoKapitalIndkomst.Each((modregning, index) =>
				{
					// Nulstilling af årets underskud
					indkomster[index].ModregnetUnderskudPersonligIndkomst -= modregning;
					indkomster[index].ModregnetUnderskudNettoKapitalIndkomst += modregning;
				});

				nettokapitalindkomst -= modregningPartnersNettoKapitalIndkomst;
				var udnyttetUnderskud = modregningPartnersNettoKapitalIndkomst.Swap();
				// TODO: Side-effekt
				// Nulstilling af årets underskud
				udnyttetUnderskud.Each((underskud, index) => indkomster[index].ModregnetUnderskudPersonligIndkomst -= underskud);
				restunderskud -= udnyttetUnderskud;
			}

			// restunderskud fremføres
			restunderskud.Each((underskud, index) => 
			{	
				// Nulstilling af årets underskud
				indkomster[index].ModregnetUnderskudPersonligIndkomst -= underskud;
				indkomster[index].UnderskudPersonligIndkomstTilFremfoersel += underskud;
			});

			var fremfoertUnderskud = indkomster.Map(x => x.FremfoertUnderskudPersonligIndkomst);

			// modregning i ægtefællers samlede positive nettokapitalindkomst

			// Modregning i egen P.I.
			// Modregning i egen kapitalpensionsindskud

			// Overfør restunderskud

			// Modregning i ægtefælles P.I.
			// Modregning i ægtefælles kapitalpensionsindskud

			// tilbagefør restunderskud til fremførsel
		}

		private ValueTuple<decimal> getMuligModregning(ValueTuple<decimal> nettokapitalindkomst, ValueTuple<decimal> underskud)
		{
			var samletkapitalindkomst = nettokapitalindkomst.Sum();
			// TODO: Make overloads of Min, Max, Path.Combine in Maxfire.Core
			return nettokapitalindkomst.Map((kapitalindkomst, index) => Math.Min(Math.Min(kapitalindkomst, samletkapitalindkomst), underskud[index]).NonNegative());
		}
	}
}