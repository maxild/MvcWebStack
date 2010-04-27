namespace Maxfire.Skat
{
	public class ModregningPair
	{
		public decimal First { get; private set; }
		public decimal Second { get; private set; }

		public ModregningPair(decimal first, decimal second)
		{
			if (first.DifferentSign(second))
			{
				decimal modregnet = (first + second).NonNegative();
				if (first < 0)
				{
					Second = modregnet;
				}
				else
				{
					First = modregnet;
				}
			}
			else
			{
				First = first.NonNegative();
				Second = second.NonNegative();
			}
		}

		public decimal Sum
		{
			get { return First + Second; }
		} 
	}
}