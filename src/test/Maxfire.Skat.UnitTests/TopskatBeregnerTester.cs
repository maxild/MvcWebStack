using Maxfire.TestCommons.AssertExtensions;
using Xunit;

namespace Maxfire.Skat.UnitTests
{
	public class TopskatBeregnerTester
	{
		[Fact]
		public void UdenSkatteloft_Ugift()
		{
			// Benytter 2009 regler inden fradrag på 40.000 blev gældende
			Constants.BundfradragPositivKapitalIndkomst = 0;
			
			var indkomster = new ValueTuple<PersonligeBeloeb>(
				new PersonligeBeloeb
				{
					PersonligIndkomst = 336000,
					NettoKapitalIndkomst = 28500,
					KapitalPensionsindskud = 32000
				}
			);

			var topskatBeregner = new TopskatBeregner();
			var topskat = topskatBeregner.BeregnSkat(indkomster);

			// 15 pct af (336000 + 32000 + 28500 - 347200)
			topskat[0].ShouldEqual(7395);
		}

		[Fact]
		public void MedSkatteloft_Ugift()
		{
			Constants.Brug2009Vaerdier();

			var indkomster = new ValueTuple<PersonligeBeloeb>(
				new PersonligeBeloeb
				{
					PersonligIndkomst = 336000,
					NettoKapitalIndkomst = 28500,
					KapitalPensionsindskud = 32000
				}
			);

			var kommunaleSatser = new ValueTuple<KommunaleSatser>(
				new KommunaleSatser
				{
					Kommuneskattesats = 0.30m
				});

			var topskatBeregner = new TopskatBeregner();
			var topskat = topskatBeregner.BeregnSkat(indkomster, kommunaleSatser);

			// (15 pct - 5,04 pct) af (336000 + 32000 + 28500 - 347200)
			topskat[0].ShouldEqual(4910.28m);
		}

		[Fact]
		public void UdenSkatteloft_Gift()
		{
			Constants.Brug2009Vaerdier();
			
			var indkomster = new ValueTuple<PersonligeBeloeb>(
				new PersonligeBeloeb
					{
						PersonligIndkomst = 336000,
						NettoKapitalIndkomst = 28500,
						KapitalPensionsindskud = 32000
					},
				new PersonligeBeloeb
					{
						PersonligIndkomst = 92000,
						NettoKapitalIndkomst = 18500
					}
			);

			var kommunaleSatser = new ValueTuple<KommunaleSatser>(
				new KommunaleSatser
				{
					Kommuneskattesats = 0.30m
				},
				new KommunaleSatser
				{
					Kommuneskattesats = 0.28m
				});

			var topskatBeregner = new TopskatBeregner();
			var topskat = topskatBeregner.BeregnSkat(indkomster, kommunaleSatser);

			// (15 pct - 5,04 pct) af (336000 + 32000 + 28500 - 347200), idet uudnyttet bundfradrag ikke kan overføres til ægtefælle
			topskat[0].ShouldEqual(4910.28m);
			// (15 pct - 3,04 pct) af 18500, idet nettokapitalindkomst beskattes hos den med størst grundlag
			topskat[1].ShouldEqual(2212.60m);
		}

		[Fact]
		public void MedSkatteloft_Gift()
		{
			// Benytter 2009 regler inden fradrag på 40.000 blev gældende
			Constants.BundfradragPositivKapitalIndkomst = 0;

			var indkomster = new ValueTuple<PersonligeBeloeb>(
				new PersonligeBeloeb
				{
					PersonligIndkomst = 336000,
					NettoKapitalIndkomst = 28500,
					KapitalPensionsindskud = 32000
				},
				new PersonligeBeloeb
				{
					PersonligIndkomst = 92000,
					NettoKapitalIndkomst = 18500
				}
			);

			var topskatBeregner = new TopskatBeregner();
			var topskat = topskatBeregner.BeregnSkat(indkomster);

			topskat[0].ShouldEqual(7395);
			topskat[1].ShouldEqual(2775);
		}
	}
}