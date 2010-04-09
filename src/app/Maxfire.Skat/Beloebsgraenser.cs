namespace Maxfire.Skat
{
	// Dette er de relevante gr�nser og satser der skal med pr. skatte�r
	//
	// Satser:
	// 1) sundhedsbidrag
	// 2) bundskat
	// 3) topskat
	// 4) skatteloft
	// 5) besk�ftigelsesfradrag
	// 6) aktieindkomstskat (lav og h�j)
	// 7) gr�ncheck-aftrapningssats
	//
	// 8) arbejdsmarkedsbidrag
	// 9) 
	//
	// Bel�bsgr�nser (Note: Mange bel�bsgr�nser er faste, da reguleringen er ukendt, og m� svare til l�nudviklingen):
	//
	// 1) MaksBesk�ftigelsesfradrag
	// 2) Topskattegr�nse
	// 3) BundfradragForPositivNettokapitalindkomstITopskat
	// 4) Multimedie
	// 5) MaksIndskudP�Ratepension
	// 6) MaksFradragEfterRejseReglerne
	// 7) Bundgr�nseForOml�gningAfnegativNettokapitalindkomst (reguleres ikke, 50000 er 2012 niveau)
	// 8) Gr�nCheck
	// 9) Gr�nCheckB�rn
	// 10) Aftrapningsgr�nseForGr�nCheck
	//
	// 11) Personfradrag
	// 12) Bundgr�nseFradragForL�nmodtagerUdgifter (grundbel�b)
	// 13) BundfradragKompensationBundlettelse (over/under 18 �r)
	// 14) BundfradragKompensationMellemlettelse
	// 15) BundfradragKompensationToplettelse
	// 16) BundfradragKompensationBesk�ftigelsesfradragLettelse
	// 16) BundfradragKompensationPersonfradragLettelse
	//
	// TODO: Aktieindkomst udenfor nu...men
	//
	// 1) Progressionsgr�nse for beskatning af aktieindkomst
	//
	// TODO: Ejedomsv�rdi skatter holde uden for...men
	// 1) EVGr�nse (progressionsgr�nse)
	// 2) Lav sats
	// 3) H�j sats
	// 4) LilleNedslag
	// 5) Stortnedslag
	// 6) Pensionistnedslag
	// 7) MaksNedslag (2000 og 6000)
	// 8) FradragsGr�nse (1200)
	// 9) Plus en masse mere, som jeg ikke forst� i TFS beregningen.

	/// <summary>
	/// Bel�bsgr�nser i skattelovgivningen der reguleres efter personskattelovens � 20
	/// </summary>
	public class Beloebsgraenser
	{
		public Beloebsgraenser()
		{
			Topskat = new TopskatBeloebsgraenser();
		}

		public PersonfradragBeloebsgraenser Personfradrag { get; set; }
		public TopskatBeloebsgraenser Topskat { get; private set; }
	}

	public class PersonfradragBeloebsgraenser
	{
		public decimal Fyldt18Aar { get; set; }
		public decimal Under18Aar { get; set; }
	}

	public class TopskatBeloebsgraenser
	{
		public decimal Progressionsgraense { get; set; }
		public decimal BundfradragForPositivNettokapitalindkomst { get; set; }
	}

	public class AktieindkomstBeloebsgraenser
	{
		public decimal Progressionsgraense { get; set; }
	}

	public class Beskaeftigelsesfradrag
	{
		public decimal Sats { get; set; }
		public decimal Loft { get; set; }
	}

	/// <summary>
	/// Mht skatte�r
	/// </summary>
	public class BeloebsgraenserRegistry
	{
		
	}
}