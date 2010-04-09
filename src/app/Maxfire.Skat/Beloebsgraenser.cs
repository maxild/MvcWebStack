namespace Maxfire.Skat
{
	/// <summary>
	/// Beløbsgrænser i skattelovgivningen der reguleres efter personskattelovens § 20
	/// </summary>
	public class Beloebsgraenser
	{
		public Beloebsgraenser()
		{
			Topskat = new TopskatBeloebsgraenser();
		}

		public PersonfradragBeloebsgraenser Personfradrag { get; set; }
		public TopskatBeloebsgraenser Topskat { get; private set; }
	}

	public class PersonfradragBeloebsgraenser
	{
		public decimal Fyldt18Aar { get; set; }
		public decimal Under18Aar { get; set; }
	}

	public class TopskatBeloebsgraenser
	{
		public decimal Progressionsgraense { get; set; }
		public decimal BundfradragForPositivNettokapitalindkomst { get; set; }
	}

	public class AktieindkomstBeloebsgraenser
	{
		public decimal Progressionsgraense { get; set; }
	}

	public class Beskaeftigelsesfradrag
	{
		public decimal Sats { get; set; }
		public decimal Loft { get; set; }
	}

	/// <summary>
	/// Mht skatteår
	/// </summary>
	public class BeloebsgraenserRegistry
	{
		
	}
}