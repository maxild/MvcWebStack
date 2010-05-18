namespace Maxfire.Skat
{
	// TODO: Prøv at eleiminer en af ModregnResult og ModregnResult
	// TODO: Sørg for så få kald til ctor, istedet skal ToModregnResult extension method benyttes
	public class ModregnResultEx<TSkatter> : ModregnResult<TSkatter>
		where TSkatter : ISumable<decimal>, new()
	{
		public static ModregnResultEx<TSkatter> Nul(TSkatter skatter)
		{
			return new ModregnResultEx<TSkatter>(skatter, 0, new TSkatter(), 0, 0);
		}

		public ModregnResultEx(TSkatter skatter, decimal skattevaerdi, TSkatter udnyttedeSkattevaerdier, 
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