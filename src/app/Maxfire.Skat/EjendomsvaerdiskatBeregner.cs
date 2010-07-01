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

	// Grundværdi, såfremt grundskyld skal kunne beregnes. Grundskyld betales ikke over selvangivelsen men opkræves via indbetalingskort
	public interface IEjendomsoplysninger
	{
		/// <summary>
		/// Grundlaget for ejendomsværdiskatten
		/// </summary>
		decimal Ejendomsvaerdi { get; }

		/// <summary>
		/// Ejendomme hvor købsaftale er underskrevet af både køber og sælger d. 1/7-1998, 
		/// eller tidligere, opnår nedslag i ejendomsværdiskatten.
		/// </summary>
		bool IsKoebtEfter01071998 { get; }

		/// <summary>
		/// Pensionister opnår nedslag på fritidsboliger
		/// </summary>
		bool IsFritidsbolig { get; }
		
		/// <summary>
		/// Ejendomme hvor købsaftale er underskrevet af både køber og sælger d. 1/7-1998, 
		/// eller tidligere, opnår et yderligere nedslag i ejendomsværdiskatten, hvis der 
		/// ikke er tale om ejerlejligheder.
		/// </summary>
		bool IsEjerlejlighed { get; }
	}

	public interface IEjendomsskattelovRegistry
	{

		decimal GetSkattesatsForUnderProgressionsgraense(decimal skatteAar);
		decimal GetSkattesatsForOverProgressionsgraense(decimal skatteAar);

		/// <summary>
		/// Aflæs progressionsgrænsen for beregning af ejendomsværdiskat.
		/// (Ejendomsværdiskatteloven § 5, stk. 1)
		/// </summary>
		decimal GetProgressionsgraense(decimal skatteAar);

		/// <summary>
		/// Aflæs det procentuelle nedslag i ejendomsværdiskatten for ejerboliger erhvervet den 1. juli 1998 eller tidligere.
		/// (Ejendomsværdiskatteloven § 6, stk. 1)
		/// </summary>
		decimal GetNedslagssatsForParagraf6(decimal skatteAar);

		/// <summary>
		/// Aflæs det maksimale nedslag i ejendomsværdiskatten for ejerboliger erhvervet den 1. juli 1998 eller tidligere.
		/// (Ejendomsværdiskatteloven § 7, stk. 1)
		/// </summary>
		decimal GetMaksimaltNedslagForParagraf7(decimal skatteAar);

		/// <summary>
		/// Aflæs det procentuelle nedslag i ejendomsværdiskatten for ejerboliger erhvervet den 1. juli 1998 eller tidligere.
		/// (Ejendomsværdiskatteloven § 7, stk. 1)
		/// </summary>
		decimal GetNedslagssatsForParagraf7(decimal skatteAar);
		
	}
}