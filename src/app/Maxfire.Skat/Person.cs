using System;

namespace Maxfire.Skat
{
	public class Person
	{
		public Person(DateTime foedselsdato)
		{
			Foedselsdato = foedselsdato;
		}

		public DateTime Foedselsdato { get; private set; }

		/// <summary>
		/// Beregn personens alder ved indkomstårets udløb.
		/// </summary>
		/// <param name="skatteAar">Indkomståret.</param>
		/// <returns>Alder ved indkomstårets udløb.</returns>
		public int GetAlder(int skatteAar)
		{
			DateTime indkomstAaretsUdloeb = new DateTime(skatteAar, 12, 31);
			return indkomstAaretsUdloeb.Year - Foedselsdato.Year - (Foedselsdato.DayOfYear < indkomstAaretsUdloeb.DayOfYear ? 0 : 1);
		}
	}
}