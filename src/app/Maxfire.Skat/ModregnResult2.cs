namespace Maxfire.Skat
{
	public class ModregnResult2 : ModregnResult
	{
		public ModregnResult2(Skatter skatter, Skatter modregning, decimal ikkeUdnyttetSkattevaerdi, decimal ikkeUdnyttetFradrag, decimal udnyttetFradrag) 
			: base(skatter, modregning, ikkeUdnyttetSkattevaerdi)
		{
			IkkeUdnyttetFradrag = ikkeUdnyttetFradrag;
			UdnyttetFradrag = udnyttetFradrag;
		}

		/// <summary>
		/// Størrelsen af det resterende ikke udnyttede fradrag.
		/// </summary>
		public decimal IkkeUdnyttetFradrag { get; private set; }
		
		/// <summary>
		/// Størrelsen af de udnyttede fradrag.
		/// </summary>
		public decimal UdnyttetFradrag { get; private set; }
	}
}