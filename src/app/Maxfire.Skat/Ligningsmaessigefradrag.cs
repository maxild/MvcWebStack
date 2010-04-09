namespace Maxfire.Skat
{
	public class Ligningsmaessigefradrag
	{
		/// <summary>
		/// Rubrik 51: Befordring
		/// </summary>
		public decimal Befordring { get; set; }

		/// <summary>
		/// Rubrik 52: Fagligt kontingent, bidrag til A-kasse, efterl�nsordning og fleksydelse. 
		/// Ansattes �rlige betaling for pc-ordning (h�jst 3.500 kroner)
		/// </summary>
		public decimal Faglig { get; set; }

		/// <summary>
		/// Rubrik 53: �vrige l�nmodtagerudgifter - 5.500 kroner
		/// </summary>
		/// <remarks>
		/// Det kan v�re bekl�dning, arbejdsv�relse, dobbelt husf�relse, faglitteratur, flytteudgifter, 
		/// forskudt arbejdstid m.m. efter g�ldende regler.
		/// </remarks>
		public decimal OevrigeLoenmodtagerUdgifter { get; set; }

		/// <summary>
		/// Rubrik 54: Standardfradrag for b�rnedagplejere og fiskere. Fradrag vedr. 
		/// DIS-indkomst (begr�nset fart). Handicappede og kronisk syges befordringsudgifter.
		/// </summary>
		// TODO: Skal m�ske opslittes i mere logiske klumper
		public decimal Rubrik54 { get; set; }

		/// <summary>
		/// Rubrik 56: Underholdsbidrag til tidligere �gtef�lle, b�rnebidrag og aft�gtsforpligtelser mv.
		/// </summary>
		public decimal Underholdsbidrag { get; set; }

		/// <summary>
		/// Rubrik 57: Fradragsberettigede indskud p� etableringskonto.
		/// </summary>
		public decimal Etableringskonto { get; set; }

		/// <summary>
		/// Gaver til almenvelg�rende og almennyttige foreninger, stiftelser og institutioner mm., 
		/// hvis midler anvendes til fordel for en st�rre kreds af personer.
		/// </summary>
		/// <remarks>
		/// Fradraget kan kun indr�mmes for det bel�b, hvormed gaverne tilsammen overstiger 500 kr. og 
		/// kan h�jst udg�re 13.700 kr. i 2007 (14.000 kr. i 2008, 14500 i 2010???)
		/// </remarks>
		public decimal GaverTilForeninger { get; set; }

		/// <summary>
		/// Fradrag for rejseudgifter til kost, sm�forn�denheder og/eller logi.
		/// </summary>
		/// <remarks>
		/// Der er fra indkomst�ret 2010 kommet nye regler om, hvor meget du i alt kan fradrage for 
		/// rejseudgifter. Hvis du som l�nmodtager benytter muligheden for at tr�kke udgifter til 
		/// kost og logi fra med standardsatserne, eller hvis du v�lger at tage fradrag for de faktisk 
		/// dokumenterede udgifter, kan du h�jest g�re dette med 50.000 kr. om �ret.
		/// </remarks>
		public decimal Rejseudgifter { get; set; }

		public decimal IAlt
		{
			get
			{
				return Befordring +
				       Faglig +
				       OevrigeLoenmodtagerUdgifter +
				       Rubrik54 +
				       Underholdsbidrag +
				       Etableringskonto +
				       GaverTilForeninger +
				       Rejseudgifter;
			}
		}
	}
}