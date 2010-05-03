using Maxfire.TestCommons.AssertExtensions;
using Xunit;

namespace Maxfire.Skat.UnitTests
{
	public class BundskatBeregnerTester
	{
		[Fact]
		public void EksempelFraBeregningAfPersonligeIndkomstSkatter2009Side15()
		{
			var personligeBeloeb = new ValueTuple<PersonligeBeloeb>(
				new PersonligeBeloeb
					{
						PersonligIndkomst = 100000,
						NettoKapitalIndkomst = -50000
					},
				new PersonligeBeloeb
					{
						PersonligIndkomst = 400000,
						NettoKapitalIndkomst = 60000
					}
				);

			var bundskatBeregner = new BundskatGrundlagBeregner();

			var grundlag = bundskatBeregner.BeregnGrundlag(personligeBeloeb);

			grundlag[0].ShouldEqual(100000);
			grundlag[1].ShouldEqual(410000);
		}
	}
}