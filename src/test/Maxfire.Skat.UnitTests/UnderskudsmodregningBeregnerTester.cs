using Maxfire.TestCommons.AssertExtensions;
using Xunit;

namespace Maxfire.Skat.UnitTests
{
	public class UnderskudsmodregningBeregnerTester
	{
		[Fact]
		public void Eksempel()
		{
			var indkomster = new ValueTuple<PersonligeBeloeb>(
				new PersonligeBeloeb
					{
						PersonligIndkomst = 360000,
						NettoKapitalIndkomst = -310000,
						LigningsmaesigeFradrag = 80000
					});

			var skatter = new ValueTuple<Skatter>(
				new Skatter
					{
						Bundskat = 18144,
						Topskat = 1
					});

			var x = new UnderskudsmodregningBeregner();

			//var modregnResult = x.Beregn(indkomster, skatter, kommunaleSatser);

			indkomster[0].SkattepligtigIndkomst.ShouldEqual(-30000);

		}
	}
}