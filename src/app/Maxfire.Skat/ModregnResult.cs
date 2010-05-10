namespace Maxfire.Skat
{
	public class ModregnResult
	{
		public ModregnResult(Skatter modregnedeSkatter, decimal ikkeUdnyttetSkattevaerdi)
		{
			ModregnedeSkatter = modregnedeSkatter;
			IkkeUdnyttetSkattevaerdi = ikkeUdnyttetSkattevaerdi;
		}

		/// <summary>
		/// Skatternes størrelse after modregning.
		/// </summary>
		public Skatter ModregnedeSkatter { get; private set; }
		
		/// <summary>
		/// Skatteværdien af det ikke udnyttede fradrag.
		/// </summary>
		public decimal IkkeUdnyttetSkattevaerdi { get; private set; }
	}
}