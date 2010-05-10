namespace Maxfire.Skat
{
	public class ModregnResult2 : ModregnResult
	{
		public ModregnResult2(Skatter modregnedeSkatter, decimal ikkeUdnyttetSkattevaerdi, decimal ikkeUdnyttetFradrag) 
			: base(modregnedeSkatter, ikkeUdnyttetSkattevaerdi)
		{
			IkkeUdnyttetFradrag = ikkeUdnyttetFradrag;
		}

		/// <summary>
		/// Størrelsen af det resterende ikke udnyttede fradrag.
		/// </summary>
		public decimal IkkeUdnyttetFradrag { get; private set; }
	}
}