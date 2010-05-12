namespace Maxfire.Skat
{
	// TODO: Væk med disse globale værdier (for mange side-effekter i tests o.lign.)
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
		const decimal AKTIEINDKOMST_LAVESTE_PROGRESSIONSGRAENSE = 48300;
		const decimal AKTIEINDKOMST_HOEJESTE_PROGRESSIONSGRAENSE = decimal.MaxValue;
		const decimal AKTIEINDKOMST_LAVESTE_SKATTESATS = 0.28m;
		const decimal AKTIEINDKOMST_MELLEMSTE_SKATTESATS = 0.42m;
		const decimal AKTIEINDKOMST_HOEJESTE_SKATTESATS = 0.42m;

		private static decimal? _aktieIndkomstLavesteSkattesats;
		public static decimal AktieIndkomstLavesteSkattesats
		{
			get { return _aktieIndkomstLavesteSkattesats ?? AKTIEINDKOMST_LAVESTE_SKATTESATS; }
			set { _aktieIndkomstLavesteSkattesats = value; }
		}

		private static decimal? _aktieIndkomstMellemsteSkattesats;
		public static decimal AktieIndkomstMellemsteSkattesats
		{
			get { return _aktieIndkomstMellemsteSkattesats ?? AKTIEINDKOMST_MELLEMSTE_SKATTESATS; }
			set { _aktieIndkomstMellemsteSkattesats = value; }
		}

		private static decimal? _aktieIndkomstHoejesteSkattesats;
		public static decimal AktieIndkomstHoejesteSkattesats
		{
			get { return _aktieIndkomstHoejesteSkattesats ?? AKTIEINDKOMST_HOEJESTE_SKATTESATS; }
			set { _aktieIndkomstHoejesteSkattesats = value; }
		}

		private static decimal? _aktieIndkomstLavesteProgressionsgraense;
		public static decimal AktieIndkomstLavesteProgressionsgraense
		{
			get { return _aktieIndkomstLavesteProgressionsgraense ?? AKTIEINDKOMST_LAVESTE_PROGRESSIONSGRAENSE; }
			set { _aktieIndkomstLavesteProgressionsgraense = value; }
		}

		private static decimal? _aktieIndkomstHoejesteProgressionsgraense;
		public static decimal AktieIndkomstHoejesteProgressionsgraense
		{
			get { return _aktieIndkomstHoejesteProgressionsgraense ?? AKTIEINDKOMST_HOEJESTE_PROGRESSIONSGRAENSE; }
			set { _aktieIndkomstHoejesteProgressionsgraense = value; }
		}

		private static decimal? _amBidragsats;
		public static decimal AMBidragsats
		{
			get { return _amBidragsats ?? AM_BIDRAG_SATS; }
			set { _amBidragsats = value; }
		}

		// TODO: Ugifte personer under 18 år har reduceret person fradrag
		private static decimal? _personFradrag;
		public static decimal Personfradrag
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

		public static void Brug2009Vaerdier()
		{
			Personfradrag = PERSON_FRADRAG;
			BundfradragPositivKapitalIndkomst = 0;
			Bundskattesats = 0.0504m;
			Mellemskattesats = 0.06m;
			Topskattesats = 0.15m;
			Sundhedsbidragsats = 0.08m;
			MellemskatBundfradrag = 347200;
			TopskatBundfradrag = 347200;
			AktieIndkomstLavesteProgressionsgraense = 48300;
			AktieIndkomstHoejesteProgressionsgraense = 106100;
			AktieIndkomstLavesteSkattesats = 0.28m;
			AktieIndkomstMellemsteSkattesats = 0.43m;
			AktieIndkomstHoejesteSkattesats = 0.45m;
		}
	}
}