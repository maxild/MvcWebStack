using System;
using Maxfire.TestCommons.AssertExtensions;
using Xunit;

namespace Maxfire.Skat.UnitTests
{
	/// <summary>
	/// Eksempler fra SKATs publikation "Beregning af personlige indkomstskatter mv. 2009"
	/// </summary>
	public class Eksempler
	{
		[Fact]
		public void Eksempel_2_Bundskattegrundlag_Gifte()
		{
			var indkomster = new ValueTuple<PersonligeBeloeb>(
				new PersonligeBeloeb
				{
					PersonligIndkomst = 100000,
					NettoKapitalIndkomst = -50000,
				},
				new PersonligeBeloeb
				{
					PersonligIndkomst = 400000,
					NettoKapitalIndkomst = 60000
				});

			var bundskatGrundlagBeregner = new BundskatGrundlagBeregner();
			var bundskatGrundlag = bundskatGrundlagBeregner.BeregnGrundlag(indkomster);

			bundskatGrundlag[0].ShouldEqual(100000);
			bundskatGrundlag[1].ShouldEqual(410000);
		}

		[Fact]
		public void Eksempel_3_Mellemskattegrundlag_Gifte()
		{
			var indkomster = new ValueTuple<PersonligeBeloeb>(
				new PersonligeBeloeb
				{
					PersonligIndkomst = 100000,
					NettoKapitalIndkomst = -50000,
				},
				new PersonligeBeloeb
				{
					PersonligIndkomst = 600000,
					NettoKapitalIndkomst = 80000
				});

			var mellemskatGrundlagBeregner = new MellemskatGrundlagBeregner();

			var grundlagFoerBundfradrag = mellemskatGrundlagBeregner.BeregnBruttoGrundlag(indkomster);
			var udnyttetBundfradrag = mellemskatGrundlagBeregner.BeregnSambeskattetBundfradrag(indkomster);
			var grundlag = mellemskatGrundlagBeregner.BeregnGrundlag(indkomster);

			grundlagFoerBundfradrag[0].ShouldEqual(100000);
			grundlagFoerBundfradrag[1].ShouldEqual(630000);
			udnyttetBundfradrag[0].ShouldEqual(100000);
			udnyttetBundfradrag[1].ShouldEqual(594400);
			grundlag[0].ShouldEqual(0);
			grundlag[1].ShouldEqual(35600);
		}

		[Fact]
		public void Eksempel_4_Topskattegrundlag_Gifte()
		{
			Constants.Brug2009Vaerdier();

			const decimal indskudPaaPrivatTegnetKapitalPension = 32000;

			var indkomster = new ValueTuple<PersonligeBeloeb>(
				new PersonligeBeloeb
				{
					PersonligIndkomst = 368000 - indskudPaaPrivatTegnetKapitalPension,
					NettoKapitalIndkomst = 28500,
					KapitalPensionsindskud = indskudPaaPrivatTegnetKapitalPension,
					LigningsmaessigeFradrag = 15000
				},
				new PersonligeBeloeb
				{
					PersonligIndkomst = 92000,
					NettoKapitalIndkomst = 18500,
					LigningsmaessigeFradrag = 8000
				});

			var topskatBeregner = new TopskatBeregner();
			var topskat = topskatBeregner.BeregnSkat(indkomster);

			topskat[0].ShouldEqual(7395);
			topskat[1].ShouldEqual(2775);
		}

		[Fact]
		public void Eksempel_5_SkatAfAktieindkomst()
		{
			// TODO
		}

