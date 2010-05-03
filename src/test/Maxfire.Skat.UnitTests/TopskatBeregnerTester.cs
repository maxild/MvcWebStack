using Maxfire.TestCommons.AssertExtensions;
using Xunit;

namespace Maxfire.Skat.UnitTests
{
	public class TopskatBeregnerTester
	{
		[Fact]
		public void EksempelFraBeregningAfPersonligeIndkomstSkatter2009Side17()
		{
			// Benytter 2009 regler inden fradrag på 40.000 blev gældende
			Constants.BundfradragPositivKapitalIndkomst = 0;
			
			var personligeBeloeb = new ValueTuple<PersonligeBeloeb>(
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

			var topskat = topskatBeregner.BeregnSkat(personligeBeloeb);

			topskat[0].ShouldEqual(7395);
			topskat[1].ShouldEqual(2775);
		}
	}
}