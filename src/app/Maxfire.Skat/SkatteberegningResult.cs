namespace Maxfire.Skat
{
	public class SkatteberegningResult
	{
		public decimal PersonligIndkomst { get; set; }
		public decimal KapitalIndkomst { get; set; }
		public decimal AktieIndkomst { get; set; }
		public decimal SkattepligtigIndkomst { get; set; }

		// TODO: Hvad med størrelsen på de ligningsmæssige fradrag

		// TODO: Hvad med topskattegrundlag?

		// Er dette person fradragene
		public decimal SkattevaerdiFradragKommuneskat { get; set; }
		public decimal SkattevaerdiFradragBundskat { get; set; }
		public decimal SkattevaerdiFradragSundhedsbidrag { get; set; }
		// etc...

		public decimal SkattevaerdiSkatteloft { get; set; }

		public decimal GroenCheck { get; set; }

		public decimal Arbejdsmarkedsbidrag { get; set; }
		public decimal Kommuneskat { get; set; }
		public decimal Bundskat { get; set; }
		public decimal Topskat { get; set; }
		public decimal Sundhedsbidrag { get; set; }

		//public decimal SamletIndkomstskat
		//{
		//    get { return 0; }
		//}

		// TODO: Mangler
		//  - Aktieskat
		//  - Ejendomsværdiskat
		//  - Grundskyld
	}
}