using System;

namespace Maxfire.Skat
{
	// TODO: make this type immutable, but keep it working with Accessor logic
	// TODO: Equals, GetHashCode
	// TODO: Refactor to finer grained value objects
	public class Skatter : IEquatable<Skatter>
	{
		public decimal ModregnetKirkeskatAfPersonfradrag { get; set; }
		public decimal BeregnetKirkeskat { get; set; }
		public decimal Kirkeskat
		{
			get { return BeregnetKirkeskat - ModregnetKirkeskatAfPersonfradrag; }
		}

		public decimal ModregnetKommuneskatAfPersonfradrag { get; set; }
		public decimal BeregnetKommuneskat { get; set; }
		public decimal Kommuneskat
		{
			get { return BeregnetKommuneskat - ModregnetKommuneskatAfPersonfradrag; }
		}

		public decimal ModregnetSundhedsbidragAfPersonfradrag { get; set; }
		public decimal BeregnetSundhedsbidrag { get; set; }
		public decimal Sundhedsbidrag
		{
			get { return BeregnetSundhedsbidrag - ModregnetSundhedsbidragAfPersonfradrag; }
		}

		public decimal ModregnetBundskatAfPersonfradrag { get; set; }
		public decimal ModregnetBundskatAfNegativSkattepligtigIndkomst { get; set; }
		public decimal BeregnetBundskat { get; set; }
		public decimal Bundskat
		{
			get { return BeregnetBundskat - ModregnetBundskatAfNegativSkattepligtigIndkomst - ModregnetBundskatAfPersonfradrag; }
		}

		public decimal ModregnetMellemskatAfPersonfradrag { get; set; }
		public decimal ModregnetMellemskatAfNegativSkattepligtigIndkomst { get; set; }
		public decimal BeregnetMellemskat { get; set; }
		public decimal Mellemskat
		{
			get { return BeregnetMellemskat - ModregnetMellemskatAfNegativSkattepligtigIndkomst - ModregnetMellemskatAfPersonfradrag; }
		}

		public decimal ModregnetTopskatAfPersonfradrag { get; set; }
		public decimal ModregnetTopskatAfNegativSkattepligtigIndkomst { get; set; }
		public decimal BeregnetTopskat { get; set; }
		public decimal Topskat
		{
			get { return BeregnetTopskat - ModregnetTopskatAfNegativSkattepligtigIndkomst - ModregnetTopskatAfPersonfradrag; }
		}

		public decimal KommunalIndkomstskatOgKirkeskat
		{
			get { return Kommuneskat + Kirkeskat; }
		}

		public decimal ModregnetAktieindkomstskatAfPersonfradrag { get; set; }
		public decimal ModregnetAktieindkomstskatAfNegativSkattepligtigIndkomst { get; set; }

		public decimal Aktieindkomstskat
		{
			get 
			{
				return AktieindkomstskatUnderGrundbeloebet + AktieindkomstskatOverGrundbeloebet 
				       - ModregnetAktieindkomstskatAfNegativSkattepligtigIndkomst 
				       - ModregnetAktieindkomstskatAfPersonfradrag; 
			}
		}

		/// <summary>
		/// I følge bestemmelser i PSL § 8a, stk. 1 bliver skat af aktieindkomst, som ikke overstiger
		/// grundbeløbet (48.300 kr. i 2009 og 2010) beregnet som en endelig skat på 28 pct. Indeholdt
		/// udbytteskat efter KSL § 65 af aktieindkomst, der ikke overstiger grundbeløbet, er endelig 
		/// betaling af skatten, og udbytteskatten modregnes ikke i slutskatten efter KSL § 67.
		/// </summary>
		/// <remarks>
		/// Skatten af aktieindkomst under progressionsgrænsen er endelig og svarer for udbytters 
		/// vedkommende til den udbytteskat, som selskabet har indeholdt. At skatten er endelig 
		/// indebærer, at der ikke kan modregnes skatteværdi af negativ skattepligtig indkomst 
		/// eller skatteværdi af personfradrag. Dette gælder også i de tilfælde, hvor der ikke 
		/// er indeholdt skat, f.eks. ved maskeret udlodning og i tilfælde, hvor aktieavancer 
		/// indgår i aktieindkomsten. I det omfang der ikke er indeholdt udbytteskat af 
		/// aktieindkomsten, forhøjes modtagerens slutskat med det manglende beløb. 
		/// </remarks>
		public decimal AktieindkomstskatUnderGrundbeloebet { get; set; }
		
		/// <summary>
		/// I følge bestemmelser i PSL § 8a, stk. 2 vil skat af aktieindkomst, der overstiger
		/// grundbeløbet (48.300 kr. i 2009 og 2010) indgå i slutskatten, og den udbytteskat
		/// der er indeholdt i denne del af udbyttet efter KSL § 65 modregnes i slutskatten 
		/// efter KSL § 67.
		/// </summary>
		/// <remarks>
		/// Den overskydende del af aktieindkomstskatten indgår altså i slutskatten og indeholdt
		/// udbytteskat modregnes.
		/// Aktieindkomst, der overstiger progressionsgrænsen, beskattes med 42 pct. af det 
		/// overskydende beløb. Denne skat er i modsætning til den lave skat ikke endelig, og 
		/// der kan således foretages modregning heri af skatteværdi af personfradrag og 
		/// negativ skattepligtig indkomst. 
		/// </remarks>
		public decimal AktieindkomstskatOverGrundbeloebet { get; set; }

		public Skatter Clone()
		{
			return (Skatter)MemberwiseClone();
		}

		public decimal Sum()
		{
			return Sundhedsbidrag + Bundskat + Mellemskat + Topskat 
				+ KommunalIndkomstskatOgKirkeskat + Aktieindkomstskat;
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
				hashCode = (hashCode * 397) ^ AktieindkomstskatUnderGrundbeloebet.GetHashCode();
				hashCode = (hashCode * 397) ^ AktieindkomstskatOverGrundbeloebet.GetHashCode();
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
				AktieindkomstskatUnderGrundbeloebet == other.AktieindkomstskatUnderGrundbeloebet &&
				AktieindkomstskatOverGrundbeloebet == other.AktieindkomstskatOverGrundbeloebet);
		}
	}
}