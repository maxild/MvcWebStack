using System;

namespace Maxfire.Skat
{
	public class EjendomsvaerdiskatBeregner
	{
		private readonly IEjendomsskattelovRegistry _registry;

		public EjendomsvaerdiskatBeregner(IEjendomsskattelovRegistry registry)
		{
			_registry = registry;
		}

		// Input:
		//
		// OBS: Hvordan ved vi om personerne er gift, idet ugifte godt kan eje bolig sammen????
		public ValueTuple<decimal> BeregnSkat(IEjendomsoplysninger ejendomsoplysninger, ValueTuple<Person> ejere, 
			ValueTuple<decimal> ejerandele, int skatteAar)
		{
			decimal lavsats  = _registry.GetSkattesatsForUnderProgressionsgraense(skatteAar);
			decimal hoejsats = _registry.GetSkattesatsForOverProgressionsgraense(skatteAar) - lavsats;
			decimal progressionsgraense = _registry.GetProgressionsgraense(skatteAar);
			decimal ejendomsvaerdi = ejendomsoplysninger.Ejendomsvaerdi;
			var fordelingsnoegle = ejerandele.NormalizeAndele();

			decimal ejendomsvaerdiskatAfLav = lavsats * ejendomsvaerdi;
			decimal ejendomsvaerdiskatAfHoej = hoejsats * ejendomsvaerdi.DifferenceGreaterThan(progressionsgraense);
			decimal ejendomsvaerdiskat = ejendomsvaerdiskatAfLav + ejendomsvaerdiskatAfHoej;

			decimal nedslagVedKoebSenest01071998 = 0;
			if (ejendomsoplysninger.IsKoebtEfter01071998 == false)
			{
				decimal nedslagssatsP6 = _registry.GetNedslagssatsForParagraf6(skatteAar);
				decimal nedslagP6 = nedslagssatsP6 * ejendomsvaerdi;
				decimal nedslagP7 = 0;
				if (ejendomsoplysninger.IsEjerlejlighed == false)
				{
					decimal nedslagssatsP7 = _registry.GetNedslagssatsForParagraf7(skatteAar);
					decimal maksNedslagP7 = _registry.GetMaksimaltNedslagForParagraf7(skatteAar);
					nedslagP7 = (nedslagssatsP7 * ejendomsvaerdi).Loft(maksNedslagP7);
				}
				nedslagVedKoebSenest01071998 = nedslagP6 + nedslagP7;
			}

			decimal pensionistNedslag = 0;

			decimal reduktionAfPensionistNedslag = 0;

			return fordelingsnoegle * (ejendomsvaerdiskat - (nedslagVedKoebSenest01071998 + pensionistNedslag));
		}
	}

	// Grundv�rdi, s�fremt grundskyld skal kunne beregnes. Grundskyld betales ikke over selvangivelsen men opkr�ves via indbetalingskort
	public interface IEjendomsoplysninger
	{
		/// <summary>
		/// Grundlaget for ejendomsv�rdiskatten
		/// </summary>
		decimal Ejendomsvaerdi { get; }

		/// <summary>
		/// Ejendomme hvor k�bsaftale er underskrevet af b�de k�ber og s�lger d. 1/7-1998, 
		/// eller tidligere, opn�r nedslag i ejendomsv�rdiskatten.
		/// </summary>
		bool IsKoebtEfter01071998 { get; }

		/// <summary>
		/// Pensionister opn�r nedslag p� fritidsboliger
		/// </summary>
		bool IsFritidsbolig { get; }
		
		/// <summary>
		/// Ejendomme hvor k�bsaftale er underskrevet af b�de k�ber og s�lger d. 1/7-1998, 
		/// eller tidligere, opn�r et yderligere nedslag i ejendomsv�rdiskatten, hvis der 
		/// ikke er tale om ejerlejligheder.
		/// </summary>
		bool IsEjerlejlighed { get; }
	}

	public interface IEjendomsskattelovRegistry
	{

		decimal GetSkattesatsForUnderProgressionsgraense(decimal skatteAar);
		decimal GetSkattesatsForOverProgressionsgraense(decimal skatteAar);

		/// <summary>
		/// Afl�s progressionsgr�nsen for beregning af ejendomsv�rdiskat.
		/// (Ejendomsv�rdiskatteloven � 5, stk. 1)
		/// </summary>
		decimal GetProgressionsgraense(decimal skatteAar);

		/// <summary>
		/// Afl�s det procentuelle nedslag i ejendomsv�rdiskatten for ejerboliger erhvervet den 1. juli 1998 eller tidligere.
		/// (Ejendomsv�rdiskatteloven � 6, stk. 1)
		/// </summary>
		decimal GetNedslagssatsForParagraf6(decimal skatteAar);

		/// <summary>
		/// Afl�s det maksimale nedslag i ejendomsv�rdiskatten for ejerboliger erhvervet den 1. juli 1998 eller tidligere.
		/// (Ejendomsv�rdiskatteloven � 7, stk. 1)
		/// </summary>
		decimal GetMaksimaltNedslagForParagraf7(decimal skatteAar);

		/// <summary>
		/// Afl�s det procentuelle nedslag i ejendomsv�rdiskatten for ejerboliger erhvervet den 1. juli 1998 eller tidligere.
		/// (Ejendomsv�rdiskatteloven � 7, stk. 1)
		/// </summary>
		decimal GetNedslagssatsForParagraf7(decimal skatteAar);
		
	}
}