		[Fact]
		public void Eksempel_6_IngenUnderskud_Ugift()
		{
			Constants.Brug2009Vaerdier();

			// Note: Indkomstopg�relsen mangler

			ValueTuple<KommunaleSatser> kommunaleSatser = getKommunaleSatserForUgift();

			var indkomster = new ValueTuple<PersonligeBeloeb>(
				new PersonligeBeloeb
					{
						PersonligIndkomst = 358395,
						NettoKapitalIndkomst = 8500,
						LigningsmaessigeFradrag = 24600,
						KapitalPensionsindskud = 46000
					});

			indkomster[0].SkattepligtigIndkomst.ShouldEqual(342295);

			var skatterBeregner = new SkatBeregner();
			var skatter = skatterBeregner.BeregnSkat(indkomster, kommunaleSatser);
			
			// Skatter f�r modregning af underskud og personfradrag
			skatter[0].Bundskat
				.RoundMoney().ShouldEqual(18491.51m);
			skatter[0].Mellemskat
				.RoundMoney().ShouldEqual(1181.70m);
			skatter[0].Topskat
				.RoundMoney().ShouldEqual(9854.25m);
			skatter[0].Sundhedsbidrag
				.RoundMoney().ShouldEqual(27383.60m);
			skatter[0].KommunalIndkomstskatOgKirkeskat
				.RoundMoney().ShouldEqual(83143.46m);

			// Beregning af skattev�rdier af personfradrag
			var personFradragBeregner = new PersonfradragBeregner();
			var skattevaerdier = personFradragBeregner.BeregnSkattevaerdierAfPersonfradrag(kommunaleSatser);

			skattevaerdier[0].Bundskat.ShouldEqual(2162.16m);
			skattevaerdier[0].Sundhedsbidrag.ShouldEqual(3432);
			skattevaerdier[0].KommunalIndkomstskatOgKirkeskat.ShouldEqual(10420.41m);
		}

		[Fact] 
		public void Eksempel_9()
		{
			Constants.Brug2009Vaerdier();

			// Note: Indkomstopg�relsen mangler

			var kommunaleSatser = getKommunaleSatserForGifte();

			var indkomster = new ValueTuple<PersonligeBeloeb>(
				new PersonligeBeloeb
					{
						PersonligIndkomst = 388000,
						NettoKapitalIndkomst = 13500,
						LigningsmaessigeFradrag = 21600,
						KapitalPensionsindskud = 32000
					},
				new PersonligeBeloeb
					{
						PersonligIndkomst = 150000,
						NettoKapitalIndkomst = 8500,
						LigningsmaessigeFradrag = 8180
					});

			indkomster[0].SkattepligtigIndkomst.ShouldEqual(379900);
			indkomster[1].SkattepligtigIndkomst.ShouldEqual(150320);

			var skatterBeregner = new SkatBeregner();
			var skatter = skatterBeregner.BeregnSkat(indkomster, kommunaleSatser);

			// Skatter f�r modregning af underskud og personfradrag
			skatter[0].Bundskat
				.RoundMoney().ShouldEqual(20235.60m);
			skatter[0].Mellemskat
				.RoundMoney().ShouldEqual(0);
			skatter[0].Topskat
				.RoundMoney().ShouldEqual(10920 + 2025);
			skatter[0].Sundhedsbidrag
				.RoundMoney().ShouldEqual(30392);
			skatter[0].KommunalIndkomstskatOgKirkeskat
				.RoundMoney().ShouldEqual(92277.71m);

			skatter[1].Bundskat
				.RoundMoney().ShouldEqual(7988.40m);
			skatter[1].Mellemskat
				.RoundMoney().ShouldEqual(0);
			skatter[1].Topskat
				.RoundMoney().ShouldEqual(1275);
			skatter[1].Sundhedsbidrag
				.RoundMoney().ShouldEqual(12025.60m);
			skatter[1].KommunalIndkomstskatOgKirkeskat
				.RoundMoney().ShouldEqual(36512.73m);

			var personFradragBeregner = new PersonfradragBeregner();
			var skattevaerdier = personFradragBeregner.BeregnSkattevaerdierAfPersonfradrag(kommunaleSatser);

			skattevaerdier[0].Bundskat.ShouldEqual(2162.16m);
			skattevaerdier[1].Bundskat.ShouldEqual(2162.16m);
			skattevaerdier[0].Sundhedsbidrag.ShouldEqual(3432);
			skattevaerdier[1].Sundhedsbidrag.ShouldEqual(3432);
			skattevaerdier[0].KommunalIndkomstskatOgKirkeskat.ShouldEqual(10420.41m);
			skattevaerdier[1].KommunalIndkomstskatOgKirkeskat.ShouldEqual(10420.41m);
		}

