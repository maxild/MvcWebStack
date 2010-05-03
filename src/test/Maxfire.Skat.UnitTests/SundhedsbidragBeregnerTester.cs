using Maxfire.TestCommons.AssertExtensions;
using Xunit;

namespace Maxfire.Skat.UnitTests
{
	public class SundhedsbidragBeregnerTester
	{
		[Fact]
		public void BeregnSkat()
		{
			Constants.SundhedsbidragSats = 0.08m;

			var personligeBeloeb = new ValueTuple<PersonligeBeloeb>(
				new PersonligeBeloeb
				{
					SkattepligtigIndkomst = 100
				}
			);

			var sundhedsbidragBeregner = new SundhedsbidragBeregner();

			var sundhedsbidrag = sundhedsbidragBeregner.BeregnSkat(personligeBeloeb);

			sundhedsbidrag[0].ShouldEqual(8);
		}
	}
}