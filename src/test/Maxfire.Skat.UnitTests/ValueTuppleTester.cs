using Maxfire.TestCommons.AssertExtensions;
using Xunit;

namespace Maxfire.Skat.UnitTests
{
	public class ValueTuppleTester
	{
		[Fact]
		public void Modregning_FuldOverfoersel()
		{
			var tupple = new ValueTupple<decimal>(-3, 2);

			var modregnet = tupple.Modregn();

			modregnet[0].ShouldEqual(0);
			modregnet[1].ShouldEqual(0);
		}

		[Fact]
		public void Modregning_DelvisOverfoersel()
		{
			var tupple = new ValueTupple<decimal>(-1, 2);

			var modregnet = tupple.Modregn();

			modregnet[0].ShouldEqual(0);
			modregnet[1].ShouldEqual(1);
		}


		[Fact]
		public void Modregning_IngenOverfoersel()
		{
			var tupple = new ValueTupple<decimal>(-1, 2);

			var modregnet = tupple.Modregn();

			modregnet[0].ShouldEqual(0);
			modregnet[1].ShouldEqual(1);
		}

		[Fact]
		public void CanAddDecimals()
		{
			var lhs = new ValueTupple<decimal>(1, 2);
			var rhs = new ValueTupple<decimal>(2, 4);

			var result = lhs + rhs;

			result[0].ShouldEqual(3);
			result[1].ShouldEqual(6);
		}

		[Fact]
		public void CanSubtractDecimals()
		{
			var lhs = new ValueTupple<decimal>(1, 2);
			var rhs = new ValueTupple<decimal>(2, 4);

			var result = lhs - rhs;

			result[0].ShouldEqual(-1);
			result[1].ShouldEqual(-2);
		}

		[Fact]
		public void CanMultiplyDecimals()
		{
			var lhs = new ValueTupple<decimal>(1, 2);
			var rhs = new ValueTupple<decimal>(2, 4);

			var result = lhs * rhs;

			result[0].ShouldEqual(2);
			result[1].ShouldEqual(8);
		}

		[Fact]
		public void CanDivideDecimals()
		{
			var lhs = new ValueTupple<decimal>(1, 2);
			var rhs = new ValueTupple<decimal>(2, 4);

			var result = lhs / rhs;

			result[0].ShouldEqual(0.5m);
			result[1].ShouldEqual(0.5m);
		}

		[Fact]
		public void CanUnaryMinusDecimals()
		{
			var tupple = new ValueTupple<decimal>(1, 2);
			var result = -tupple;

			result[0].ShouldEqual(-1);
			result[1].ShouldEqual(-2);
		}

		[Fact]
		public void UnaryPlusMakesResultNonNegative()
		{
			var tupple = new ValueTupple<decimal>(-1, 2);
			var result = +tupple;

			result[0].ShouldEqual(0);
			result[1].ShouldEqual(2);
		}
	}
}