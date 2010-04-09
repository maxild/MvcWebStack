namespace Maxfire.Skat
{
	public class Skattesatser
	{
		public decimal Bundskattesats { get; set; }
		public decimal Topskattesats { get; set; }
		public decimal Sundhedsbidrag { get; set; }
		public decimal AktieindkomstLavesteSats { get; set; }
		public decimal AktieindkomstHoejesteSats { get; set; }
		public decimal AktieindkomstModregningVedNegativSkat { get; set; }
		public decimal Skatteloft { get; set; }
	}

	public class Satser
	{
		public decimal BeskaeftigelsesfradragSats { get; set; }
	}

	// TODO: IDictionary --> registry (per skatteår) (unflattening a la NameValueDeserializer, DefaultModelBinder)

	// TODO: Man skal kunne se de konfigurerede satser og beløbsgrænser registry --> IDictionary (flattening a la AutoMapper)

	// Note: Registry == Singleton
}