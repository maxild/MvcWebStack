namespace Maxfire.Skat
{
	public class ModregnResult
	{
		public ModregnResult(Skatter skatter, Skatter modregning, decimal ikkeUdnyttetSkattevaerdi)
		{
			Skatter = skatter;
			UdnyttedeSkattevaerdier = modregning;
			IkkeUdnyttetSkattevaerdi = ikkeUdnyttetSkattevaerdi;
		}

		/// <summary>
		/// Størrelsen af de skatter, der skal modregnes i.
		/// </summary>
		public Skatter Skatter { get; private set; }

		/// <summary>
		/// De udnyttede skatteværdier, der svarer til de mulige modregninger i skatterne.
		/// </summary>
		public Skatter UdnyttedeSkattevaerdier { get; private set; }

		/// <summary>
		/// Skatternes størrelse efter modregning.
		/// </summary>
		public Skatter ModregnedeSkatter
		{
			get { return Skatter - UdnyttedeSkattevaerdier; }
		}
		
		/// <summary>
		/// Skatteværdien af det ikke udnyttede fradrag, underskud el.lign.
		/// </summary>
		public decimal IkkeUdnyttetSkattevaerdi { get; private set; }
		
		/// <summary>
		/// Skatteværdien af det udnyttede fradrag, underskud el.lign.
		/// </summary>
		public decimal UdnyttetSkattevaerdi
		{
			get { return UdnyttedeSkattevaerdier.Sum(); }
		}
	}
}