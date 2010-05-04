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
		public void Eksempel_6_IngenUnderskud_Ugift()
		{
			// 2009 v�rdier
			Constants.BundfradragPositivKapitalIndkomst = 0;
			Constants.Bundskattesats = 0.0504m;
			Constants.Mellemskattesats = 0.06m;
			Constants.Topskattesats = 0.15m;
			Constants.Sundhedsbidragsats = 0.08m;
			Constants.MellemskatBundfradrag = 347200;
			Constants.TopskatBundfradrag = 347200;

			// Note: Indkomstopg�relsen mangler

			var kommunaleSatser = new ValueTuple<KommunaleSatser>(
				new KommunaleSatser
					{
						Kommuneskattesats = 0.237m,
						Kirkeskattesats = 0.0059m
				});

			var indkomster = new ValueTuple<PersonligeBeloeb>(
				new PersonligeBeloeb
					{
						PersonligIndkomst = 358395,		// Skattepligtig l�ninkomst - Indskud p� privattegnet kapitalpension
						NettoKapitalIndkomst = 8500,	// Anden kapital indkomst + Renteindt�gter - Renteudg
						LigningsmaesigeFradrag = 24600,	// Befordring, Besk�ftigelsesfradrag og Fagligekontingent
						KapitalPensionsindskud = 46000
					});

			indkomster[0].SkattepligtigIndkomst.ShouldEqual(342295);

			var bundskatBeregner = new BundskatBeregner();
			var bundskat = bundskatBeregner.BeregnSkat(indkomster);

			var mellemskatBeregner = new MellemskatBeregner();
			var mellemskat = mellemskatBeregner.BeregnSkat(indkomster);

			var topskatBeregner = new TopskatBeregner();
			var topskat = topskatBeregner.BeregnSkat(indkomster);

			var sundhedsbidragBeregner = new SundhedsbidragBeregner();
			var sundhedsbidrag = sundhedsbidragBeregner.BeregnSkat(indkomster);

			var kommuneskatBeregner = new KommuneskatBeregner();
			var kommuneskat = kommuneskatBeregner.BeregnSkat(indkomster, kommunaleSatser);

			var kirkeskatBeregner = new KirkeskatBeregner();
			var kirkeskat = kirkeskatBeregner.BeregnSkat(indkomster, kommunaleSatser);

			// Skatter f�r modregning af underskud og personfradrag
			bundskat[0].RoundMoney().ShouldEqual(18491.51m);
			mellemskat[0].RoundMoney().ShouldEqual(1181.70m);
			topskat[0].RoundMoney().ShouldEqual(9854.25m);
			sundhedsbidrag[0].RoundMoney().ShouldEqual(27383.60m);
			(kirkeskat[0] + kommuneskat[0]).RoundMoney().ShouldEqual(83143.46m);

			// Beregning af skattev�rdier af personfradrag
			var x = new PersonFradragBeregner();
			var skattevaerdier = x.BeregnSkattevaerdier(kommunaleSatser);

			skattevaerdier[0].Bundskat.ShouldEqual(2162.16m);
			skattevaerdier[0].Sundhedsbidrag.ShouldEqual(3432);
			skattevaerdier[0].KommunalIndkomstskatOgKirkeskat.ShouldEqual(10420.41m);
		}

		[Fact] 
		public void Eksempel_9()
		{
			
		}

		[Fact] 
		public void Eksempel_12_NegativSkattepigtigIndkomst_Ugift()
		{
			// 2009 v�rdier
			Constants.Bundskattesats = 0.0504m;
			Constants.Mellemskattesats = 0.06m;
			Constants.Topskattesats = 0.15m;
			Constants.MellemskatBundfradrag = 347200;
			Constants.TopskatBundfradrag = 347200;

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

			// Skattepligtig indkomst f�r modregning
			indkomster[0].SkattepligtigIndkomst.ShouldEqual(-30000);

			// Skatter der ikke benytter skattepligtig indkomst som grundlag

			var bundskatBeregner = new BundskatBeregner();
			var bundskat = bundskatBeregner.BeregnSkat(indkomster);

			var mellemskatBeregner = new MellemskatBeregner();
			var mellemskat = mellemskatBeregner.BeregnSkat(indkomster);

			var topskatBeregner = new TopskatBeregner();
			var topskat = topskatBeregner.BeregnSkat(indkomster);

			// Skatter der benytter den skattepligtige indkomst (beregnere s�rger for negativt grundlag nulstilles)

			var sundhedsbidragBeregner = new SundhedsbidragBeregner();
			var sundhedsbidrag = sundhedsbidragBeregner.BeregnSkat(indkomster);

			var kommuneskatBeregner = new KommuneskatBeregner();
			var kommuneskat = kommuneskatBeregner.BeregnSkat(indkomster, kommunaleSatser);

			// Skatter f�r modregning af underskud og personfradrag
			bundskat[0].ShouldEqual(18144);
			mellemskat[0].ShouldEqual(768);
			topskat[0].ShouldEqual(1920);
			sundhedsbidrag[0].ShouldEqual(0);
			kommuneskat[0].ShouldEqual(0);

			var skatter = new ValueTuple<Skatter>(new Skatter
			                                      	{
			                                      		Bundskat = bundskat[0],
														Mellemskat = mellemskat[0],
			                                      		Topskat = topskat[0],
			                                      		Sundhedsbidrag = sundhedsbidrag[0],
			                                      		Kommuneskat = kommuneskat[0]
			                                      	});

			// Skattev�rdi af underskud
			var x = new UnderskudsmodregningBeregner();
			var modregnedeSkatter = x.Beregn(indkomster, skatter, kommunaleSatser);

			modregnedeSkatter[0].Bundskat.ShouldEqual(8457);

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