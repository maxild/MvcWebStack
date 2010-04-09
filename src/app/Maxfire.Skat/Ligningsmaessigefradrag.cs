namespace Maxfire.Skat
{
	public class Ligningsmaessigefradrag
	{
		/// <summary>
		/// Rubrik 51: Befordring
		/// </summary>
		public decimal Befordring { get; set; }

		/// <summary>
		/// Rubrik 52: Fagligt kontingent, bidrag til A-kasse, efterlønsordning og fleksydelse. 
		/// Ansattes årlige betaling for pc-ordning (højst 3.500 kroner)
		/// </summary>
		public decimal Faglig { get; set; }

		/// <summary>
		/// Rubrik 53: Øvrige lønmodtagerudgifter - 5.500 kroner
		/// </summary>
		/// <remarks>
		/// Det kan være beklædning, arbejdsværelse, dobbelt husførelse, faglitteratur, flytteudgifter, 
		/// forskudt arbejdstid m.m. efter gældende regler.
		/// </remarks>
		public decimal OevrigeLoenmodtagerUdgifter { get; set; }

		/// <summary>
		/// Rubrik 54: Standardfradrag for børnedagplejere og fiskere. Fradrag vedr. 
		/// DIS-indkomst (begrænset fart). Handicappede og kronisk syges befordringsudgifter.
		/// </summary>
		// TODO: Skal måske opslittes i mere logiske klumper
		public decimal Rubrik54 { get; set; }

		/// <summary>
		/// Rubrik 56: Underholdsbidrag til tidligere ægtefælle, børnebidrag og aftægtsforpligtelser mv.
		/// </summary>
		public decimal Underholdsbidrag { get; set; }

		/// <summary>
		/// Rubrik 57: Fradragsberettigede indskud på etableringskonto.
		/// </summary>
		public decimal Etableringskonto { get; set; }

		/// <summary>
		/// Gaver til almenvelgørende og almennyttige foreninger, stiftelser og institutioner mm., 
		/// hvis midler anvendes til fordel for en større kreds af personer.
		/// </summary>
		/// <remarks>
		/// Fradraget kan kun indrømmes for det beløb, hvormed gaverne tilsammen overstiger 500 kr. og 
		/// kan højst udgøre 13.700 kr. i 2007 (14.000 kr. i 2008, 14500 i 2010???)
		/// </remarks>
		public decimal GaverTilForeninger { get; set; }

		/// <summary>
		/// Fradrag for rejseudgifter til kost, småfornødenheder og/eller logi.
		/// </summary>
		/// <remarks>
		/// Der er fra indkomståret 2010 kommet nye regler om, hvor meget du i alt kan fradrage for 
		/// rejseudgifter. Hvis du som lønmodtager benytter muligheden for at trække udgifter til 
		/// kost og logi fra med standardsatserne, eller hvis du vælger at tage fradrag for de faktisk 
		/// dokumenterede udgifter, kan du højest gøre dette med 50.000 kr. om året.
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