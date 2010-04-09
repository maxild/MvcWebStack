namespace Maxfire.Skat
{
	/// <summary>
	/// Årlige bidrag og præmier.
	/// </summary>
	public class Pensionsbidrag
	{
		public decimal Rate { get; set; }
		public decimal Kapital { get; set; }
		
		public decimal IAlt
		{
			get { return Rate + Kapital; }
		}
	}
}