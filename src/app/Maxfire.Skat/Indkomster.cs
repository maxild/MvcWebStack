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
		/// De personlige indkomst fradragne og ikke medregnede beløb omfattet 
		/// af beløbsgrænsen i pensionsbeskatningslovens § 16, stk. 1.
		/// </summary>
		public decimal KapitalPensionsindskud { get; set; }
	}
}