		[Fact]
		public void Eksempel_10_GifteMedNegativNettoKapitalIndkomstOgUudnyttetBundfradrag()
		{
			Constants.Brug2009Vaerdier();

			const decimal indskudPaaPrivatTegnetKapitalPension = 32000;

			var kommunaleSatser = getKommunaleSatserForGifte();

			var indkomster = new ValueTuple<PersonligeBeloeb>(
				new PersonligeBeloeb
				{
					PersonligIndkomst = 600000 - indskudPaaPrivatTegnetKapitalPension,
					NettoKapitalIndkomst = -11500,
					LigningsmaessigeFradrag = 21000,
					KapitalPensionsindskud = indskudPaaPrivatTegnetKapitalPension
				},
				new PersonligeBeloeb
				{
					PersonligIndkomst = 150000,
					NettoKapitalIndkomst = 8500,
					LigningsmaessigeFradrag = 7772
				});

			indkomster[0].SkattepligtigIndkomst.ShouldEqual(535500);
			indkomster[1].SkattepligtigIndkomst.ShouldEqual(150728);

			var skatterBeregner = new SkatBeregner();
			var skatter = skatterBeregner.BeregnSkat(indkomster, kommunaleSatser);

			// Skatter f�r modregning af underskud og personfradrag
			skatter[0].Bundskat
				.RoundMoney().ShouldEqual(28627.20m);
			skatter[0].Mellemskat
				.RoundMoney().ShouldEqual(1416);
			skatter[0].Topskat
				.RoundMoney().ShouldEqual(37920);
			skatter[0].Sundhedsbidrag
				.RoundMoney().ShouldEqual(42840);
			skatter[0].KommunalIndkomstskatOgKirkeskat
				.RoundMoney().ShouldEqual(130072.95m);

			skatter[1].Bundskat
				.RoundMoney().ShouldEqual(7560);
			skatter[1].Mellemskat
				.RoundMoney().ShouldEqual(0);
			skatter[1].Topskat
				.RoundMoney().ShouldEqual(0);
			skatter[1].Sundhedsbidrag
				.RoundMoney().ShouldEqual(12058.24m);
			skatter[1].KommunalIndkomstskatOgKirkeskat
				.RoundMoney().ShouldEqual(36611.83m);

			var personFradragBeregner = new PersonfradragBeregner();
			var skattevaerdier = personFradragBeregner.BeregnSkattevaerdierAfPersonfradrag(kommunaleSatser);

			skattevaerdier[0].Bundskat.ShouldEqual(2162.16m);
			skattevaerdier[1].Bundskat.ShouldEqual(2162.16m);
			skattevaerdier[0].Sundhedsbidrag.ShouldEqual(3432);
			skattevaerdier[1].Sundhedsbidrag.ShouldEqual(3432);
			skattevaerdier[0].KommunalIndkomstskatOgKirkeskat.ShouldEqual(10420.41m);
			skattevaerdier[1].KommunalIndkomstskatOgKirkeskat.ShouldEqual(10420.41m);
		}

