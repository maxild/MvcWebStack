using System;
using System.Linq;
using Maxfire.Core.Extensions;

namespace Maxfire.Skat
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////
	//
	// � 13, Stk. 3.
	//
	// Pkt. 1: Hvis den personlige indkomst er negativ, modregnes den inden opg�relsen af beregningsgrundlaget 
	// efter �� 6, 6 a og 7 i indkomst�rets positive kapitalindkomst.
	// Pkt. 2: Et resterende negativt bel�b fremf�res til modregning f�rst i kapitalindkomst og derefter i 
	// personlig indkomst med till�g af heri fradragne og ikke medregnede bel�b omfattet af bel�bsgr�nsen 
	// i pensionsbeskatningslovens � 16, stk. 1, for de f�lgende indkomst�r inden opg�relsen af 
	// beregningsgrundlagene efter �� 6, 6 a og 7. 
	// Pkt. 3: Negativ personlig indkomst kan kun fremf�res, i det omfang den ikke kan modregnes efter 
	// 1. eller 2. pkt. for et tidligere indkomst�r.
	//
	// � 13, Stk. 4.
	//
	// Pkt. 1: Hvis en gift persons personlige indkomst er negativ og �gtef�llerne er samlevende ved 
	// indkomst�rets udl�b, skal det negative bel�b inden opg�relse af beregningsgrundlagene efter 
	// �� 6, 6 a og 7 modregnes i den anden �gtef�lles personlige indkomst med till�g af heri 
	// fradragne og ikke medregnede bel�b omfattet af bel�bsgr�nsen i pensionsbeskatningslovens � 16, stk. 1. 
	// Pkt. 2: Et overskydende negativt bel�b modregnes i �gtef�llernes positive kapitalindkomst opgjort 
	// under �t.
	// Pkt. 3: Har begge �gtef�ller positiv kapitalindkomst, modregnes det negative bel�b fortrinsvis 
	// i den skattepligtiges kapitalindkomst og derefter i �gtef�llens kapitalindkomst.
	// Pkt. 4: Modregning sker, f�r �gtef�llens egne uudnyttede underskud fra tidligere indkomst�r 
	// fremf�res efter 5.-8. pkt.
	// Pkt. 5: Et negativt bel�b, der herefter ikke er modregnet, fremf�res inden for de f�lgende 
	// indkomst�r, i det omfang bel�bet ikke kan modregnes for et tidligere indkomst�r, til modregning 
	// inden opg�relsen af beregningsgrundlagene efter �� 6, 6 a og 7.
	// Pkt. 6: Hvert �r modregnes negativ personlig indkomst f�rst i �gtef�llernes positive kapitalindkomst 
	// opgjort under �t, derefter i den skattepligtiges personlige indkomst med till�g af heri fradragne 
	// og ikke medregnede bel�b omfattet af bel�bsgr�nsen i pensionsbeskatningslovens � 16, stk. 1, og 
	// endelig i �gtef�llens personlige indkomst med till�g af heri fradragne og ikke medregnede bel�b 
	// omfattet af bel�bsgr�nsen i pensionsbeskatningslovens � 16, stk. 1. 3. pkt. finder tilsvarende 
	// anvendelse. 
	// Pkt. 7: Hvis �gtef�llen ligeledes har uudnyttet underskud vedr�rende tidligere indkomst�r, skal 
	// �gtef�llens egne underskud modregnes f�rst. 
	//
	//////////////////////////////////////////////////////////////////////////////////////////////////////////
	public class PersonligIndkomstUnderskudBeregner
	{
		public void ModregningAfUnderskud(ValueTuple<PersonligeBeloeb> indkomster)
		{
			var personligeIndkomster = indkomster.Map(x => x.PersonligIndkomst);
			var kapitalPensionsindskud = indkomster.Map(x => x.KapitalPensionsindskud);

			// I. �rets underskud

			var aaretsUnderskud = +(-personligeIndkomster);

			var aaretsRestunderskud = aaretsUnderskud;

			if (indkomster.Size == 2)
			{
				// Modregning i �gtef�lles (positive) personlige indkomst
				var overfoertUnderskud = aaretsUnderskud.Swap();
				var modregnIndkomstResults = personligeIndkomster.ModregnUnderskud(overfoertUnderskud);
				var overfoertRestunderskud = modregnIndkomstResults.Map(x => x.IkkeUdnyttetUnderskud);

				// TODO: Side-effekt
				modregnIndkomstResults.Each((modregnIndkomstResult, index) => 
					indkomster[index].ModregnetUnderskudPersonligIndkomst += modregnIndkomstResult.UdnyttetUnderskud);
				
				// Modregning i �gtef�lles kapitalpensionsindskud
				var modregnKapitalPensionsindskudResults = kapitalPensionsindskud.ModregnUnderskud(overfoertRestunderskud);
				overfoertRestunderskud = modregnKapitalPensionsindskudResults.Map(x => x.IkkeUdnyttetUnderskud);
				aaretsRestunderskud = overfoertRestunderskud.Swap();

				var udnyttetUnderskud = aaretsUnderskud - aaretsRestunderskud;
				// TODO: Side-effekt
				// Nulstilling af �rets underskud
				udnyttetUnderskud.Each((underskud, index) => indkomster[index].ModregnetUnderskudPersonligIndkomst -= underskud);
			}

			// Modregning i �gtef�llers samlede nettokapitalindkomst
			aaretsRestunderskud = getRestunderskudEfterNettoKapitalIndkomstModregning(indkomster, 
				aaretsRestunderskud, (value, index) => indkomster[index].ModregnetUnderskudPersonligIndkomst -= value);

			// restunderskud fremf�res
			aaretsRestunderskud.Each((underskud, index) =>
			{
				// Nulstilling af �rets underskud
				indkomster[index].ModregnetUnderskudPersonligIndkomst -= underskud;
				indkomster[index].UnderskudPersonligIndkomstTilFremfoersel += underskud;
			});

			// II. Fremf�rt underskud

			var fremfoertUnderskud = indkomster.Map(x => x.FremfoertUnderskudPersonligIndkomst);

			// modregning i �gtef�llers samlede positive nettokapitalindkomst
			var fremfoertRestunderskud = getRestunderskudEfterNettoKapitalIndkomstModregning(indkomster,
				fremfoertUnderskud, (value, index) => indkomster[index].FremfoertUnderskudPersonligIndkomst -= value);

			// modregning i egen og �gtef�lles personlige indkomst
			fremfoertRestunderskud = getRestunderskudEfterPersonligIndkomstModregning(indkomster, 
				fremfoertRestunderskud, (value, index) => indkomster[index].FremfoertUnderskudPersonligIndkomst -= value);

			// restunderskud fremf�res
			fremfoertRestunderskud.Each((underskud, index) =>
			{
				// Nulstilling af �rets underskud
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

			// Modregning i �gtef�lles personlige indkomst med till�g af kapitalpensionsindskud))
			if (indkomster.Size == 2)
			{
				// Modregning i �gtef�lles (positive) personlige indkomst
				var overfoertUnderskud = underskud.Swap();
				var modregnIndkomstResults = personligeIndkomster.ModregnUnderskud(overfoertUnderskud);
				var overfoertRestunderskud = modregnIndkomstResults.Map(x => x.IkkeUdnyttetUnderskud);

				// TODO: Side-effekt
				modregnIndkomstResults.Each((modregnIndkomstResult, index) =>
					indkomster[index].ModregnetUnderskudPersonligIndkomst += modregnIndkomstResult.UdnyttetUnderskud);

				// Modregning i �gtef�lles kapitalpensionsindskud
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
			// Modregn underskud f�rst i egen (positive) nettokapitalindkomst
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

			// Modregn dern�st restunderskud i �gtef�lles (positive) nettokapitalindkomst
			if (indkomster.Size == 2)
			{
				var overfoertUnderskud = underskud.Swap();
				var modregningPartnersNettoKapitalIndkomst = getMuligModregning(nettokapitalindkomst, overfoertUnderskud);

				// TODO: Side-effekt
				modregningPartnersNettoKapitalIndkomst.Each((modregning, index) =>
				{
					// Nulstilling af �rets underskud
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