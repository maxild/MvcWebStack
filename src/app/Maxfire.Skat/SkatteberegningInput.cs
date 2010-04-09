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
		/// Bidrag og præmier til privattegnede pensionsordninger med løbende udbetalinger og ratepension 
		/// samt privattegnet kapitalpension dog højest 43.100 kr. i 2007.
		/// </summary>
		/// 
		//TODO: Rename to PrivatPension el.lign.
		//TODO: Skal slettes idet den kun benyttes til opgørelse af den personlige indkomst (OBS: Benyttes ved beregning af beskæftigelsefradrag)
		public decimal Pension
		{
			get { return PersonligIndkomst.Fradrag.PrivatPension.IAlt; }
		}

		/// <summary>
		/// Indbetalinger til arbejdsgiver- og privattegnet kapitalpension. 
		/// </summary>
		/// <remarks>
		/// For arbejdsgiverpension skal det være efter fradrag af 8% arbejdsmarkedsbidrag.
		/// 
		/// Fradrages i personlig indkomst og tillægges igen(!) ved beregning af topskat
		/// 
		/// Må højest udgøre 46.000 kr i 2010 ()
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