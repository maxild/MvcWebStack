using Maxfire.TestCommons.AssertExtensions;
using Xunit;

namespace Maxfire.Skat.UnitTests
{
	// TODO: Refactor to Context, Because_Of etc...
	public class SkatteModregnerTester
	{
		[Fact]
		public void FuldModregningAfPositivSkattevaerdi()
		{
			var skatter = new Skatter
			              	{
			              		BeregnetBundskat = 800,
			              		BeregnetMellemskat = 500,
			              		BeregnetTopskat = 1000
			              	};

			var skatteModregner = new SkatteModregner(
				Modregning.Af(x => x.Bundskat).Med(x => x.ModregnetBundskatAfNegativSkattepligtigIndkomst),
				Modregning.Af(x => x.Mellemskat).Med(x => x.ModregnetMellemskatAfNegativSkattepligtigIndkomst));

			var modregnResult = skatteModregner.Modregn(skatter, 1000);

			modregnResult.IkkeUdnyttetSkattevaerdi.ShouldEqual(0);
			modregnResult.ModregnedeSkatter.Bundskat.ShouldEqual(0);
			modregnResult.ModregnedeSkatter.Mellemskat.ShouldEqual(300);
			modregnResult.ModregnedeSkatter.Topskat.ShouldEqual(1000);
		}

		[Fact]
		public void DelvisModregningAfPositivSkattevaerdi()
		{
			var skatter = new Skatter
			{
				BeregnetBundskat = 800,
				BeregnetMellemskat = 500,
				BeregnetTopskat = 1000
			};

			var skatteModregner = new SkatteModregner(
				Modregning.Af(x => x.Bundskat).Med(x => x.ModregnetBundskatAfNegativSkattepligtigIndkomst),
				Modregning.Af(x => x.Mellemskat).Med(x => x.ModregnetMellemskatAfNegativSkattepligtigIndkomst));

			var modregnResult = skatteModregner.Modregn(skatter, 3000);

			modregnResult.IkkeUdnyttetSkattevaerdi.ShouldEqual(1700);
			modregnResult.ModregnedeSkatter.Bundskat.ShouldEqual(0);
			modregnResult.ModregnedeSkatter.Mellemskat.ShouldEqual(0);
			modregnResult.ModregnedeSkatter.Topskat.ShouldEqual(1000);
		}

		[Fact]
		public void FuldModregningAfNegativSkattevaerdi()
		{
			var skatter = new Skatter
			{
				BeregnetBundskat = 800,
				BeregnetMellemskat = 500,
				BeregnetTopskat = 1000
			};

			var skatteModregner = new SkatteModregner(
				Modregning.Af(x => x.Bundskat).Med(x => x.ModregnetBundskatAfNegativSkattepligtigIndkomst),
				Modregning.Af(x => x.Mellemskat).Med(x => x.ModregnetMellemskatAfNegativSkattepligtigIndkomst));

			var modregnResult = skatteModregner.Modregn(skatter, -1000);

			modregnResult.IkkeUdnyttetSkattevaerdi.ShouldEqual(0);
			modregnResult.ModregnedeSkatter.Bundskat.ShouldEqual(0);
			modregnResult.ModregnedeSkatter.Mellemskat.ShouldEqual(300);
			modregnResult.ModregnedeSkatter.Topskat.ShouldEqual(1000);
		}

		[Fact]
		public void DelvisModregningAfNegativSkattevaerdi()
		{
			var skatter = new Skatter
			{
				BeregnetBundskat = 800,
				BeregnetMellemskat = 500,
				BeregnetTopskat = 1000
			};

			var skatteModregner = new SkatteModregner(
				Modregning.Af(x => x.Bundskat).Med(x => x.ModregnetBundskatAfNegativSkattepligtigIndkomst),
				Modregning.Af(x => x.Mellemskat).Med(x => x.ModregnetMellemskatAfNegativSkattepligtigIndkomst));

			var modregnResult = skatteModregner.Modregn(skatter, -4000);

			modregnResult.IkkeUdnyttetSkattevaerdi.ShouldEqual(-2700);
			modregnResult.ModregnedeSkatter.Bundskat.ShouldEqual(0);
			modregnResult.ModregnedeSkatter.Mellemskat.ShouldEqual(0);
			modregnResult.ModregnedeSkatter.Topskat.ShouldEqual(1000);
		}
	}
}