		[Fact] 
		public void Eksempel_12_NegativSkattepigtigIndkomst_Ugift()
		{
			Constants.Brug2009Vaerdier();

			var indkomster = new ValueTuple<PersonligeBeloeb>(
				new PersonligeBeloeb
					{
						PersonligIndkomst = 360000,
						NettoKapitalIndkomst = -310000,
						LigningsmaessigeFradrag = 80000
					});

			var kommunaleSatser = getKommunaleSatserForUgift();

			indkomster[0].SkattepligtigIndkomst.ShouldEqual(-30000);

			var skatterBeregner = new SkatBeregner();
			var skatter = skatterBeregner.BeregnSkat(indkomster, kommunaleSatser);
			
			// Skatter f�r modregning af underskud og personfradrag
			skatter[0].Bundskat.ShouldEqual(18144);
			skatter[0].Mellemskat.ShouldEqual(768);
			skatter[0].Topskat.ShouldEqual(1920);
			skatter[0].Sundhedsbidrag.ShouldEqual(0);
			skatter[0].Kommuneskat.ShouldEqual(0);

			var underskudBeregner = new UnderskudBeregner();
			var modregnResults = underskudBeregner.Beregn(indkomster, skatter, kommunaleSatser);
			var skatterFoerPersonfradrag = modregnResults.Map(x => x.ModregnedeSkatter);
			var ikkeUdnyttedeSkattevaerdier = modregnResults.Map(x => x.IkkeUdnyttetSkattevaerdi);

			// Fuld udnyttelse, idet hele skattev�rdien kan rummes i bundskatten
			ikkeUdnyttedeSkattevaerdier[0].ShouldEqual(0);

			skatterFoerPersonfradrag[0].Bundskat.ShouldEqual(8457);
			skatterFoerPersonfradrag[0].Mellemskat.ShouldEqual(768);
			skatterFoerPersonfradrag[0].Topskat.ShouldEqual(1920);
			skatterFoerPersonfradrag[0].Sundhedsbidrag.ShouldEqual(0);
			skatterFoerPersonfradrag[0].Kommuneskat.ShouldEqual(0);

			var personfradragBeregner = new PersonfradragBeregner();
			modregnResults = personfradragBeregner.BeregnSkatEfterPersonfradrag(skatterFoerPersonfradrag, kommunaleSatser);
			var skatterEfterPersonfradrag = modregnResults.Map(x => x.ModregnedeSkatter);

			skatterEfterPersonfradrag[0].Bundskat.ShouldEqual(0);
			skatterEfterPersonfradrag[0].Mellemskat.ShouldEqual(0);
			skatterEfterPersonfradrag[0].Topskat.ShouldEqual(0);
			skatterEfterPersonfradrag[0].Sundhedsbidrag.ShouldEqual(0);
			skatterEfterPersonfradrag[0].Kommuneskat.ShouldEqual(0);
		}

		[Fact]
		public void Eksempel_13_HeleUnderskuddetFremfoeres_Ugift()
		{
			Constants.Brug2009Vaerdier();

			var indkomster = new ValueTuple<PersonligeBeloeb>(
				new PersonligeBeloeb
				{
					NettoKapitalIndkomst = -55000,
					LigningsmaessigeFradrag = 4000
				});

			var kommunaleSatser = getKommunaleSatserForUgift();

			indkomster[0].SkattepligtigIndkomst.ShouldEqual(-59000);

			var skatterBeregner = new SkatBeregner();
			var skatter = skatterBeregner.BeregnSkat(indkomster, kommunaleSatser);

			// Skatter f�r modregning af underskud og personfradrag
			skatter[0].Bundskat.ShouldEqual(0);
			skatter[0].Mellemskat.ShouldEqual(0);
			skatter[0].Topskat.ShouldEqual(0);
			skatter[0].Sundhedsbidrag.ShouldEqual(0);
			skatter[0].Kommuneskat.ShouldEqual(0);

			var underskudBeregner = new UnderskudBeregner();
			var modregnResults = underskudBeregner.Beregn(indkomster, skatter, kommunaleSatser);
			var skatterFoerPersonfradrag = modregnResults.Map(x => x.ModregnedeSkatter);
			var ikkeUdnyttedeUnderskud = modregnResults.Map(x => x.IkkeUdnyttetFradrag);
			var ikkeUdnyttedeSkattevaerdier = modregnResults.Map(x => x.IkkeUdnyttetSkattevaerdi);

			skatterFoerPersonfradrag[0].Bundskat.ShouldEqual(0);
			skatterFoerPersonfradrag[0].Mellemskat.ShouldEqual(0);
			skatterFoerPersonfradrag[0].Topskat.ShouldEqual(0);
			skatterFoerPersonfradrag[0].Sundhedsbidrag.ShouldEqual(0);
			skatterFoerPersonfradrag[0].Kommuneskat.ShouldEqual(0);

			ikkeUdnyttedeUnderskud[0].ShouldEqual(59000);
			ikkeUdnyttedeSkattevaerdier[0].ShouldEqual(59000 * (kommunaleSatser[0].KommuneOgKirkeskattesats + Constants.Sundhedsbidragsats));
		}

