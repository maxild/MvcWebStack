using Maxfire.TestCommons.AssertExtensions;
using Xunit;

namespace Maxfire.Skat.UnitTests
{
	public class ValueTuppleTester
	{
		[Fact]
		public void DifferentSign_SizeEqualsOne_ReturnsFalse()
		{
			new ValueTupple<decimal>(1).DifferentSign().ShouldBeFalse();
		}

		[Fact]
		public void DifferentSign_BothPositive_ReturnsFalse()
		{
			new ValueTupple<decimal>(1, 1).DifferentSign().ShouldBeFalse();
		}

		[Fact]
		public void DifferentSign_BothNegative_ReturnsFalse()
		{
			new ValueTupple<decimal>(-1, -1).DifferentSign().ShouldBeFalse();
		}

		[Fact]
		public void DifferentSign_OneZero_ReturnsFalse()
		{
			new ValueTupple<decimal>(0, 1).DifferentSign().ShouldBeFalse();
			new ValueTupple<decimal>(1, 0).DifferentSign().ShouldBeFalse();
		}

		[Fact]
		public void DifferentSign_BothZero_ReturnsFalse()
		{
			new ValueTupple<decimal>(0, 0).DifferentSign().ShouldBeFalse();
		}

		[Fact]
		public void DifferentSign_PositiveAndNegative_ReturnsTrue()
		{
			new ValueTupple<decimal>(1, -1).DifferentSign().ShouldBeTrue();
			new ValueTupple<decimal>(-1, 1).DifferentSign().ShouldBeTrue();
		}

		[Fact]
		public void NedbringPositivtMedEvtNegativt_FuldOverfoersel()
		{
			var tupple = new ValueTupple<decimal>(-3, 2);

			var modregnet = tupple.NedbringPositivtMedEvtNegativt();

			modregnet[0].ShouldEqual(-1);
			modregnet[1].ShouldEqual(0);
		}

		[Fact]
		public void NedbringPositivtMedEvtNegativt_DelvisOverfoersel()
		{
			var tupple = new ValueTupple<decimal>(-1, 2);

			var modregnet = tupple.NedbringPositivtMedEvtNegativt();

			modregnet[0].ShouldEqual(0);
			modregnet[1].ShouldEqual(1);
		}

		[Fact]
		public void NedbringPositivtMedEvtNegativt_IngenOverfoersel()
		{
			var tupple = new ValueTupple<decimal>(1, 2);
			tupple.NedbringPositivtMedEvtNegativt().ShouldEqual(tupple);

			tupple = new ValueTupple<decimal>(-1, -2);
			tupple.NedbringPositivtMedEvtNegativt().ShouldEqual(tupple);

			tupple = new ValueTupple<decimal>(0, 2);
			tupple.NedbringPositivtMedEvtNegativt().ShouldEqual(tupple);

			tupple = new ValueTupple<decimal>(2, 0);
			tupple.NedbringPositivtMedEvtNegativt().ShouldEqual(tupple);
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