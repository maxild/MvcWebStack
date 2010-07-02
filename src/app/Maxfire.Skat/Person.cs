using System;

namespace Maxfire.Skat
{
	public class Person
	{
		public Person(DateTime foedselsdato)
		{
			Foedselsdato = foedselsdato;
		}

		public Person(DateTime foedselsdato, int antalBoern) : this(foedselsdato)
		{
			AntalBoern = antalBoern;
		}

		public DateTime Foedselsdato { get; private set; }

		public int AntalBoern { get; private set; }

		/// <summary>
		/// Beregn personens alder ved indkomstårets udløb.
		/// </summary>
		/// <param name="skatteAar">Indkomståret.</param>
		/// <returns>Alder ved indkomstårets udløb.</returns>
		public int GetAlder(int skatteAar)
		{
			DateTime indkomstAaretsUdloeb = new DateTime(skatteAar, 12, 31);
			return GetAlder(indkomstAaretsUdloeb);
		}

		public int GetAlder(DateTime atDate)
		{
			return atDate.Year - Foedselsdato.Year - (Foedselsdato.DayOfYear < atDate.DayOfYear ? 0 : 1);
		}
	}
}