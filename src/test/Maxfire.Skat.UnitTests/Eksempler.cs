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
		public void Eksempel_9_IngenUnderskud_Gifte()
		{
			Constants.Brug2009Vaerdier();

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
		public void Eksempel_12_NegativSkattepligtigIndkomst_Ugift()
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
			
			// Skattepligtig indkomst før modregning
			indkomster[0].SkattepligtigIndkomst.ShouldEqual(-30000);

			// OBS: PersonligIndkomstUnderskudBeregner ville blive eksekveret her, hvis P.I. var negativ

			var skatterAfPersonligIndkomstBeregner = new SkatterAfPersonligIndkomstBeregner();
			var skatterAfPersonligIndkomst = skatterAfPersonligIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser);
			
			// Skatter før modregning af underskud og personfradrag
			skatterAfPersonligIndkomst[0].Bundskat.ShouldEqual(18144);
			skatterAfPersonligIndkomst[0].Mellemskat.ShouldEqual(768);
			skatterAfPersonligIndkomst[0].Topskat.ShouldEqual(1920);
			
			var underskudBeregner = new SkattepligtigIndkomstUnderskudBeregner();
			var modregnResults = underskudBeregner.ModregningAfUnderskud(indkomster, skatterAfPersonligIndkomst, kommunaleSatser);
			skatterAfPersonligIndkomst = modregnResults.Map(x => x.ModregnedeSkatter);

			// Skattepligtig indkomst efter modregning
			indkomster[0].SkattepligtigIndkomst.ShouldEqual(0);
			
			// Ingen fremførsel af underskud idet hele skatteværdien kan rummes i bundskatten
			var underskudTilFremfoersel = modregnResults.Map(x => x.UnderskudTilFremfoersel);
			underskudTilFremfoersel[0].ShouldEqual(0);
			indkomster[0].UnderskudSkattepligtigIndkomstTilFremfoersel.ShouldEqual(0);

			// Behøver ikke reberegne, idet ændringer i ægtefælles skattepligtig indkomst, bliver medtaget i denne beregning
			var skatterAfSkattepligtigIndkomstBeregner = new SkatterAfSkattepligtigIndkomstBeregner();
			var skatterAfSkattepligtigIndkomst = skatterAfSkattepligtigIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser);

			var skatterFoerPersonfradrag =
				new ValueTuple<Skatter>(new Skatter(skatterAfPersonligIndkomst[0], skatterAfSkattepligtigIndkomst[0]));

			// Skatter after modregning af underkud, men før modregning af personfradrag
			skatterFoerPersonfradrag[0].Bundskat.ShouldEqual(8457); // <--- modregning i bundskatten
			skatterFoerPersonfradrag[0].Mellemskat.ShouldEqual(768);
			skatterFoerPersonfradrag[0].Topskat.ShouldEqual(1920);

			var personfradragBeregner = new PersonfradragBeregner();
			var modregnPersonfradragResults = personfradragBeregner.ModregningAfPersonfradrag(skatterFoerPersonfradrag, kommunaleSatser);
			var skatterEfterPersonfradrag = modregnPersonfradragResults.Map(x => x.ModregnedeSkatter);

			skatterEfterPersonfradrag[0].ShouldEqual(Skatter.Nul);

			var tabtVardiAfPersonfradrag = modregnPersonfradragResults.Map(x => x.IkkeUdnyttetSkattevaerdi);
			var tabtPersonfradrag = modregnPersonfradragResults.Map(x => x.IkkeUdnyttetFradrag);

			tabtVardiAfPersonfradrag[0].ShouldEqual(4869.57m);
			tabtPersonfradrag[0].ShouldEqual(13044.66m);
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

			// Skattepligtig indkomst før modregning og fremførsel
			indkomster[0].SkattepligtigIndkomst.ShouldEqual(-59000);

			var skatAfPersonligIndkomstBeregner = new SkatterAfPersonligIndkomstBeregner();
			var skatterAfPersonligIndkomst = skatAfPersonligIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser);
			
			// Skatter før modregning af underskud og personfradrag er alle nul
			skatterAfPersonligIndkomst[0].ShouldEqual(SkatterAfPersonligIndkomst.Nul);
			
			var underskudBeregner = new SkattepligtigIndkomstUnderskudBeregner();
			var modregnResults = underskudBeregner.ModregningAfUnderskud(indkomster, skatterAfPersonligIndkomst, kommunaleSatser);
			skatterAfPersonligIndkomst = modregnResults.Map(x => x.ModregnedeSkatter);
			
			// Skatter efter modregning af underskud i bundskat, mellemskat og topskat er alle nul
			skatterAfPersonligIndkomst[0].ShouldEqual(SkatterAfPersonligIndkomst.Nul);

			// Skattepligtig indkomst efter modregning og fremførsel
			indkomster[0].SkattepligtigIndkomst.ShouldEqual(0);
			indkomster[0].UnderskudSkattepligtigIndkomstTilFremfoersel.ShouldEqual(59000);

			// Hele årets underskud i S.I fremføres
			var underskudTilFremfoersel = modregnResults.Map(x => x.UnderskudTilFremfoersel);
			underskudTilFremfoersel[0].ShouldEqual(59000);

			var skatAfSkattepligtigIndkomstBeregner = new SkatterAfSkattepligtigIndkomstBeregner();
			var skatterAfSkattepligtigIndkomst = skatAfSkattepligtigIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser);
			
			// Kommuneskat, kirkeskat og sundhedsbidrag er alle nul
			skatterAfSkattepligtigIndkomst[0].ShouldEqual(SkatterAfSkattepligtigIndkomst.Nul);
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

			// Skattepligtig indkomst før modregning og fremførsel
			indkomster[0].SkattepligtigIndkomst.ShouldEqual(-90000);

			var skatterAfPersonligIndkomstBeregner = new SkatterAfPersonligIndkomstBeregner();
			var skatterAfPersonligIndkomst = skatterAfPersonligIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser);

			// Skatter før modregning af underskud og personfradrag
			skatterAfPersonligIndkomst[0].Bundskat.ShouldEqual(18144);
			skatterAfPersonligIndkomst[0].Mellemskat.ShouldEqual(768);
			skatterAfPersonligIndkomst[0].Topskat.ShouldEqual(1920);
			
			var underskudBeregner = new SkattepligtigIndkomstUnderskudBeregner();
			var modregnSkatteplitigIndkomstResults = underskudBeregner.ModregningAfUnderskud(indkomster, skatterAfPersonligIndkomst, kommunaleSatser);
			var underskudTilFremfoersel = modregnSkatteplitigIndkomstResults.Map(x => x.UnderskudTilFremfoersel);
			var modregningUnderskud = modregnSkatteplitigIndkomstResults.Map(x => x.ModregningUnderskud);
			
			// Skattepligtig indkomst efter modregning og fremførsel
			indkomster[0].SkattepligtigIndkomst.ShouldEqual(0);
			indkomster[0].UnderskudSkattepligtigIndkomstTilFremfoersel.ShouldEqual(25484.67m);
			underskudTilFremfoersel[0].ShouldEqual(25484.67m);
			modregningUnderskud[0].ShouldEqual(64515.33m);
			
			// Dette er skatter efter modregning af underskud i skattepligtig indkomst
			skatterAfPersonligIndkomst = modregnSkatteplitigIndkomstResults.Map(x => x.ModregnedeSkatter);

			var skatAfSkattepligtigIndkomstBeregner = new SkatterAfSkattepligtigIndkomstBeregner();
			var skatterAfSkattepligtigIndkomst = skatAfSkattepligtigIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser);

			var skatterFoerPersonfradrag = new Skatter(skatterAfPersonligIndkomst[0], skatterAfSkattepligtigIndkomst[0]).ToTuple();

			skatterFoerPersonfradrag[0].ShouldEqual(Skatter.Nul);

			var personfradragBeregner = new PersonfradragBeregner();
			var modregnPersonfradragResults = personfradragBeregner.ModregningAfPersonfradrag(skatterFoerPersonfradrag, kommunaleSatser);
			var skatterEfterPersonfradrag = modregnPersonfradragResults.Map(x => x.ModregnedeSkatter);
			var tabtPersonfradrag = modregnPersonfradragResults.Map(x => x.IkkeUdnyttetFradrag);

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

			// Skattepligtig indkomst før modregning og fremførsel af underskud
			indkomster[0].SkattepligtigIndkomst.ShouldEqual(130000);
			indkomster[0].FremfoertUnderskudSkattepligtigIndkomst.ShouldEqual(12000);

			var skatterAfPersonligIndkomstBeregner = new SkatterAfPersonligIndkomstBeregner();
			var skatterAfPersonligIndkomst = skatterAfPersonligIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser);

			var underskudBeregner = new SkattepligtigIndkomstUnderskudBeregner();
			var modregnResults = underskudBeregner.ModregningAfUnderskud(indkomster, skatterAfPersonligIndkomst, kommunaleSatser);
			var underskudTilFremfoersel = modregnResults.Map(x => x.UnderskudTilFremfoersel);
			var modregningUnderskud = modregnResults.Map(x => x.ModregningUnderskud);
			var modregningSkatter = modregnResults.Map(x => x.ModregningSkatter);
			
			// Skattepligtig indkomst efter modregning og fremførsel af underskud
			indkomster[0].SkattepligtigIndkomst.ShouldEqual(118000);
			indkomster[0].FremfoertUnderskudSkattepligtigIndkomst.ShouldEqual(0); // <----- Fejler her!!!!
			modregningUnderskud[0].ShouldEqual(12000);
			underskudTilFremfoersel[0].ShouldEqual(0);
			indkomster[0].UnderskudSkattepligtigIndkomstTilFremfoersel.ShouldEqual(0);
			indkomster[0].ModregnetUnderskudSkattepligtigIndkomst.ShouldEqual(12000);
			// Hele modregningen sker i indkomst
			modregningSkatter[0].ShouldEqual(SkatterAfPersonligIndkomst.Nul);
			
			var skatterAfSkattepligtigIndkomstBeregner = new SkatterAfSkattepligtigIndkomstBeregner();
			var skatterAfSkattepligtigIndkomst = skatterAfSkattepligtigIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser);

			var skatterFoerPersonfradrag = new Skatter(skatterAfPersonligIndkomst[0], skatterAfSkattepligtigIndkomst[0]).ToTuple();

			skatterFoerPersonfradrag[0].Bundskat.ShouldEqual(8568);
			skatterFoerPersonfradrag[0].Mellemskat.ShouldEqual(0);
			skatterFoerPersonfradrag[0].Topskat.ShouldEqual(0);
			skatterFoerPersonfradrag[0].Sundhedsbidrag.ShouldEqual(9440);
			skatterFoerPersonfradrag[0].KommunalIndkomstskatOgKirkeskat.ShouldEqual(28662.20m);

			var personfradragBeregner = new PersonfradragBeregner();
			var modregnPersonfradragResults = personfradragBeregner.ModregningAfPersonfradrag(skatterFoerPersonfradrag, kommunaleSatser);
			var skatterEfterPersonfradrag = modregnPersonfradragResults.Map(x => x.ModregnedeSkatter);

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

			// Skattepligtig indkomst før modregning og fremførsel af underskud
			indkomster[0].SkattepligtigIndkomst.ShouldEqual(300000);
			indkomster[0].FremfoertUnderskudSkattepligtigIndkomst.ShouldEqual(500000);

			var skatterAfPersonligIndkomstBeregner = new SkatterAfPersonligIndkomstBeregner();
			var skatterAfPersonligIndkomst = skatterAfPersonligIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser);

			// Skatter før modregning af underskud og personfradrag
			skatterAfPersonligIndkomst[0].Bundskat.ShouldEqual(18144);
			skatterAfPersonligIndkomst[0].Mellemskat.ShouldEqual(768);
			skatterAfPersonligIndkomst[0].Topskat.ShouldEqual(1920);
			skatterAfPersonligIndkomst[0].Sum().ShouldEqual(20832);

			var underskudBeregner = new SkattepligtigIndkomstUnderskudBeregner();
			var modregnSkattepligtigIndkomstUnderskudResults 
				= underskudBeregner.ModregningAfUnderskud(indkomster, skatterAfPersonligIndkomst, kommunaleSatser);
			skatterAfPersonligIndkomst = modregnSkattepligtigIndkomstUnderskudResults.Map(x => x.ModregnedeSkatter);
			var underskudTilFremfoersel = modregnSkattepligtigIndkomstUnderskudResults.Map(x => x.UnderskudTilFremfoersel);
			var modregningUnderskud = modregnSkattepligtigIndkomstUnderskudResults.Map(x => x.ModregningUnderskud);
			var modregningSkatter = modregnSkattepligtigIndkomstUnderskudResults.Map(x => x.ModregningSkatter);
			var modregningUnderskudSkatter = modregnSkattepligtigIndkomstUnderskudResults.Map(x => x.ModregningUnderskudSkatter);
			var modregningSkattepligtigIndkomst = modregnSkattepligtigIndkomstUnderskudResults.Map(x => x.ModregningUnderskudSkattepligtigIndkomst);

			// Skattepligtig indkomst efter modregning og fremførsel af underskud
			indkomster[0].SkattepligtigIndkomst.ShouldEqual(0);
			indkomster[0].FremfoertUnderskudSkattepligtigIndkomst.ShouldEqual(0);
			skatterAfPersonligIndkomst[0].ShouldEqual(SkatterAfPersonligIndkomst.Nul);
			// Og dette fremføres til efterfølgende skatteår
			indkomster[0].UnderskudSkattepligtigIndkomstTilFremfoersel.ShouldEqual(135484.67m);
			underskudTilFremfoersel[0].ShouldEqual(135484.67m);
			modregningSkatter[0].Sum().ShouldEqual(20832); // svarende til et underskud på 64515.33m
			modregningUnderskudSkatter[0].ShouldEqual(64515.33m);
			modregningSkattepligtigIndkomst[0].ShouldEqual(300000);
			modregningUnderskud[0].ShouldEqual(300000 + 64515.33m);
			
			var skatAfSkattepligtigIndkomstBeregner = new SkatterAfSkattepligtigIndkomstBeregner();
			var skatterAfSkattepligtigIndkomst = skatAfSkattepligtigIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser);

			skatterAfSkattepligtigIndkomst[0].Sundhedsbidrag.ShouldEqual(0);
			skatterAfSkattepligtigIndkomst[0].KommunalIndkomstskatOgKirkeskat.ShouldEqual(0);

			var skatterFoerPersonfradrag =
				new Skatter(skatterAfPersonligIndkomst[0], skatterAfSkattepligtigIndkomst[0]).ToTuple();

			var personfradragBeregner = new PersonfradragBeregner();
			var modregnPersonfradragResults = personfradragBeregner.ModregningAfPersonfradrag(skatterFoerPersonfradrag, kommunaleSatser);
			var skatterEfterPersonfradrag = modregnPersonfradragResults.Map(x => x.ModregnedeSkatter);
			var tabtPersonfradrag = modregnPersonfradragResults.Map(x => x.IkkeUdnyttetFradrag);

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

			// Skattepligtig indkomst inden modregning og fremførsel
			indkomster[0].SkattepligtigIndkomst.ShouldEqual(-60000);
			indkomster[0].FremfoertUnderskudSkattepligtigIndkomst.ShouldEqual(210000);

			var skatAfPersonligIndkomstBeregner = new SkatterAfPersonligIndkomstBeregner();
			var skatterAfPersonligIndkomst = skatAfPersonligIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser);

			// Skatter før modregning af underskud og personfradrag
			skatterAfPersonligIndkomst[0].Bundskat.ShouldEqual(1008);
			skatterAfPersonligIndkomst[0].Mellemskat.ShouldEqual(0);
			skatterAfPersonligIndkomst[0].Topskat.ShouldEqual(0);
			skatterAfPersonligIndkomst[0].AktieindkomstskatUnderGrundbeloebet.ShouldEqual(13524);
			skatterAfPersonligIndkomst[0].AktieindkomstskatOverGrundbeloebet.ShouldEqual(24854 + 42255);

			var underskudBeregner = new SkattepligtigIndkomstUnderskudBeregner();
			var modregnResults = underskudBeregner.ModregningAfUnderskud(indkomster, skatterAfPersonligIndkomst, kommunaleSatser);
			var skatterAfPersonligIndkomstEfterModregningAfUnderskud = modregnResults.Map(x => x.ModregnedeSkatter);
			var underskudTilFremfoersel = modregnResults.Map(x => x.UnderskudTilFremfoersel);

			// Skattepligtig indkomst efter modregning og fremførsel
			indkomster[0].SkattepligtigIndkomst.ShouldEqual(0);
			indkomster[0].FremfoertUnderskudSkattepligtigIndkomst.ShouldEqual(0);

			// Dette fremføres til efterfølgende skatteår
			indkomster[0].UnderskudSkattepligtigIndkomstTilFremfoersel.ShouldEqual(59046.14m);
			underskudTilFremfoersel[0].ShouldEqual(59046.14m);
			
			var skatAfSkattepligtigIndkomstBeregner = new SkatterAfSkattepligtigIndkomstBeregner();
			var skatterAfSkattepligtigIndkomst = skatAfSkattepligtigIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser);

			var skatterFoerPersonfradrag 
				= new Skatter(skatterAfPersonligIndkomstEfterModregningAfUnderskud[0], skatterAfSkattepligtigIndkomst[0]).ToTuple();

			skatterFoerPersonfradrag[0].Bundskat.ShouldEqual(0);
			skatterFoerPersonfradrag[0].Mellemskat.ShouldEqual(0);
			skatterFoerPersonfradrag[0].Topskat.ShouldEqual(0);
			skatterFoerPersonfradrag[0].Sundhedsbidrag.ShouldEqual(0);
			skatterFoerPersonfradrag[0].KommunalIndkomstskatOgKirkeskat.ShouldEqual(0);
			skatterFoerPersonfradrag[0].AktieindkomstskatUnderGrundbeloebet.ShouldEqual(13524);
			skatterFoerPersonfradrag[0].AktieindkomstskatOverGrundbeloebet.ShouldEqual(0);

			var personfradragBeregner = new PersonfradragBeregner();
			var modregnResults2 = personfradragBeregner.ModregningAfPersonfradrag(skatterFoerPersonfradrag, kommunaleSatser);
			var skatterEfterPersonfradrag = modregnResults2.Map(x => x.ModregnedeSkatter);
			var tabtPersonfradrag = modregnResults2.Map(x => x.IkkeUdnyttetFradrag);

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

			// Skattepligtig indkomst før modregning og fremførsel af underskud
			indkomster[0].FremfoertUnderskudSkattepligtigIndkomst.ShouldEqual(0);
			indkomster[0].SkattepligtigIndkomst.ShouldEqual(-53000);
			indkomster[1].FremfoertUnderskudSkattepligtigIndkomst.ShouldEqual(0);
			indkomster[1].SkattepligtigIndkomst.ShouldEqual(252000);

			var skatterAfPersonligIndkomstBeregner = new SkatterAfPersonligIndkomstBeregner();
			var skatterAfPersonligIndkomst = skatterAfPersonligIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser);

			// Æ1 har ingen egne skatter at modregne underskud i
			skatterAfPersonligIndkomst[0].ShouldEqual(SkatterAfPersonligIndkomst.Nul);
			
			// Og hele underskuddet overføres til Æ2, hvor det kan indeholdes i Æ2s skattepligtige indkomst
			var underskudBeregner = new SkattepligtigIndkomstUnderskudBeregner();
			var modregnResults = underskudBeregner.ModregningAfUnderskud(indkomster, skatterAfPersonligIndkomst, kommunaleSatser);
			var skatterAfPersonligIndkomstEfterModregningAfUnderskud = modregnResults.Map(x => x.ModregnedeSkatter);
			var underskudTilFremfoersel = modregnResults.Map(x => x.UnderskudTilFremfoersel);

			// Skattepligtig indkomst efter modregning og fremførsel af underskud
			indkomster[0].FremfoertUnderskudSkattepligtigIndkomst.ShouldEqual(0);
			indkomster[0].SkattepligtigIndkomst.ShouldEqual(0);
			indkomster[0].ModregnetUnderskudSkattepligtigIndkomst.ShouldEqual(-53000); // Overført fra
			indkomster[1].FremfoertUnderskudSkattepligtigIndkomst.ShouldEqual(0);
			indkomster[1].ModregnetUnderskudSkattepligtigIndkomst.ShouldEqual(53000); // Overført og udnyttet hos
			indkomster[1].SkattepligtigIndkomst.ShouldEqual(199000);

			// O til 1 overførsel af hele underskuddet
			// Der er ingen underskud til fremførsel
			// BUG: Den udnyttede overførsel fra ægtefælle skal justeres i basis underskud, sådan at kun tilbageførte underskud fremføres
			underskudTilFremfoersel[0].ShouldEqual(53000); 
			underskudTilFremfoersel[1].ShouldEqual(-53000); // BUG: Det overførte og udnyttede underskud regnes som uudnyttet hos Æ1

			var skatterAfSkattepligtigIndkomstBeregner = new SkatterAfSkattepligtigIndkomstBeregner();
			var skatterAfSkattepligtigIndkomst = skatterAfSkattepligtigIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser);

			var skatterFoerPersonfradrag = new ValueTuple<Skatter>(
				new Skatter(skatterAfPersonligIndkomstEfterModregningAfUnderskud[0], skatterAfSkattepligtigIndkomst[0]),
				new Skatter(skatterAfPersonligIndkomstEfterModregningAfUnderskud[1], skatterAfSkattepligtigIndkomst[1]));

			skatterFoerPersonfradrag[0].ShouldEqual(Skatter.Nul);

			skatterFoerPersonfradrag[1].Bundskat.ShouldEqual(13104);
			skatterFoerPersonfradrag[1].Mellemskat.ShouldEqual(0);
			skatterFoerPersonfradrag[1].Topskat.ShouldEqual(0);
			skatterFoerPersonfradrag[1].AktieindkomstskatUnderGrundbeloebet.ShouldEqual(0);
			skatterFoerPersonfradrag[1].AktieindkomstskatOverGrundbeloebet.ShouldEqual(0);
			skatterFoerPersonfradrag[1].Sundhedsbidrag.ShouldEqual(15920);
			skatterFoerPersonfradrag[1].KommunalIndkomstskatOgKirkeskat.ShouldEqual(48337.10m);

			var personfradragBeregner = new PersonfradragBeregner();
			var modregnResults2 = personfradragBeregner.ModregningAfPersonfradrag(skatterFoerPersonfradrag, kommunaleSatser);
			var skatterEfterPersonfradrag = modregnResults2.Map(x => x.ModregnedeSkatter);

			skatterEfterPersonfradrag[0].ShouldEqual(Skatter.Nul);

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

			// Skattepligtig indkomst før modregning og fremførsel af underskud
			indkomster[0].FremfoertUnderskudSkattepligtigIndkomst.ShouldEqual(0);
			indkomster[0].SkattepligtigIndkomst.ShouldEqual(-21000);
			indkomster[1].FremfoertUnderskudSkattepligtigIndkomst.ShouldEqual(0);
			indkomster[1].SkattepligtigIndkomst.ShouldEqual(248000);

			var skatterAfPersonligIndkomstBeregner = new SkatterAfPersonligIndkomstBeregner();
			var skatterAfPersonligIndkomst = skatterAfPersonligIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser);

			// Æ1 har egne skatter at modregne underskud i
			skatterAfPersonligIndkomst[0].Bundskat.ShouldEqual(2520);
			skatterAfPersonligIndkomst[0].Mellemskat.ShouldEqual(0);
			skatterAfPersonligIndkomst[0].Topskat.ShouldEqual(0);
			skatterAfPersonligIndkomst[0].AktieindkomstskatOverGrundbeloebet.ShouldEqual(0);

			var underskudBeregner = new SkattepligtigIndkomstUnderskudBeregner();
			var modregnResults = underskudBeregner.ModregningAfUnderskud(indkomster, skatterAfPersonligIndkomst, kommunaleSatser);
			var skatterAfPersonligIndkomstEfterModregningAfUnderskud = modregnResults.Map(x => x.ModregnedeSkatter);
			
			// Skattepligtig indkomst efter modregning og fremførsel af underskud
			indkomster[0].SkattepligtigIndkomst.ShouldEqual(0);
			indkomster[0].UnderskudSkattepligtigIndkomstTilFremfoersel.ShouldEqual(0);
			indkomster[0].ModregnetUnderskudSkattepligtigIndkomst.ShouldEqual(-21000); // ikke muligt at dekomponere i modregning i hhv indkomst og skatter
			indkomster[1].SkattepligtigIndkomst.ShouldEqual(248000 - 13195.73m);
			indkomster[1].UnderskudSkattepligtigIndkomstTilFremfoersel.ShouldEqual(0);
			indkomster[1].ModregnetUnderskudSkattepligtigIndkomst.ShouldEqual(13195.73m);

			var modregningUnderskudSkattepligtigIndkomst = modregnResults.Map(x => x.ModregningUnderskudSkattepligtigIndkomst);
			var modregningUnderskudSkatter = modregnResults.Map(x => x.ModregningUnderskudSkatter);
			var modregningUnderskud = modregnResults.Map(x => x.ModregningUnderskud);
			var underskudTilFremfoersel = modregnResults.Map(x => x.UnderskudTilFremfoersel);

			// Selvom en del af modregningen sker i egne skatter (7804.27) og den resterende del 
			// i ægtefælles indkomst, så er modregningen af underskud opgjort hos kilden
			modregningUnderskudSkattepligtigIndkomst[0].ShouldEqual(13195.73m);
			modregningUnderskudSkatter[0].ShouldEqual(7804.27m);
			modregningUnderskud[0].ShouldEqual(21000);
			underskudTilFremfoersel[0].ShouldEqual(0);
			modregningUnderskudSkattepligtigIndkomst[1].ShouldEqual(0);
			modregningUnderskudSkatter[1].ShouldEqual(0);
			modregningUnderskud[1].ShouldEqual(0);
			underskudTilFremfoersel[1].ShouldEqual(0);

			var skatAfSkattepligtigIndkomstBeregner = new SkatterAfSkattepligtigIndkomstBeregner();
			var skatterAfSkattepligtigIndkomst = skatAfSkattepligtigIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser);

			var skatterFoerPersonfradrag = new ValueTuple<Skatter>(
				new Skatter(skatterAfPersonligIndkomstEfterModregningAfUnderskud[0], skatterAfSkattepligtigIndkomst[0]),
				new Skatter(skatterAfPersonligIndkomstEfterModregningAfUnderskud[1], skatterAfSkattepligtigIndkomst[1]));

			skatterFoerPersonfradrag[0].ShouldEqual(Skatter.Nul);

			skatterFoerPersonfradrag[1].Bundskat.ShouldEqual(13104);
			skatterFoerPersonfradrag[1].Mellemskat.ShouldEqual(0);
			skatterFoerPersonfradrag[1].Topskat.ShouldEqual(0);
			skatterFoerPersonfradrag[1].AktieindkomstskatUnderGrundbeloebet.ShouldEqual(0);
			skatterFoerPersonfradrag[1].AktieindkomstskatOverGrundbeloebet.ShouldEqual(0);
			skatterFoerPersonfradrag[1].Sundhedsbidrag.RoundMoney().ShouldEqual(18784.34m);
			skatterFoerPersonfradrag[1].KommunalIndkomstskatOgKirkeskat.RoundMoney().ShouldEqual(57033.96m);

			var personfradragBeregner = new PersonfradragBeregner();
			var modregnResults2 = personfradragBeregner.ModregningAfPersonfradrag(skatterFoerPersonfradrag, kommunaleSatser);
			var skatterEfterPersonfradrag = modregnResults2.Map(x => x.ModregnedeSkatter);

			skatterEfterPersonfradrag[0].ShouldEqual(Skatter.Nul);

			skatterEfterPersonfradrag[1].Bundskat.ShouldEqual(8779.68m);
			skatterEfterPersonfradrag[1].Mellemskat.ShouldEqual(0);
			skatterEfterPersonfradrag[1].Topskat.ShouldEqual(0);
			skatterEfterPersonfradrag[1].AktieindkomstskatUnderGrundbeloebet.ShouldEqual(0);
			skatterEfterPersonfradrag[1].AktieindkomstskatOverGrundbeloebet.ShouldEqual(0);
			skatterEfterPersonfradrag[1].Sundhedsbidrag.RoundMoney().ShouldEqual(11920.34m);
			skatterEfterPersonfradrag[1].KommunalIndkomstskatOgKirkeskat.RoundMoney().ShouldEqual(36193.14m);
			skatterEfterPersonfradrag[1].Sum().RoundMoney().ShouldEqual(56893.16m);
		}

		[Fact]
		public void Eksempel_20_ModregningPartnersIndkomstOgSkat()
		{
			Constants.Brug2009Vaerdier();

			var indkomster = new ValueTuple<PersonligeBeloeb>(
				new PersonligeBeloeb
				{
					NettoKapitalIndkomst = -20000,
					LigningsmaessigeFradrag = 5000
				},
				new PersonligeBeloeb
				{
					PersonligIndkomst = 160000,
					NettoKapitalIndkomst = -128000,
					LigningsmaessigeFradrag = 20000
				});

			var kommunaleSatser = getKommunaleSatserForGifte();

			// Skattepligtig indkomst før modregning af underskud mellem ægtefæller
			indkomster[0].FremfoertUnderskudSkattepligtigIndkomst.ShouldEqual(0);
			indkomster[0].SkattepligtigIndkomst.ShouldEqual(-25000);
			indkomster[1].FremfoertUnderskudSkattepligtigIndkomst.ShouldEqual(0);
			indkomster[1].SkattepligtigIndkomst.ShouldEqual(12000);

			var skatAfPersonligIndkomstBeregner = new SkatterAfPersonligIndkomstBeregner();
			var skatterAfPersonligIndkomst = skatAfPersonligIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser);
			
			// Æ1 har ingen egne skatter at modregne underskud i
			skatterAfPersonligIndkomst[0].ShouldEqual(SkatterAfPersonligIndkomst.Nul);

			var underskudBeregner = new SkattepligtigIndkomstUnderskudBeregner();
			var modregnResults = underskudBeregner.ModregningAfUnderskud(indkomster, skatterAfPersonligIndkomst, kommunaleSatser);
			var skatterAfPersonligIndkomstEfterModregningAfUnderskud = modregnResults.Map(x => x.ModregnedeSkatter);
			var modregningUnderskudSkattepligtigIndkomst = modregnResults.Map(x => x.ModregningUnderskudSkattepligtigIndkomst);
			var modregningUnderskudSkatter = modregnResults.Map(x => x.ModregningUnderskudSkatter);
			var modregningUnderskud = modregnResults.Map(x => x.ModregningUnderskud);
			var underskudTilFremfoersel = modregnResults.Map(x => x.UnderskudTilFremfoersel);

			// Skattepligtig indkomst efter modregning og fremførsel af underskud
			indkomster[0].SkattepligtigIndkomst.ShouldEqual(0);
			indkomster[0].UnderskudSkattepligtigIndkomstTilFremfoersel.ShouldEqual(0);
			indkomster[0].ModregnetUnderskudSkattepligtigIndkomst.ShouldEqual(-25000);
			indkomster[1].SkattepligtigIndkomst.ShouldEqual(0);
			indkomster[1].UnderskudSkattepligtigIndkomstTilFremfoersel.ShouldEqual(0);
			indkomster[1].ModregnetUnderskudSkattepligtigIndkomst.ShouldEqual(12000); 

			// Der er ingen underskud til fremførsel
			modregningUnderskudSkattepligtigIndkomst[0].ShouldEqual(12000);
			modregningUnderskudSkatter[0].ShouldEqual(13000);
			modregningUnderskud[0].ShouldEqual(25000);
			underskudTilFremfoersel[0].ShouldEqual(0);
			modregningUnderskudSkattepligtigIndkomst[1].ShouldEqual(0);
			modregningUnderskudSkatter[1].ShouldEqual(0);
			modregningUnderskud[1].ShouldEqual(0);
			underskudTilFremfoersel[1].ShouldEqual(0);

			var skatAfSkattepligtigIndkomstBeregner = new SkatterAfSkattepligtigIndkomstBeregner();
			var skatterAfSkattepligtigIndkomst = skatAfSkattepligtigIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser);

			var skatterFoerPersonfradrag = new ValueTuple<Skatter>(
				new Skatter(skatterAfPersonligIndkomstEfterModregningAfUnderskud[0], skatterAfSkattepligtigIndkomst[0]),
				new Skatter(skatterAfPersonligIndkomstEfterModregningAfUnderskud[1], skatterAfSkattepligtigIndkomst[1]));

			skatterFoerPersonfradrag[0].ShouldEqual(Skatter.Nul);

			skatterFoerPersonfradrag[1].Bundskat.ShouldEqual(3866.30m);
			skatterFoerPersonfradrag[1].Mellemskat.ShouldEqual(0);
			skatterFoerPersonfradrag[1].Topskat.ShouldEqual(0);
			skatterFoerPersonfradrag[1].AktieindkomstskatUnderGrundbeloebet.ShouldEqual(0);
			skatterFoerPersonfradrag[1].AktieindkomstskatOverGrundbeloebet.ShouldEqual(0);
			skatterFoerPersonfradrag[1].Sundhedsbidrag.ShouldEqual(0);
			skatterFoerPersonfradrag[1].KommunalIndkomstskatOgKirkeskat.ShouldEqual(0);

			var personfradragBeregner = new PersonfradragBeregner();
			var modregnResults2 = personfradragBeregner.ModregningAfPersonfradrag(skatterFoerPersonfradrag, kommunaleSatser);
			var skatterEfterPersonfradrag = modregnResults2.Map(x => x.ModregnedeSkatter);

			skatterEfterPersonfradrag[0].ShouldEqual(Skatter.Nul);
			skatterEfterPersonfradrag[1].ShouldEqual(Skatter.Nul);
		}

		[Fact]
		public void Eksempel_21_ModregningEgenSkatOgPartnersIndkomstOgSkat()
		{
			Constants.Brug2009Vaerdier();

			var indkomster = new ValueTuple<PersonligeBeloeb>(
				new PersonligeBeloeb
				{
					PersonligIndkomst = 110000,
					NettoKapitalIndkomst = -200000,
					LigningsmaessigeFradrag = 4000
				},
				new PersonligeBeloeb
				{
					PersonligIndkomst = 375000,
					NettoKapitalIndkomst = -320000,
					LigningsmaessigeFradrag = 31000
				});

			var kommunaleSatser = getKommunaleSatserForGifte();

			// Skattepligtig indkomst før modregning af underskud mellem ægtefæller
			indkomster[0].SkattepligtigIndkomst.ShouldEqual(-94000);
			indkomster[1].SkattepligtigIndkomst.ShouldEqual(24000);

			var skatAfPersonligIndkomstBeregner = new SkatterAfPersonligIndkomstBeregner();
			var skatterAfPersonligIndkomst = skatAfPersonligIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser);

			// Æ1 har egne skatter at modregne i
			skatterAfPersonligIndkomst[0].Bundskat.ShouldEqual(5544);
			skatterAfPersonligIndkomst[0].Mellemskat.ShouldEqual(0);
			skatterAfPersonligIndkomst[0].Topskat.ShouldEqual(0);
			skatterAfPersonligIndkomst[0].AktieindkomstskatOverGrundbeloebet.ShouldEqual(0);

			skatterAfPersonligIndkomst[1].Bundskat.ShouldEqual(18900);
			skatterAfPersonligIndkomst[1].Mellemskat.ShouldEqual(0);
			skatterAfPersonligIndkomst[1].Topskat.ShouldEqual(4170);
			skatterAfPersonligIndkomst[1].AktieindkomstskatOverGrundbeloebet.ShouldEqual(0);

			var underskudBeregner = new SkattepligtigIndkomstUnderskudBeregner();
			var modregnResults = underskudBeregner.ModregningAfUnderskud(indkomster, skatterAfPersonligIndkomst, kommunaleSatser);
			var skatterAfPersonligIndkomstEfterModregningAfUnderskud = modregnResults.Map(x => x.ModregnedeSkatter);
			var modregningUnderskudSkattepligtigIndkomst = modregnResults.Map(x => x.ModregningUnderskudSkattepligtigIndkomst);
			var modregningUnderskudSkatter = modregnResults.Map(x => x.ModregningUnderskudSkatter);
			var modregningUnderskud = modregnResults.Map(x => x.ModregningUnderskud);
			var underskudTilFremfoersel = modregnResults.Map(x => x.UnderskudTilFremfoersel);

			// Skattepligtig indkomst efter modregning af underskud mellem ægtefæller
			indkomster[0].SkattepligtigIndkomst.ShouldEqual(0);
			indkomster[0].ModregnetUnderskudSkattepligtigIndkomst.ShouldEqual(-25000);
			indkomster[0].UnderskudSkattepligtigIndkomstTilFremfoersel.ShouldEqual(0);
			indkomster[1].SkattepligtigIndkomst.ShouldEqual(0);
			indkomster[1].ModregnetUnderskudSkattepligtigIndkomst.ShouldEqual(12000); // de resterende 13000 modregnes i skatter
			indkomster[1].UnderskudSkattepligtigIndkomstTilFremfoersel.ShouldEqual(0);
			
			// Der er ingen underskud til fremførsel
			//underskudTilFremfoersel[0].ShouldEqual(94000 - 17169.40m); // Note: Restunderskuddet som Æ1 ikke selv kan rumme i sine skatter
			
			//underskudTilFremfoersel[1].ShouldEqual(-(94000 - 17169.40m)); // BUG: Ægtefæller udnytter underskuddet, og restunderskud positivt for Æ1

			var skatAfSkattepligtigIndkomstBeregner = new SkatterAfSkattepligtigIndkomstBeregner();
			var skatterAfSkattepligtigIndkomst = skatAfSkattepligtigIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser);

			var skatterFoerPersonfradrag = new ValueTuple<Skatter>(
				new Skatter(skatterAfPersonligIndkomstEfterModregningAfUnderskud[0], skatterAfSkattepligtigIndkomst[0]),
				new Skatter(skatterAfPersonligIndkomstEfterModregningAfUnderskud[1], skatterAfSkattepligtigIndkomst[1]));

			skatterFoerPersonfradrag[0].ShouldEqual(Skatter.Nul);

			skatterFoerPersonfradrag[1].Bundskat.ShouldEqual(1841);
			skatterFoerPersonfradrag[1].Mellemskat.ShouldEqual(0);
			skatterFoerPersonfradrag[1].Topskat.ShouldEqual(4170);
			skatterFoerPersonfradrag[1].AktieindkomstskatUnderGrundbeloebet.ShouldEqual(0);
			skatterFoerPersonfradrag[1].AktieindkomstskatOverGrundbeloebet.ShouldEqual(0);
			skatterFoerPersonfradrag[1].Sundhedsbidrag.ShouldEqual(0);
			skatterFoerPersonfradrag[1].KommunalIndkomstskatOgKirkeskat.ShouldEqual(0);

			var personfradragBeregner = new PersonfradragBeregner();
			var modregnResults2 = personfradragBeregner.ModregningAfPersonfradrag(skatterFoerPersonfradrag, kommunaleSatser);
			var skatterEfterPersonfradrag = modregnResults2.Map(x => x.ModregnedeSkatter);

			skatterEfterPersonfradrag[0].ShouldEqual(Skatter.Nul);
			skatterEfterPersonfradrag[1].ShouldEqual(Skatter.Nul);
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

	public class SkatBeregner
	{
		public ValueTuple<Skatter> BeregnSkat(ValueTuple<PersonligeBeloeb> indkomster, ValueTuple<KommunaleSatser> kommunaleSatser)
		{
			var skatAfPersonligIndkomstBeregner = new SkatterAfPersonligIndkomstBeregner();
			var skatterAfPersonligIndkomst = skatAfPersonligIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser);

			var skatAfSkattepligtigIndkomstBeregner = new SkatterAfSkattepligtigIndkomstBeregner();
			var skatterAfSkattepligtigIndkomst = skatAfSkattepligtigIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser);

			return skatterAfPersonligIndkomst.Map(index =>
				new Skatter(skatterAfPersonligIndkomst[index], skatterAfSkattepligtigIndkomst[index]));
		}
	}
}