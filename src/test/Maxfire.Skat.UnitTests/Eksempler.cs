using Maxfire.TestCommons.AssertExtensions;
using Xunit;

namespace Maxfire.Skat.UnitTests
{
	// TODO: Øre- Afrundinger mangler at blive lavet ordentligt

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

			// Note: Indkomstopgørelsen mangler

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
			
			// Skatter før modregning af underskud og personfradrag
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

			// Beregning af skatteværdier af personfradrag
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

			// Note: Indkomstopgørelsen mangler

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

			// Skatter før modregning af underskud og personfradrag
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

			// Skatter før modregning af underskud og personfradrag
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
			
			// Skatter før modregning af underskud og personfradrag
			skatter[0].Bundskat.ShouldEqual(18144);
			skatter[0].Mellemskat.ShouldEqual(768);
			skatter[0].Topskat.ShouldEqual(1920);
			skatter[0].Sundhedsbidrag.ShouldEqual(0);
			skatter[0].Kommuneskat.ShouldEqual(0);

			var underskudBeregner = new UnderskudBeregner();
			var modregnResults = underskudBeregner.Beregn(indkomster, skatter, kommunaleSatser);
			var skatterFoerPersonfradrag = modregnResults.Map(x => x.ModregnedeSkatter);
			var ikkeUdnyttedeSkattevaerdier = modregnResults.Map(x => x.IkkeUdnyttetSkattevaerdi);

			// Fuld udnyttelse, idet hele skatteværdien kan rummes i bundskatten
			ikkeUdnyttedeSkattevaerdier[0].ShouldEqual(0);

			skatterFoerPersonfradrag[0].Bundskat.ShouldEqual(8457);
			skatterFoerPersonfradrag[0].Mellemskat.ShouldEqual(768);
			skatterFoerPersonfradrag[0].Topskat.ShouldEqual(1920);
			skatterFoerPersonfradrag[0].Sundhedsbidrag.ShouldEqual(0);
			skatterFoerPersonfradrag[0].Kommuneskat.ShouldEqual(0);

			var personfradragBeregner = new PersonfradragBeregner();
			modregnResults = personfradragBeregner.ModregningAfPersonfradrag(skatterFoerPersonfradrag, kommunaleSatser);
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

			// Skatter før modregning af underskud og personfradrag
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

			// Skatter før modregning af underskud og personfradrag
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

			// Dette underskud skal fremføres til følgende skatteår
			ikkeUdnyttedeUnderskud[0].ShouldEqual(25484.67m);
			ikkeUdnyttedeSkattevaerdier[0].ShouldEqual(8229);

			var personfradragBeregner = new PersonfradragBeregner();
			modregnResults= personfradragBeregner.ModregningAfPersonfradrag(skatterFoerPersonfradrag, kommunaleSatser);
			var skatterEfterPersonfradrag = modregnResults.Map(x => x.ModregnedeSkatter);
			var tabtPersonfradrag = modregnResults.Map(x => x.IkkeUdnyttetFradrag);

