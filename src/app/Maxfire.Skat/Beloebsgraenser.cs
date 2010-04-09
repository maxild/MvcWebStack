namespace Maxfire.Skat
{
	// Dette er de relevante grænser og satser der skal med pr. skatteår
	//
	// Satser:
	// 1) sundhedsbidrag
	// 2) bundskat
	// 3) topskat
	// 4) skatteloft
	// 5) beskæftigelsesfradrag
	// 6) aktieindkomstskat (lav og høj)
	// 7) grøncheck-aftrapningssats
	//
	// 8) arbejdsmarkedsbidrag
	// 9) 
	//
	// Beløbsgrænser (Note: Mange beløbsgrænser er faste, da reguleringen er ukendt, og må svare til lønudviklingen):
	//
	// 1) MaksBeskæftigelsesfradrag
	// 2) Topskattegrænse
	// 3) BundfradragForPositivNettokapitalindkomstITopskat
	// 4) Multimedie
	// 5) MaksIndskudPåRatepension
	// 6) MaksFradragEfterRejseReglerne
	// 7) BundgrænseForOmlægningAfnegativNettokapitalindkomst (reguleres ikke, 50000 er 2012 niveau)
	// 8) GrønCheck
	// 9) GrønCheckBørn
	// 10) AftrapningsgrænseForGrønCheck
	//
	// 11) Personfradrag
	// 12) BundgrænseFradragForLønmodtagerUdgifter (grundbeløb)
	// 13) BundfradragKompensationBundlettelse (over/under 18 år)
	// 14) BundfradragKompensationMellemlettelse
	// 15) BundfradragKompensationToplettelse
	// 16) BundfradragKompensationBeskæftigelsesfradragLettelse
	// 16) BundfradragKompensationPersonfradragLettelse
	//
	// TODO: Aktieindkomst udenfor nu...men
	//
	// 1) Progressionsgrænse for beskatning af aktieindkomst
	//
	// TODO: Ejedomsværdi skatter holde uden for...men
	// 1) EVGrænse (progressionsgrænse)
	// 2) Lav sats
	// 3) Høj sats
	// 4) LilleNedslag
	// 5) Stortnedslag
	// 6) Pensionistnedslag
	// 7) MaksNedslag (2000 og 6000)
	// 8) FradragsGrænse (1200)
	// 9) Plus en masse mere, som jeg ikke forstå i TFS beregningen.

	/// <summary>
	/// Beløbsgrænser i skattelovgivningen der reguleres efter personskattelovens § 20
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
	/// Mht skatteår
	/// </summary>
	public class BeloebsgraenserRegistry
	{
		
	}
}