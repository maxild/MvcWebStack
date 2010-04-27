namespace Maxfire.Skat
{
	/// <summary>
	/// Skattepligtige indkomster
	/// </summary>
	public class Indkomster
	{
		public decimal PersonligIndkomst { get; set; }
		
		public decimal NettoKapitalIndkomst { get; set; }
		
		/// <summary>
		/// // Det beløb, der ikke gives fradrag for i topskatten
		/// </summary>
		public decimal KapitalPensionsindskud { get; set; }
	}
}