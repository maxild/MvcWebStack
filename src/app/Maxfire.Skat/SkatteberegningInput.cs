namespace Maxfire.Skat
{
	public class SkatteberegningInput
	{
		public SkatteberegningInput()
		{
			PersonligIndkomst = new PersonligIndkomst();
			Ligningsmaessigefradrag = new Ligningsmaessigefradrag();
			NettoKapitalIndkomst = new NettoKapitalIndkomst();
			ArbejdsgiverPension = new Pensionsbidrag();
		}

		public PersonligIndkomst PersonligIndkomst { get; private set; }

		public NettoKapitalIndkomst NettoKapitalIndkomst { get; private set; }

		public Ligningsmaessigefradrag Ligningsmaessigefradrag { get; private set; }

		public Pensionsbidrag ArbejdsgiverPension { get; private set; }

		/// <summary>
		/// Bidrag og pr�mier til privattegnede pensionsordninger med l�bende udbetalinger og ratepension 
		/// samt privattegnet kapitalpension dog h�jest 43.100 kr. i 2007.
		/// </summary>
		/// 
		//TODO: Rename to PrivatPension el.lign.
		//TODO: Skal slettes idet den kun benyttes til opg�relse af den personlige indkomst (OBS: Benyttes ved beregning af besk�ftigelsefradrag)
		public decimal Pension
		{
			get { return PersonligIndkomst.Fradrag.PrivatPension.IAlt; }
		}

		/// <summary>
		/// Indbetalinger til arbejdsgiver- og privattegnet kapitalpension. 
		/// </summary>
		/// <remarks>
		/// For arbejdsgiverpension skal det v�re efter fradrag af 8% arbejdsmarkedsbidrag.
		/// 
		/// Fradrages i personlig indkomst og till�gges igen(!) ved beregning af topskat
		/// 
		/// M� h�jest udg�re 46.000 kr i 2010 ()
		/// </remarks>
		//TODO: Rename to SamletKapitalPensionIndbetalinger el.lign.
		public decimal KapitalPension
		{
			get { return PersonligIndkomst.Fradrag.PrivatPension.Kapital + ArbejdsgiverPension.Kapital; }
		}

		public decimal UdbytteIndkomst { get; set; }

		/// <summary>
		/// kursgevinst/tab til beskatning.
		/// </summary>
		public decimal Kursgevinst { get; set; } // TODO: ????

		public decimal Kommuneskattesats { get; set; }

		public decimal Kirkeskattesats { get; set; }
	}
}