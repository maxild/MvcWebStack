using System;

namespace Maxfire.Skat
{
	public static class DecimalExtensions
	{
		public static int Sign(this decimal number)
		{
			if (number < 0)
			{
				return -1;
			}
			return number > 0 ? 1 : 0;
		}

		public static bool DifferentSign(this decimal lhs, decimal rhs)
		{
			return lhs.Sign() * rhs.Sign() == -1;
		}

		public static decimal NonNegative(this decimal number)
		{
			return Math.Max(0, number);
		}

		public static decimal RoundMoney(this decimal value)
		{
			return Math.Round(value, 2, MidpointRounding.ToEven);
		}
	}
}