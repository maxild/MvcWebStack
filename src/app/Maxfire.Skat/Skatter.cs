using System;
using Maxfire.Core;

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
	public class Skatter : IEquatable<Skatter>
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

		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = Kirkeskat.GetHashCode();
				hashCode = (hashCode * 397) ^ Kommuneskat.GetHashCode();
				hashCode = (hashCode * 397) ^ Sundhedsbidrag.GetHashCode();
				hashCode = (hashCode * 397) ^ Bundskat.GetHashCode();
				hashCode = (hashCode * 397) ^ Mellemskat.GetHashCode();
				hashCode = (hashCode * 397) ^ Topskat.GetHashCode();
				hashCode = (hashCode * 397) ^ SkatAfAktieindkomst.GetHashCode();
				return hashCode;
			}
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as Skatter);
		}

		public bool Equals(Skatter other)
		{
			if (other == null)
			{
				return false;
			}

			return (Kirkeskat == other.Kirkeskat &&
				Kommuneskat == other.Kommuneskat &&
				Sundhedsbidrag == other.Sundhedsbidrag &&
				Bundskat == other.Bundskat &&
				Mellemskat == other.Mellemskat &&
				Topskat == other.Topskat && 
				SkatAfAktieindkomst == other.SkatAfAktieindkomst);
		}
	}
}