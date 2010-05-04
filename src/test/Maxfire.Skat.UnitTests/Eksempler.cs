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
			var udnyttetBundfradrag = mellemskatGrundlagBeregner.BeregnUdnyttetBundfradrag(indkomster);
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
			
		}

		[Fact]
		public void Eksempel_6_IngenUnderskud_Ugift()
		{
			Constants.Brug2009Vaerdier();

			// Note: Indkomstopgørelsen mangler

			var kommunaleSatser = new ValueTuple<KommunaleSatser>(
				new KommunaleSatser
					{
						Kommuneskattesats = 0.237m,
						Kirkeskattesats = 0.0059m
				});

			var indkomster = new ValueTuple<PersonligeBeloeb>(
				new PersonligeBeloeb
					{
						PersonligIndkomst = 358395,
						NettoKapitalIndkomst = 8500,
						LigningsmaesigeFradrag = 24600,
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
			var personFradragBeregner = new PersonFradragBeregner();
			var skattevaerdier = personFradragBeregner.BeregnSkattevaerdier(kommunaleSatser);

			skattevaerdier[0].Bundskat.ShouldEqual(2162.16m);
			skattevaerdier[0].Sundhedsbidrag.ShouldEqual(3432);
			skattevaerdier[0].KommunalIndkomstskatOgKirkeskat.ShouldEqual(10420.41m);
		}

		[Fact] 
		public void Eksempel_9()
		{
			Constants.Brug2009Vaerdier();

			// Note: Indkomstopgørelsen mangler

			var kommunaleSatser = new ValueTuple<KommunaleSatser>(
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

			var indkomster = new ValueTuple<PersonligeBeloeb>(
				new PersonligeBeloeb
					{
						PersonligIndkomst = 388000,
						NettoKapitalIndkomst = 13500,
						LigningsmaesigeFradrag = 21600,
						KapitalPensionsindskud = 46000
					},
				new PersonligeBeloeb
					{
						PersonligIndkomst = 150000,
						NettoKapitalIndkomst = 8500,
						LigningsmaesigeFradrag = 8180
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
				.RoundMoney().ShouldEqual(36512.72m);
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
						LigningsmaesigeFradrag = 80000
					});

			var kommunaleSatser = new ValueTuple<KommunaleSatser>(
				new KommunaleSatser
					{
						Kommuneskattesats = 0.2429m
					});

			// Skattepligtig indkomst før modregning
			indkomster[0].SkattepligtigIndkomst.ShouldEqual(-30000);

			var skatterBeregner = new SkatBeregner();
			var skatter = skatterBeregner.BeregnSkat(indkomster, kommunaleSatser);
			
			// Skatter før modregning af underskud og personfradrag
			skatter[0].Bundskat.ShouldEqual(18144);
			skatter[0].Mellemskat.ShouldEqual(768);
			skatter[0].Topskat.ShouldEqual(1920);
			skatter[0].Sundhedsbidrag.ShouldEqual(0);
			skatter[0].Kommuneskat.ShouldEqual(0);

			// Skatteværdi af underskud
			var x = new UnderskudsmodregningBeregner();
			var modregnedeSkatter = x.Beregn(indkomster, skatter, kommunaleSatser);

			modregnedeSkatter[0].Bundskat.ShouldEqual(8457);

			// TODO: Færdiggør her

			//var modregnResult = x.Beregn(indkomster, skatter, kommunaleSatser);

			//indkomster[0].SkattepligtigIndkomst.ShouldEqual(-30000);
		}
	}

	public static class DecimalExtensions
	{
		public static decimal RoundMoney(this decimal value)
		{
			return Math.Round(value, 2, MidpointRounding.ToEven);
		}
	}
}