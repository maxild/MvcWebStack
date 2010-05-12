namespace Maxfire.Skat
{
	// TODO: Prøv at eleiminer en af ModregnResult og ModregnResult
	// TODO: Sørg for så få kald til ctor, istedet skal ToModregnResult extension method benyttes
	public class ModregnResultEx : ModregnResult
	{
		public static ModregnResultEx Nul(Skatter skatter)
		{
			return new ModregnResultEx(skatter, 0, Skatter.Nul, 0, 0);
		}

		public ModregnResultEx(Skatter skatter, decimal skattevaerdi, Skatter udnyttedeSkattevaerdier, 
			decimal fradrag, decimal udnyttetFradrag) 
			: base(skatter, skattevaerdi, udnyttedeSkattevaerdier)
		{
			Fradrag = fradrag;
			UdnyttetFradrag = udnyttetFradrag;
		}

		/// <summary>
		/// Størrelsen af det fradrag, der er forsøgt udnyttet.
		/// </summary>
		public decimal Fradrag { get; private set; }
		
		/// <summary>
		/// Størrelsen af det udnyttede fradrag.
		/// </summary>
		public decimal UdnyttetFradrag { get; private set; }

		/// <summary>
		/// Størrelsen af det resterende ikke udnyttede fradrag.
		/// </summary>
		public decimal IkkeUdnyttetFradrag
		{
			get { return Fradrag - UdnyttetFradrag; }
		}
		
	}
}