		[Fact]
		public void Eksempel_14_DelvisModregningDelvisFremfoerselAfUnderskud_Ugift()
		{
			Constants.Brug2009Vaerdier();

			var indkomster = new ValueTuple<PersonligeBeloeb>(
				new PersonligeBeloeb
				{
					PersonligIndkomst = 360000,
					NettoKapitalIndkomst = -400000,
					LigningsmaessigeFradrag = 50000
				});

			var kommunaleSatser = getKommunaleSatserForUgift();

			indkomster[0].SkattepligtigIndkomst.ShouldEqual(-90000);

			var skatterBeregner = new SkatBeregner();
			var skatter = skatterBeregner.BeregnSkat(indkomster, kommunaleSatser);

			// Skatter f�r modregning af underskud og personfradrag
			skatter[0].Bundskat.ShouldEqual(18144);
			skatter[0].Mellemskat.ShouldEqual(768);
			skatter[0].Topskat.ShouldEqual(1920);
			skatter[0].Sundhedsbidrag.ShouldEqual(0);
			skatter[0].Kommuneskat.ShouldEqual(0);

			var underskudBeregner = new UnderskudBeregner();
			var modregnResults = underskudBeregner.Beregn(indkomster, skatter, kommunaleSatser);
			var skatterFoerPersonfradrag = modregnResults.Map(x => x.ModregnedeSkatter);
			var ikkeUdnyttedeUnderskud = modregnResults.Map(x => x.IkkeUdnyttetFradrag);
			var ikkeUdnyttedeSkattevaerdier = modregnResults.Map(x => x.IkkeUdnyttetSkattevaerdi);

			skatterFoerPersonfradrag[0].Bundskat.ShouldEqual(0);
			skatterFoerPersonfradrag[0].Mellemskat.ShouldEqual(0);
			skatterFoerPersonfradrag[0].Topskat.ShouldEqual(0);
			skatterFoerPersonfradrag[0].Sundhedsbidrag.ShouldEqual(0);
			skatterFoerPersonfradrag[0].Kommuneskat.ShouldEqual(0);

			// Dette underskud skal fremf�res til f�lgende skatte�r
			ikkeUdnyttedeUnderskud[0].ShouldEqual(25484.67m);
			ikkeUdnyttedeSkattevaerdier[0].ShouldEqual(8229);

			var personfradragBeregner = new PersonfradragBeregner();
			modregnResults= personfradragBeregner.BeregnSkatEfterPersonfradrag(skatterFoerPersonfradrag, kommunaleSatser);
			var skatterEfterPersonfradrag = modregnResults.Map(x => x.ModregnedeSkatter);
			var tabtPersonfradrag = modregnResults.Map(x => x.IkkeUdnyttetFradrag);

			// Hele personfradraget fortabes
			tabtPersonfradrag[0].ShouldEqual(Constants.Personfradrag);
			skatterEfterPersonfradrag.ShouldEqual(skatterFoerPersonfradrag);
		}

