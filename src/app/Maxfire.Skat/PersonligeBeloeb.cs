namespace Maxfire.Skat
{
	/// <summary>
	/// Input der varierer pr. person
	/// </summary>
	public class PersonligeBeloeb
	{
		public decimal FremfoertUnderskudSkattepligtigIndkomst { get; set; }

		/// <summary>
		/// Underskud i skattepligtig indkomst til fremførsel i efterfølgende skatteår.
		/// </summary>
		public decimal UnderskudSkattepligtigIndkomstFremfoersel { get; set; }
		
		/// <summary>
		/// Underskud i skattepligtig indkomst, der er blevet modregnet i årets skatter.
		/// </summary>
		public decimal UnderskudSkattepligtigIndkomstModregning { get; set; }

		public decimal AMIndkomst { get; set; }

		public decimal PersonligIndkomst { get; set; }

		// TODO: Sondring mellem årets underskud i skattepligtige indkomst, og fremført underskud i skattepligtig indkomst
		public decimal SkattepligtigIndkomst
		{
			get { return PersonligIndkomst + NettoKapitalIndkomst - LigningsmaessigeFradrag - FremfoertUnderskudSkattepligtigIndkomst; }
		}

		public decimal NettoKapitalIndkomst { get; set; }

		public decimal AktieIndkomst { get; set; }

		public decimal LigningsmaessigeFradrag { get; set; }

		/// <summary>
		/// PBL § 16, stk. 1, omhandler indskud til kapitalpensionsordninger, kapitalforsikring 
		/// og bidrag til supplerende engangsydelser fra pensionskasser, hvor der maksimalt kan 
		/// indbetales op til 46.000 kr. (2009 og 2010).
		/// </summary>
		public decimal KapitalPensionsindskud { get; set; }
	}

	/// <summary>
	/// Ægtefæller kan f.eks have forskellige skatteprocenter, hvis de bor i hver sin kommune, 
	/// men de kan være samlevende i skattemæssig forstand, eller hvis kun den ene er medlem af 
	/// folkekirken, eller de er blevet gift og flyttet sammen efter 5. september året forud 
	/// for indkomståret.
	/// </summary>
	public class KommunaleSatser
	{
		public decimal Kommuneskattesats { get; set; }
		public decimal Kirkeskattesats { get; set; }
		public decimal KommuneOgKirkeskattesats
		{
			get { return Kommuneskattesats + Kirkeskattesats; }
		}
	}
}