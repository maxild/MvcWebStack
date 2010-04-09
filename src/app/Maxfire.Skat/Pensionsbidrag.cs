namespace Maxfire.Skat
{
	/// <summary>
	/// �rlige bidrag og pr�mier.
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