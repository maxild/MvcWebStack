using Maxfire.TestCommons.AssertExtensions;
using Xunit;

namespace Maxfire.Skat.UnitTests
{
	public class SundhedsbidragBeregnerTester
	{
		[Fact]
		public void BeregnSkat()
		{
			Constants.Sundhedsbidragsats = 0.08m;

			var personligeBeloeb = new ValueTuple<PersonligeBeloeb>(
				new PersonligeBeloeb
				{
					PersonligIndkomst = 100
				}
			);

			var sundhedsbidragBeregner = new SundhedsbidragBeregner();

			var sundhedsbidrag = sundhedsbidragBeregner.BeregnSkat(personligeBeloeb);

			sundhedsbidrag[0].ShouldEqual(8);
		}
	}
}