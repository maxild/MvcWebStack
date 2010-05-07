namespace Maxfire.Skat
{
	public interface ISkattesatser
	{
		decimal Kirkeskattesats { get; set; }
		decimal Kommuneskattesats { get; set; }
		decimal Sundhedsbidragsats { get; set; }
		decimal Bundskattesats { get; }
		decimal Mellemskattesats { get; }
		decimal Topskattesats { get; }
		decimal SkatAfAktieindkomstsats { get; }
	}

	// TODO: make this type immutable, but keep it working with Accessor logic
	public class Skatter
	{
		public decimal Kirkeskat { get; set; }
		public decimal Kommuneskat { get; set; }
		public decimal Sundhedsbidrag { get; set; }
		public decimal Bundskat { get; set; }
		public decimal Mellemskat { get; set; }
		public decimal Topskat { get; set; }
		
		public decimal KommunalIndkomstskatOgKirkeskat
		{
			get { return Kommuneskat + Kirkeskat; }
		}

		/// <summary>
		/// Kun den del angivet ved PSL § 8a, stk. 2, der angiver den del af skat af aktieindkomst, der
		/// overstiger progressionsgrænsen, og ikke allerede er betalt som kildeskat (A-skat). Dermed 
		/// angiver beløbet den del der er B-skat.
		/// </summary>
		public decimal SkatAfAktieindkomst { get; set; }

		public Skatter Clone()
		{
			return (Skatter)MemberwiseClone();
		}

		public decimal Sum()
		{
			return Sundhedsbidrag + Bundskat + Mellemskat + Topskat + KommunalIndkomstskatOgKirkeskat;
		}
	}
}