		[Fact]
		public void Eksempel_15_FuldModregningAfFremfoertUnderskudOgIntetUnderskudTildFremfoersel_Ugift()
		{
			Constants.Brug2009Vaerdier();

			var indkomster = new ValueTuple<PersonligeBeloeb>(
				new PersonligeBeloeb
				{
					FremfoertUnderskudSkattepligtigIndkomst = 12000,
					PersonligIndkomst = 170000,
					NettoKapitalIndkomst = -30000,
					LigningsmaessigeFradrag = 10000
				});

			var kommunaleSatser = getKommunaleSatserForUgift();

			// TODO: Ingen sondring mellem skattepligtig indkomst f�r modregning af fremf�rt underskud og efter modregning af fremf�rt underskud
			indkomster[0].SkattepligtigIndkomst.ShouldEqual(118000);

			var skatterBeregner = new SkatBeregner();
			var skatter = skatterBeregner.BeregnSkat(indkomster, kommunaleSatser);

			// Skatter f�r modregning af underskud og personfradrag
			skatter[0].Bundskat.ShouldEqual(8568);
			skatter[0].Mellemskat.ShouldEqual(0);
			skatter[0].Topskat.ShouldEqual(0);
			skatter[0].Sundhedsbidrag.ShouldEqual(9440);
			skatter[0].KommunalIndkomstskatOgKirkeskat.ShouldEqual(28662.20m);

			var underskudBeregner = new UnderskudBeregner();
			var modregnResults = underskudBeregner.Beregn(indkomster, skatter, kommunaleSatser);
			var skatterFoerPersonfradrag = modregnResults.Map(x => x.ModregnedeSkatter);
			var ikkeUdnyttedeUnderskud = modregnResults.Map(x => x.IkkeUdnyttetFradrag);
			var ikkeUdnyttedeSkattevaerdier = modregnResults.Map(x => x.IkkeUdnyttetSkattevaerdi);

			// Intet underskud => Ingen modregning
			skatterFoerPersonfradrag.ShouldEqual(skatter);

			ikkeUdnyttedeUnderskud[0].ShouldEqual(0);
			ikkeUdnyttedeSkattevaerdier[0].ShouldEqual(0);

			var personfradragBeregner = new PersonfradragBeregner();
			modregnResults = personfradragBeregner.BeregnSkatEfterPersonfradrag(skatterFoerPersonfradrag, kommunaleSatser);
			var skatterEfterPersonfradrag = modregnResults.Map(x => x.ModregnedeSkatter);

			skatterEfterPersonfradrag[0].Bundskat.ShouldEqual(8568 - 2162.16m);
			skatterEfterPersonfradrag[0].Mellemskat.ShouldEqual(0);
			skatterEfterPersonfradrag[0].Topskat.ShouldEqual(0);
			skatterEfterPersonfradrag[0].Sundhedsbidrag.ShouldEqual(9440 - 3432);
			skatterEfterPersonfradrag[0].KommunalIndkomstskatOgKirkeskat.ShouldEqual(28662.20m - 10420.41m);
		}

		[Fact]
		public void Eksempel_16()
		{
			// TODO
		}

		private static ValueTuple<KommunaleSatser> getKommunaleSatserForUgift()
		{
			return new ValueTuple<KommunaleSatser>(
				new KommunaleSatser
				{
					Kommuneskattesats = 0.237m,
					Kirkeskattesats = 0.0059m
				});
		}

		private static ValueTuple<KommunaleSatser> getKommunaleSatserForGifte()
		{
			return new ValueTuple<KommunaleSatser>(
				new KommunaleSatser
				{
					Kommuneskattesats = 0.237m,
					Kirkeskattesats = 0.0059m
				},
				new KommunaleSatser
				{
					Kommuneskattesats = 0.237m,
					Kirkeskattesats = 0.0059m
				});
		}
	}
}