namespace Maxfire.Skat
{
	public class NettoKapitalIndkomst
	{
		public NettoKapitalIndkomst()
		{
			Indtaegter = new KapitalIndkomstIndtaegter();
			Udgifter = new KapitalIndkomstUdgifter();
		}

		public KapitalIndkomstIndtaegter Indtaegter { get; private set; }
		public KapitalIndkomstUdgifter Udgifter { get; private set; }

		public decimal IAlt
		{
			get { return Indtaegter.IAlt - Udgifter.IAlt; }
		}
	}

	public class KapitalIndkomstIndtaegter
	{
		/// <summary>
		/// Rubrik 31: Renteindt�gter af indest�ende i pengeinstitut, obligationer og pantebreve i 
		/// depot samt udlodning fra obligationsbaserede investeringsforeninger.
		/// </summary>
		public decimal Renteindtaegter { get; set; }

		/// <summary>
		/// Rubrik 33: Reservefondsudlodninger og kapitalv�rdistigninger af pensionsordninger
		/// </summary>
		public decimal Pensionsordninger { get; set; }

		/// <summary>
		/// Rubrik 34: Udlodning fra investeringsselskab/-forening, hvor der er indeholdt udbytteskat
		/// </summary>
		public decimal Udlodninger { get; set; }

		/// <summary>
		/// Rubrik 35: Over- /underskud ved visse skibsprojekter (underskud angives med minus).
		/// Overskud ved anden anpartsvirksomhed.
		/// </summary>
		public decimal SkibsprojekterLoebendeOverskud { get; set; }

		/// <summary>
		/// Rubrik 36: Fortjeneste/tab ved oph�r af visse skibsprojekter (tab angives med minus).
		/// Fortjeneste ved oph�r af anden anpartsvirksomhed
		/// </summary>
		public decimal SkibsprojekterFortjenesteVedOphoer { get; set; }

		//
		/// <summary>
		/// Rubrik 37: Lejeindt�gt ved udleje af hel�rsbolig en del af �ret samt sommerhus- og v�relsesudlejning. 
		/// G�lder kun, hvis du selv ejer boligen.
		/// </summary>
		public decimal BoligUdlejningsindtaegter { get; set; }

		/// <summary>
		/// Rubrik 38: Renter af pantebreve, der ikke er i depot. Gevinst/tab p� bevis i investeringsselskab 
		/// og i udloddende blandet og obligationsbaseret investeringsforening.
		/// </summary>
		public decimal AndreRenteindtaegter { get; set; }

		/// <summary>
		/// Rubrik 39: Anden kapitalindkomst fx finansielle kontrakter og aftaler.
		/// </summary>
		public decimal AndenKapitalIndkomst { get; set; }

		public decimal IAlt
		{
			get
			{
				return Renteindtaegter +
					   Pensionsordninger +
					   Udlodninger +
					   SkibsprojekterLoebendeOverskud +
					   SkibsprojekterFortjenesteVedOphoer +
					   BoligUdlejningsindtaegter +
					   AndreRenteindtaegter +
					   AndenKapitalIndkomst;
			}
		}
	}

	/// <summary>
	/// Fradrag i kapitalindkomsten.
	/// </summary>
	public class KapitalIndkomstUdgifter
	{
		/// <summary>
		/// Rubrik 41: Renteudgifter af g�ld til realkreditinstitutter og reall�nefonde samt 
		/// fradragsberettigede kurstab ved oml�gning af kontantl�n.
		/// </summary>
		public decimal RenteudgifterRealkredit { get; set; }

		/// <summary>
		/// Rubrik 42: Renteudgifter af g�ld til pengeinstitutter, pensionskasser, forsikrings- og 
		/// finansieringsselskaber, kontokortordninger samt af pantebreve i depot.
		/// </summary>
		public decimal RenteudgifterPengeinstitutter { get; set; }

		/// <summary>
		/// Rubrik 43: Renteudgifter af studiel�n fra �konomistyrelsen
		/// </summary>
		public decimal RenteudgifterStudielaan { get; set; }

		/// <summary>
		/// Rubrik 44: Renteudgifter af anden g�ld, herunder af statsgaranterede studiel�n 
		/// i et pengeinstitut samt af pantebreve, der ikke er i depot
		/// </summary>
		public decimal RenteudgifterAndenGaeld { get; set; }

		public decimal IAlt
		{
			get
			{
				return RenteudgifterRealkredit +
					   RenteudgifterPengeinstitutter +
					   RenteudgifterStudielaan +
					   RenteudgifterAndenGaeld;
			}
		}
	}
}