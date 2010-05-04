namespace Maxfire.Skat
{
	public static class Constants
	{
		// Alle derfault værdier er 2009, 2010 værdier (husk ingen regulering fra 2009 til 2010)
		const decimal BUNDFRADRAG_POSITIV_KAPITAL_INDKOMST = 40000;
		const decimal TOPSKAT_BUNDFRADRAG = 347200;
		const decimal MELLEMSKAT_BUNDFRADRAG = 347200;
		const decimal AM_BIDRAG_SATS = 0.08m;
		const decimal BUND_SKATTESATS = 0.0367m;
		const decimal MELLEM_SKATTESATS = 0.06m;
		const decimal TOP_SKATTESATS = 0.15m;
		const decimal SUNDHEDSBIDRAG_SATS = 0.08m;
		const decimal PERSON_FRADRAG = 42900;

		private static decimal? _amBidragsats;
		public static decimal AMBidragsats
		{
			get { return _amBidragsats ?? AM_BIDRAG_SATS; }
			set { _amBidragsats = value; }
		}

		// TODO: Ugifte personer under 18 år har reduceret person fradrag
		private static decimal? _personFradrag;
		public static decimal PersonFradrag
		{
			get { return _personFradrag ?? PERSON_FRADRAG; }
			set { _personFradrag = value; }
		}

		private static decimal? _mellemskatBundfradrag;
		public static decimal MellemskatBundfradrag
		{
			get { return _mellemskatBundfradrag ?? MELLEMSKAT_BUNDFRADRAG; }
			set { _mellemskatBundfradrag = value; }
		}

		private static decimal? _topskatBundfradrag;
		public static decimal TopskatBundfradrag
		{
			get { return _topskatBundfradrag ?? TOPSKAT_BUNDFRADRAG; }
			set { _topskatBundfradrag = value; }
		}

		private static decimal? _bundfradragPositivKapitalIndkomst;
		public static decimal BundfradragPositivKapitalIndkomst
		{
			get { return _bundfradragPositivKapitalIndkomst ?? BUNDFRADRAG_POSITIV_KAPITAL_INDKOMST; }
			set { _bundfradragPositivKapitalIndkomst = value; }
		}

		private static decimal? _sundhedsbidragSats;
		public static decimal Sundhedsbidragsats
		{
			get { return _sundhedsbidragSats ?? SUNDHEDSBIDRAG_SATS; }
			set { _sundhedsbidragSats = value; }
		}

		private static decimal? _bundSkattesats;
		public static decimal Bundskattesats
		{
			get { return _bundSkattesats ?? BUND_SKATTESATS; }
			set { _bundSkattesats = value; }
		}

		private static decimal? _mellemSkattesats;
		public static decimal Mellemskattesats
		{
			get { return _mellemSkattesats ?? MELLEM_SKATTESATS; }
			set { _mellemSkattesats = value; }
		}

		private static decimal? _topSkattesats;
		public static decimal Topskattesats
		{
			get { return _topSkattesats ?? TOP_SKATTESATS; }
			set { _topSkattesats = value; }
		}
	}
}