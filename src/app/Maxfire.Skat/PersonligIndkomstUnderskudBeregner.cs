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

			// I. Årets underskud

			var aaretsUnderskud = +(-personligeIndkomster);

			var aaretsRestunderskud = aaretsUnderskud;

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
				aaretsRestunderskud = overfoertRestunderskud.Swap();

				var udnyttetUnderskud = aaretsUnderskud - aaretsRestunderskud;
				// TODO: Side-effekt
				// Nulstilling af årets underskud
				udnyttetUnderskud.Each((underskud, index) => indkomster[index].ModregnetUnderskudPersonligIndkomst -= underskud);
			}

			// Modregning i ægtefællers samlede nettokapitalindkomst
			aaretsRestunderskud = getRestunderskudEfterNettoKapitalIndkomstModregning(indkomster, 
				aaretsRestunderskud, (value, index) => indkomster[index].ModregnetUnderskudPersonligIndkomst -= value);

			// restunderskud fremføres
			aaretsRestunderskud.Each((underskud, index) =>
			{
				// Nulstilling af årets underskud
				indkomster[index].ModregnetUnderskudPersonligIndkomst -= underskud;
				indkomster[index].UnderskudPersonligIndkomstTilFremfoersel += underskud;
			});

			// II. Fremført underskud

			var fremfoertUnderskud = indkomster.Map(x => x.FremfoertUnderskudPersonligIndkomst);

			// modregning i ægtefællers samlede positive nettokapitalindkomst
			var fremfoertRestunderskud = getRestunderskudEfterNettoKapitalIndkomstModregning(indkomster,
				fremfoertUnderskud, (value, index) => indkomster[index].FremfoertUnderskudPersonligIndkomst -= value);

			// modregning i egen og ægtefælles personlige indkomst
			fremfoertRestunderskud = getRestunderskudEfterPersonligIndkomstModregning(indkomster, 
				fremfoertRestunderskud, (value, index) => indkomster[index].FremfoertUnderskudPersonligIndkomst -= value);

			// restunderskud fremføres
			fremfoertRestunderskud.Each((underskud, index) =>
			{
				// Nulstilling af årets underskud
				indkomster[index].ModregnetUnderskudPersonligIndkomst -= underskud;
				indkomster[index].UnderskudPersonligIndkomstTilFremfoersel += underskud;
			});
		}

		private ValueTuple<decimal> getRestunderskudEfterPersonligIndkomstModregning(ValueTuple<PersonligeBeloeb> indkomster, 
			ValueTuple<decimal> underskud, Action<decimal, int> nulstilHandler)
		{
			var personligeIndkomster = indkomster.Map(x => x.PersonligIndkomst);
			var kapitalPensionsindskud = indkomster.Map(x => x.KapitalPensionsindskud);

			// Modregning i egen P.I.
			// Modregning i egen kapitalpensionsindskud
			var modregnEgenIndkomstResults = personligeIndkomster.ModregnUnderskud(underskud);

			// TODO: Side-effekt
			modregnEgenIndkomstResults.Each((modregnIndkomstResult, index) =>
				indkomster[index].ModregnetUnderskudPersonligIndkomst += modregnIndkomstResult.UdnyttetUnderskud);

			
			var modregninger = modregnEgenIndkomstResults.Map(x => x.UdnyttetUnderskud);
			personligeIndkomster -= modregninger;

			// Modregning i ægtefælles personlige indkomst med tillæg af kapitalpensionsindskud))
			if (indkomster.Size == 2)
			{
				// Modregning i ægtefælles (positive) personlige indkomst
				var overfoertUnderskud = underskud.Swap();
				var modregnIndkomstResults = personligeIndkomster.ModregnUnderskud(overfoertUnderskud);
				var overfoertRestunderskud = modregnIndkomstResults.Map(x => x.IkkeUdnyttetUnderskud);

				// TODO: Side-effekt
				modregnIndkomstResults.Each((modregnIndkomstResult, index) =>
					indkomster[index].ModregnetUnderskudPersonligIndkomst += modregnIndkomstResult.UdnyttetUnderskud);

				// Modregning i ægtefælles kapitalpensionsindskud
				var modregnKapitalPensionsindskudResults = kapitalPensionsindskud.ModregnUnderskud(overfoertRestunderskud);
				overfoertRestunderskud = modregnKapitalPensionsindskudResults.Map(x => x.IkkeUdnyttetUnderskud);
				var restunderskud = overfoertRestunderskud.Swap();

				var udnyttetUnderskud = underskud - restunderskud;
				// TODO: Side-effekt
				udnyttetUnderskud.Each(nulstilHandler);
			}

			// TODO
			return null;
		}

		private ValueTuple<decimal> getRestunderskudEfterNettoKapitalIndkomstModregning(ValueTuple<PersonligeBeloeb> indkomster, 
			ValueTuple<decimal> underskud, Action<decimal, int> nulstilHandler)
		{
			// Modregn underskud først i egen (positive) nettokapitalindkomst
			var nettokapitalindkomst = indkomster.Map(x => x.NettoKapitalIndkomstSkattegrundlag);
			var modregningEgenNettoKapitalIndkomst = getMuligModregning(nettokapitalindkomst, underskud);
			
			// TODO: Side-effekt
			modregningEgenNettoKapitalIndkomst.Each((modregning, index) => 
			{
				nulstilHandler(modregning, index);
				indkomster[index].ModregnetUnderskudNettoKapitalIndkomst += modregning;
			});

			nettokapitalindkomst -= modregningEgenNettoKapitalIndkomst;
			underskud -= modregningEgenNettoKapitalIndkomst;

			// Modregn dernæst restunderskud i ægtefælles (positive) nettokapitalindkomst
			if (indkomster.Size == 2)
			{
				var overfoertUnderskud = underskud.Swap();
				var modregningPartnersNettoKapitalIndkomst = getMuligModregning(nettokapitalindkomst, overfoertUnderskud);

				// TODO: Side-effekt
				modregningPartnersNettoKapitalIndkomst.Each((modregning, index) =>
				{
					// Nulstilling af årets underskud
					indkomster.PartnerOf(index).ModregnetUnderskudPersonligIndkomst -= modregning;
					indkomster[index].ModregnetUnderskudNettoKapitalIndkomst += modregning;
				});

				var udnyttetUnderskud = modregningPartnersNettoKapitalIndkomst.Swap();
				underskud -= udnyttetUnderskud;
			}

			return underskud;
		}

		private ValueTuple<decimal> getMuligModregning(ValueTuple<decimal> nettokapitalindkomst, ValueTuple<decimal> underskud)
		{
			var samletkapitalindkomst = nettokapitalindkomst.Sum();
			// TODO: Make overloads of Min, Max, Path.Combine in Maxfire.Core
			return nettokapitalindkomst.Map((kapitalindkomst, index) => 
				Math.Min(Math.Min(kapitalindkomst, samletkapitalindkomst), underskud[index]).NonNegative());
		}
	}
}