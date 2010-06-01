namespace Maxfire.Skat
{
	/// <summary>
	/// Input der varierer pr. person
	/// </summary>
	public class PersonligeBeloeb
	{
		/// <summary>
		/// Et fremført underskud i skattepligtig indkomst fra tidligere indkomstår.
		/// </summary>
		public decimal FremfoertUnderskudSkattepligtigIndkomst { get; set; }

		/// <summary>
		/// Modregning af underskud i skattepligtig indkomst i årets positive skattepligtige indkomst.
		/// </summary>
		/// <remarks>
		/// Der er tre årsager til modregning i den positive skattepligtige indkomst:
		///    1) Årets underskud, der er overført og modregnet i ægtefælles positive skattepligtige indkomst.
		///    2) Fremført underskud, der er modregnet i egen positive skattepligtige indkomst.
		///    3) Fremført underskud, der er overført og modregnet i ægtefælles positive skattepligtige indkomst.
		/// </remarks>
		public decimal ModregnetUnderskudSkattepligtigIndkomst { get; set; }

		/// <summary>
		/// Underskud i skattepligtig indkomst til fremførsel i efterfølgende skatteår.
		/// </summary>
		public decimal UnderskudSkattepligtigIndkomstTilFremfoersel { get; set; }
		
		public decimal AMIndkomst { get; set; }

		public decimal PersonligIndkomst { get; set; }

		/// <summary>
		/// Skattepligtig indkomst efter modregning af underskud.
		/// </summary>
		/// <remarks>
		/// Først efter modregning og fremførsel af underskud er udført vil SkattepligtigIndkomst indeholde værdien af
		/// et fremført underskud. Derfor er det yderst vigtigt at modregning og fremførsel af underskud sker inden 
		/// beregning af skatter baseret på den skattepligtige indkomst.
		/// </remarks>
		public decimal SkattepligtigIndkomst
		{
			get 
			{
				// Note: ModregnetUnderskudSkattepligtigIndkomst indeholder den del årets og fremførst underskud, der kan rummes i positiv skattepligtig indkomst
				return PersonligIndkomst + NettoKapitalIndkomst - LigningsmaessigeFradrag - ModregnetUnderskudSkattepligtigIndkomst; 
			}
		}

		public decimal NettoKapitalIndkomst { get; set; }

		// Note: Ingen sondring mellem tab/gevinst på aktier og udbytte, og dermed ingen justering af indeholdt udbytte på årsopgørelsen
		/// <summary>
		/// Aktieindkomst uden indeholdt udbytteskat.
		/// </summary>
		public decimal AktieIndkomst { get; set; }

		public decimal LigningsmaessigeFradrag { get; set; }

		/// <summary>
		/// PBL § 16, stk. 1, omhandler indskud til kapitalpensionsordninger, kapitalforsikring 
		/// og bidrag til supplerende engangsydelser fra pensionskasser, hvor der maksimalt kan 
		/// indbetales op til 46.000 kr. (2009 og 2010).
		/// </summary>
		public decimal KapitalPensionsindskud { get; set; }
	}
}