namespace Maxfire.Skat
{
	public static class Constants
	{
		const decimal BUNDFRADRAG_POSITIV_KAPITAL_INDKOMST = 40000;
		public const decimal TopskatBeloebsgraense = 347200;
		public const decimal Topskattesats = 0.15m;
		const decimal SUNDHEDSBIDRAG_SATS = 0.08m;

		private static decimal? _bundfradragPositivKapitalIndkomst;
		public static decimal BundfradragPositivKapitalIndkomst
		{
			get { return _bundfradragPositivKapitalIndkomst ?? BUNDFRADRAG_POSITIV_KAPITAL_INDKOMST; }
			set { _bundfradragPositivKapitalIndkomst = value; }
		}

		private static decimal? _sundhedsbidragSats;
		public static decimal SundhedsbidragSats
		{
			get { return _sundhedsbidragSats ?? SUNDHEDSBIDRAG_SATS; }
			set { _sundhedsbidragSats = value; }
		}
	}
}