using Maxfire.Core.Reflection;
using Maxfire.TestCommons.AssertExtensions;
using Xunit;

namespace Maxfire.Skat.UnitTests
{
	public class SkatteModregnerTester
	{
		[Fact]
		public void FuldModregningAfPositivSkattevaerdi()
		{
			var skatter = new Skatter
			              	{
			              		Bundskat = 1000,
			              		Topskat = 500,
			              		Sundhedsbidrag = 800
			              	};

			var skatteModregner = new SkatteModregner(
				IntrospectionOf<Skatter>.GetAccessorFor(x => x.Topskat),
				IntrospectionOf<Skatter>.GetAccessorFor(x => x.Sundhedsbidrag));

			var modregnResult = skatteModregner.Modregn(skatter, 1000);

			modregnResult.IkkeUdnyttetSkattevaerdi.ShouldEqual(0);
			modregnResult.ModregnedeSkatter.Topskat.ShouldEqual(0);
			modregnResult.ModregnedeSkatter.Sundhedsbidrag.ShouldEqual(300);
			modregnResult.ModregnedeSkatter.Bundskat.ShouldEqual(1000);
		}

		[Fact]
		public void DelvisModregningAfPositivSkattevaerdi()
		{
			var skatter = new Skatter
			{
				Bundskat = 1000,
				Topskat = 500,
				Sundhedsbidrag = 800
			};

			var skatteModregner = new SkatteModregner(
				IntrospectionOf<Skatter>.GetAccessorFor(x => x.Topskat),
				IntrospectionOf<Skatter>.GetAccessorFor(x => x.Sundhedsbidrag));

			var modregnResult = skatteModregner.Modregn(skatter, 3000);

			modregnResult.IkkeUdnyttetSkattevaerdi.ShouldEqual(1700);
			modregnResult.ModregnedeSkatter.Topskat.ShouldEqual(0);
			modregnResult.ModregnedeSkatter.Sundhedsbidrag.ShouldEqual(0);
			modregnResult.ModregnedeSkatter.Bundskat.ShouldEqual(1000);
		}

		[Fact]
		public void FuldModregningAfNegativSkattevaerdi()
		{
			var skatter = new Skatter
			{
				Bundskat = 1000,
				Topskat = 500,
				Sundhedsbidrag = 800
			};

			var skatteModregner = new SkatteModregner(
				IntrospectionOf<Skatter>.GetAccessorFor(x => x.Topskat),
				IntrospectionOf<Skatter>.GetAccessorFor(x => x.Sundhedsbidrag));

			var modregnResult = skatteModregner.Modregn(skatter, -1000);

			modregnResult.IkkeUdnyttetSkattevaerdi.ShouldEqual(0);
			modregnResult.ModregnedeSkatter.Topskat.ShouldEqual(0);
			modregnResult.ModregnedeSkatter.Sundhedsbidrag.ShouldEqual(300);
			modregnResult.ModregnedeSkatter.Bundskat.ShouldEqual(1000);
		}

		[Fact]
		public void DelvisModregningAfNegativSkattevaerdi()
		{
			var skatter = new Skatter
			{
				Bundskat = 1000,
				Topskat = 500,
				Sundhedsbidrag = 800
			};

			var skatteModregner = new SkatteModregner(
				IntrospectionOf<Skatter>.GetAccessorFor(x => x.Topskat),
				IntrospectionOf<Skatter>.GetAccessorFor(x => x.Sundhedsbidrag));

			var modregnResult = skatteModregner.Modregn(skatter, -4000);

			modregnResult.IkkeUdnyttetSkattevaerdi.ShouldEqual(-2700);
			modregnResult.ModregnedeSkatter.Topskat.ShouldEqual(0);
			modregnResult.ModregnedeSkatter.Sundhedsbidrag.ShouldEqual(0);
			modregnResult.ModregnedeSkatter.Bundskat.ShouldEqual(1000);
		}
	}
}