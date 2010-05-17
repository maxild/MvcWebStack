namespace Maxfire.Skat
{
	public static class SkatterExtensions
	{
		public static Skatter RoundMoney(this Skatter skatter)
		{
			return new Skatter
			{
				Kommuneskat = skatter.Kommuneskat.RoundMoney(),
				Kirkeskat = skatter.Kirkeskat.RoundMoney(),
				Sundhedsbidrag = skatter.Sundhedsbidrag.RoundMoney(),
				Bundskat = skatter.Bundskat.RoundMoney(),
				Mellemskat = skatter.Mellemskat.RoundMoney(),
				Topskat = skatter.Topskat.RoundMoney(),
				AktieindkomstskatUnderGrundbeloebet = skatter.AktieindkomstskatUnderGrundbeloebet.RoundMoney(),
				AktieindkomstskatOverGrundbeloebet = skatter.AktieindkomstskatOverGrundbeloebet.RoundMoney()
			};
		}
	}
}