			// Hele personfradraget fortabes
			tabtPersonfradrag[0].ShouldEqual(Constants.Personfradrag);
			skatterEfterPersonfradrag.ShouldEqual(skatterFoerPersonfradrag);
		}

		[Fact]
		public void Eksempel_15_FuldModregningAfFremfoertUnderskudOgIntetUnderskudTilFremfoersel_Ugift()
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

			// TODO: Ingen sondring mellem skattepligtig indkomst før modregning af fremført underskud og efter modregning af fremført underskud
			indkomster[0].SkattepligtigIndkomst.ShouldEqual(118000);

			var skatterBeregner = new SkatBeregner();
			var skatter = skatterBeregner.BeregnSkat(indkomster, kommunaleSatser);

			// Skatter før modregning af underskud og personfradrag
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
			modregnResults = personfradragBeregner.ModregningAfPersonfradrag(skatterFoerPersonfradrag, kommunaleSatser);
			var skatterEfterPersonfradrag = modregnResults.Map(x => x.ModregnedeSkatter);

			skatterEfterPersonfradrag[0].Bundskat.ShouldEqual(8568 - 2162.16m);
			skatterEfterPersonfradrag[0].Mellemskat.ShouldEqual(0);
			skatterEfterPersonfradrag[0].Topskat.ShouldEqual(0);
			skatterEfterPersonfradrag[0].Sundhedsbidrag.ShouldEqual(9440 - 3432);
			skatterEfterPersonfradrag[0].KommunalIndkomstskatOgKirkeskat.ShouldEqual(28662.20m - 10420.41m);
		}

		[Fact]
		public void Eksempel_16_ModregningAfFremfoertUnderskudOgUnderskudTilFremfoersel_Ugift()
		{
			Constants.Brug2009Vaerdier();

			var indkomster = new ValueTuple<PersonligeBeloeb>(
				new PersonligeBeloeb
				{
					FremfoertUnderskudSkattepligtigIndkomst = 500000,
					PersonligIndkomst = 360000,
					NettoKapitalIndkomst = -10000,
					LigningsmaessigeFradrag = 50000
				});

			var kommunaleSatser = getKommunaleSatserForUgift();

			// TODO: Ingen sondring mellem skattepligtig indkomst før modregning af fremført underskud og efter modregning af fremført underskud
			indkomster[0].SkattepligtigIndkomst.ShouldEqual(-200000);

			var skatterBeregner = new SkatBeregner();
			var skatter = skatterBeregner.BeregnSkat(indkomster, kommunaleSatser);

			// Skatter før modregning af underskud og personfradrag
			skatter[0].Bundskat.ShouldEqual(18144);
			skatter[0].Mellemskat.ShouldEqual(768);
			skatter[0].Topskat.ShouldEqual(1920);
			skatter[0].Sundhedsbidrag.ShouldEqual(0);
			skatter[0].KommunalIndkomstskatOgKirkeskat.ShouldEqual(0);
			skatter[0].Sum().ShouldEqual(20832);

			var underskudBeregner = new UnderskudBeregner();
			var modregnResults = underskudBeregner.Beregn(indkomster, skatter, kommunaleSatser);
			var skatterFoerPersonfradrag = modregnResults.Map(x => x.ModregnedeSkatter);
			var ikkeUdnyttedeUnderskud = modregnResults.Map(x => x.IkkeUdnyttetFradrag);
			var ikkeUdnyttedeSkattevaerdier = modregnResults.Map(x => x.IkkeUdnyttetSkattevaerdi);

			skatterFoerPersonfradrag[0].Bundskat.ShouldEqual(0);
			skatterFoerPersonfradrag[0].Mellemskat.ShouldEqual(0);
			skatterFoerPersonfradrag[0].Topskat.ShouldEqual(0);
			skatterFoerPersonfradrag[0].Sundhedsbidrag.ShouldEqual(0);
			skatterFoerPersonfradrag[0].KommunalIndkomstskatOgKirkeskat.ShouldEqual(0);

			// Dette fremføres til efterfølgende skatteår
			ikkeUdnyttedeUnderskud[0].ShouldEqual(135484.67m);
			ikkeUdnyttedeSkattevaerdier[0].ShouldEqual(200000 * 0.3229m - 20832);

			var personfradragBeregner = new PersonfradragBeregner();
			modregnResults = personfradragBeregner.ModregningAfPersonfradrag(skatterFoerPersonfradrag, kommunaleSatser);
			var skatterEfterPersonfradrag = modregnResults.Map(x => x.ModregnedeSkatter);
			var tabtPersonfradrag = modregnResults.Map(x => x.IkkeUdnyttetFradrag);

			tabtPersonfradrag[0].ShouldEqual(Constants.Personfradrag);
			skatterEfterPersonfradrag.ShouldEqual(skatterFoerPersonfradrag);
		}

		[Fact]
		public void Eksempel_17_ModregningAfAaretsOgFremfoertUnderskudOgResterendeUnderskudFremfoeres_Ugift()
		{
			Constants.Brug2009Vaerdier();

			var indkomster = new ValueTuple<PersonligeBeloeb>(
				new PersonligeBeloeb
				{
					FremfoertUnderskudSkattepligtigIndkomst = 210000,
					PersonligIndkomst = 20000,
					NettoKapitalIndkomst = -80000,
					AktieIndkomst = 200000
				});

			var kommunaleSatser = getKommunaleSatserForUgift();

			// TODO: Ingen sondring mellem skattepligtig indkomst før modregning af fremført underskud og efter modregning af fremført underskud
			indkomster[0].SkattepligtigIndkomst.ShouldEqual(-270000);

			var skatterBeregner = new SkatBeregner();
			var skatter = skatterBeregner.BeregnSkat(indkomster, kommunaleSatser);

			// Skatter før modregning af underskud og personfradrag
			skatter[0].Bundskat.ShouldEqual(1008);
			skatter[0].Mellemskat.ShouldEqual(0);
			skatter[0].Topskat.ShouldEqual(0);
			skatter[0].Sundhedsbidrag.ShouldEqual(0);
			skatter[0].KommunalIndkomstskatOgKirkeskat.ShouldEqual(0);
			skatter[0].AktieindkomstskatUnderGrundbeloebet.ShouldEqual(13524);
			skatter[0].AktieindkomstskatOverGrundbeloebet.ShouldEqual(24854 + 42255);

			var underskudBeregner = new UnderskudBeregner();
			var modregnResults = underskudBeregner.Beregn(indkomster, skatter, kommunaleSatser);
			var skatterFoerPersonfradrag = modregnResults.Map(x => x.ModregnedeSkatter);
			var ikkeUdnyttedeUnderskud = modregnResults.Map(x => x.IkkeUdnyttetFradrag);
			var ikkeUdnyttedeSkattevaerdier = modregnResults.Map(x => x.IkkeUdnyttetSkattevaerdi);

			skatterFoerPersonfradrag[0].Bundskat.ShouldEqual(0);
			skatterFoerPersonfradrag[0].Mellemskat.ShouldEqual(0);
			skatterFoerPersonfradrag[0].Topskat.ShouldEqual(0);
			skatterFoerPersonfradrag[0].Sundhedsbidrag.ShouldEqual(0);
			skatterFoerPersonfradrag[0].KommunalIndkomstskatOgKirkeskat.ShouldEqual(0);
			skatterFoerPersonfradrag[0].AktieindkomstskatUnderGrundbeloebet.ShouldEqual(13524);
			skatterFoerPersonfradrag[0].AktieindkomstskatOverGrundbeloebet.ShouldEqual(0);

			// Dette fremføres til efterfølgende skatteår
			ikkeUdnyttedeUnderskud[0].ShouldEqual(59046.14m);
			ikkeUdnyttedeSkattevaerdier[0].ShouldEqual(270000 * 0.3229m - (skatter[0].Sum() - skatter[0].AktieindkomstskatUnderGrundbeloebet));

			var personfradragBeregner = new PersonfradragBeregner();
			modregnResults = personfradragBeregner.ModregningAfPersonfradrag(skatterFoerPersonfradrag, kommunaleSatser);
			var skatterEfterPersonfradrag = modregnResults.Map(x => x.ModregnedeSkatter);
			var tabtPersonfradrag = modregnResults.Map(x => x.IkkeUdnyttetFradrag);

			tabtPersonfradrag[0].ShouldEqual(Constants.Personfradrag);
			skatterEfterPersonfradrag.ShouldEqual(skatterFoerPersonfradrag);
		}

		[Fact]
		public void Eksempel_18_ModregningFuldtUdViaNedbringelseAfPartnersSkattepligtigeIndkomst()
		{
			Constants.Brug2009Vaerdier();

			var indkomster = new ValueTuple<PersonligeBeloeb>(
				new PersonligeBeloeb
				{
					NettoKapitalIndkomst = -52000,
					LigningsmaessigeFradrag = 1000
				},
				new PersonligeBeloeb 
				{
					PersonligIndkomst = 260000,
					LigningsmaessigeFradrag = 8000
				});

			var kommunaleSatser = getKommunaleSatserForGifte();

			indkomster[0].SkattepligtigIndkomst.ShouldEqual(-53000);
			indkomster[1].SkattepligtigIndkomst.ShouldEqual(252000);

			var skatterBeregner = new SkatBeregner();
			var skatter = skatterBeregner.BeregnSkat(indkomster, kommunaleSatser);

			// Æ1 har ingen egne skatter at modregne underskud i
			skatter[0].Bundskat.ShouldEqual(0);
			skatter[0].Mellemskat.ShouldEqual(0);
			skatter[0].Topskat.ShouldEqual(0);
			skatter[0].AktieindkomstskatOverGrundbeloebet.ShouldEqual(0);
			// Og hele underskuddet overføres til Æ2, hvor det kan indeholdes i Æ2s skattepligtige indkomst

			var underskudBeregner = new UnderskudBeregner();
			var modregnResults = underskudBeregner.Beregn(indkomster, skatter, kommunaleSatser);
			var skatterFoerPersonfradrag = modregnResults.Map(x => x.ModregnedeSkatter);
			var ikkeUdnyttedeUnderskud = modregnResults.Map(x => x.IkkeUdnyttetFradrag);

			// O til 1 overførsel af hele underskuddet
			indkomster[0].ModregnetUnderskudSkattepligtigIndkomst.ShouldEqual(-53000);
			indkomster[1].ModregnetUnderskudSkattepligtigIndkomst.ShouldEqual(53000);
			// Der er ingen underskud til fremførsel
			ikkeUdnyttedeUnderskud[0].ShouldEqual(0);
			ikkeUdnyttedeUnderskud[1].ShouldEqual(0);

			skatterFoerPersonfradrag[0].Bundskat.ShouldEqual(0);
			skatterFoerPersonfradrag[0].Mellemskat.ShouldEqual(0);
			skatterFoerPersonfradrag[0].Topskat.ShouldEqual(0);
			skatterFoerPersonfradrag[0].AktieindkomstskatUnderGrundbeloebet.ShouldEqual(0);
			skatterFoerPersonfradrag[0].AktieindkomstskatOverGrundbeloebet.ShouldEqual(0);
			skatterFoerPersonfradrag[0].Sundhedsbidrag.ShouldEqual(0);
			skatterFoerPersonfradrag[0].KommunalIndkomstskatOgKirkeskat.ShouldEqual(0);

			skatterFoerPersonfradrag[1].Bundskat.ShouldEqual(13104);
			skatterFoerPersonfradrag[1].Mellemskat.ShouldEqual(0);
			skatterFoerPersonfradrag[1].Topskat.ShouldEqual(0);
			skatterFoerPersonfradrag[1].AktieindkomstskatUnderGrundbeloebet.ShouldEqual(0);
			skatterFoerPersonfradrag[1].AktieindkomstskatOverGrundbeloebet.ShouldEqual(0);
			skatterFoerPersonfradrag[1].Sundhedsbidrag.ShouldEqual(15920);
			skatterFoerPersonfradrag[1].KommunalIndkomstskatOgKirkeskat.ShouldEqual(48337.10m);

			var personfradragBeregner = new PersonfradragBeregner();
			modregnResults = personfradragBeregner.ModregningAfPersonfradrag(skatterFoerPersonfradrag, kommunaleSatser);
			var skatterEfterPersonfradrag = modregnResults.Map(x => x.ModregnedeSkatter);

			skatterEfterPersonfradrag[0].Bundskat.ShouldEqual(0);
			skatterEfterPersonfradrag[0].Mellemskat.ShouldEqual(0);
			skatterEfterPersonfradrag[0].Topskat.ShouldEqual(0);
			skatterEfterPersonfradrag[0].AktieindkomstskatUnderGrundbeloebet.ShouldEqual(0);
			skatterEfterPersonfradrag[0].AktieindkomstskatOverGrundbeloebet.ShouldEqual(0);
			skatterEfterPersonfradrag[0].Sundhedsbidrag.ShouldEqual(0);
			skatterEfterPersonfradrag[0].KommunalIndkomstskatOgKirkeskat.ShouldEqual(0);

			skatterEfterPersonfradrag[1].Bundskat.ShouldEqual(8779.68m);
			skatterEfterPersonfradrag[1].Mellemskat.ShouldEqual(0);
			skatterEfterPersonfradrag[1].Topskat.ShouldEqual(0);
			skatterEfterPersonfradrag[1].AktieindkomstskatUnderGrundbeloebet.ShouldEqual(0);
			skatterEfterPersonfradrag[1].AktieindkomstskatOverGrundbeloebet.ShouldEqual(0);
			skatterEfterPersonfradrag[1].Sundhedsbidrag.ShouldEqual(9056);
			skatterEfterPersonfradrag[1].KommunalIndkomstskatOgKirkeskat.ShouldEqual(27496.28m);
			skatterEfterPersonfradrag[1].Sum().ShouldEqual(45331.96m);
		}

		[Fact]
		public void Eksempel_19_ModregningEgenSkatOgPartnersSkattepligtigeIndkomst()
		{
			Constants.Brug2009Vaerdier();

			var indkomster = new ValueTuple<PersonligeBeloeb>(
				new PersonligeBeloeb
				{
					PersonligIndkomst = 50000,
					NettoKapitalIndkomst = -58000,
					LigningsmaessigeFradrag = 13000
				},
				new PersonligeBeloeb
				{
					PersonligIndkomst = 260000,
					NettoKapitalIndkomst = -4000,
					LigningsmaessigeFradrag = 8000
				});

			var kommunaleSatser = getKommunaleSatserForGifte();

			indkomster[0].SkattepligtigIndkomst.ShouldEqual(-21000);
			indkomster[1].SkattepligtigIndkomst.ShouldEqual(248000);

			var skatterBeregner = new SkatBeregner();
			var skatter = skatterBeregner.BeregnSkat(indkomster, kommunaleSatser);

			// Æ1 har egne skatter at modregne underskud i
			skatter[0].Bundskat.ShouldEqual(2520);
			skatter[0].Mellemskat.ShouldEqual(0);
			skatter[0].Topskat.ShouldEqual(0);
			skatter[0].AktieindkomstskatOverGrundbeloebet.ShouldEqual(0);

			var underskudBeregner = new UnderskudBeregner();
			var modregnResults = underskudBeregner.Beregn(indkomster, skatter, kommunaleSatser);
			var skatterFoerPersonfradrag = modregnResults.Map(x => x.ModregnedeSkatter);
			var udnyttetUnderskud = modregnResults.Map(x => x.UdnyttetFradrag);
			var ikkeUdnyttetUnderskud = modregnResults.Map(x => x.IkkeUdnyttetFradrag);

			// O til 1 overførsel af hele underskuddet
			indkomster[0].ModregnetUnderskudSkattepligtigIndkomst.ShouldEqual(-13195.73m);
			indkomster[1].ModregnetUnderskudSkattepligtigIndkomst.ShouldEqual(13195.73m);
			// Der er ingen underskud til fremførsel
			udnyttetUnderskud[0].ShouldEqual(7804.27m);
			ikkeUdnyttetUnderskud[0].ShouldEqual(0);
			udnyttetUnderskud[1].ShouldEqual(13195.73m); // Note: Dette er det overførte underskud
			ikkeUdnyttetUnderskud[1].ShouldEqual(0);

			skatterFoerPersonfradrag[0].Bundskat.ShouldEqual(0);
			skatterFoerPersonfradrag[0].Mellemskat.ShouldEqual(0);
			skatterFoerPersonfradrag[0].Topskat.ShouldEqual(0);
			skatterFoerPersonfradrag[0].AktieindkomstskatUnderGrundbeloebet.ShouldEqual(0);
			skatterFoerPersonfradrag[0].AktieindkomstskatOverGrundbeloebet.ShouldEqual(0);
			skatterFoerPersonfradrag[0].Sundhedsbidrag.ShouldEqual(0);
			skatterFoerPersonfradrag[0].KommunalIndkomstskatOgKirkeskat.ShouldEqual(0);

			skatterFoerPersonfradrag[1].Bundskat.ShouldEqual(13104);
			skatterFoerPersonfradrag[1].Mellemskat.ShouldEqual(0);
			skatterFoerPersonfradrag[1].Topskat.ShouldEqual(0);
			skatterFoerPersonfradrag[1].AktieindkomstskatUnderGrundbeloebet.ShouldEqual(0);
			skatterFoerPersonfradrag[1].AktieindkomstskatOverGrundbeloebet.ShouldEqual(0);
			skatterFoerPersonfradrag[1].Sundhedsbidrag.RoundMoney().ShouldEqual(18784.34m);
			skatterFoerPersonfradrag[1].KommunalIndkomstskatOgKirkeskat.RoundMoney().ShouldEqual(57033.96m);

			var personfradragBeregner = new PersonfradragBeregner();
			modregnResults = personfradragBeregner.ModregningAfPersonfradrag(skatterFoerPersonfradrag, kommunaleSatser);
			var skatterEfterPersonfradrag = modregnResults.Map(x => x.ModregnedeSkatter);

			skatterEfterPersonfradrag[0].Bundskat.ShouldEqual(0);
			skatterEfterPersonfradrag[0].Mellemskat.ShouldEqual(0);
			skatterEfterPersonfradrag[0].Topskat.ShouldEqual(0);
			skatterEfterPersonfradrag[0].AktieindkomstskatUnderGrundbeloebet.ShouldEqual(0);
			skatterEfterPersonfradrag[0].AktieindkomstskatOverGrundbeloebet.ShouldEqual(0);
			skatterEfterPersonfradrag[0].Sundhedsbidrag.ShouldEqual(0);
			skatterEfterPersonfradrag[0].KommunalIndkomstskatOgKirkeskat.ShouldEqual(0);

			skatterEfterPersonfradrag[1].Bundskat.ShouldEqual(8779.68m);
			skatterEfterPersonfradrag[1].Mellemskat.ShouldEqual(0);
			skatterEfterPersonfradrag[1].Topskat.ShouldEqual(0);
			skatterEfterPersonfradrag[1].AktieindkomstskatUnderGrundbeloebet.ShouldEqual(0);
			skatterEfterPersonfradrag[1].AktieindkomstskatOverGrundbeloebet.ShouldEqual(0);
			skatterEfterPersonfradrag[1].Sundhedsbidrag.RoundMoney().ShouldEqual(11920.34m);
			skatterEfterPersonfradrag[1].KommunalIndkomstskatOgKirkeskat.RoundMoney().ShouldEqual(36193.14m);
			skatterEfterPersonfradrag[1].Sum().RoundMoney().ShouldEqual(56893.16m);
		}

		//[Fact]
		//public void Eksempel_20_()
		//{
			
		//}

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