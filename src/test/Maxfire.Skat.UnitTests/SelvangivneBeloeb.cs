namespace Maxfire.Skat.UnitTests
{
	public class SelvangivneBeloeb : AbstractSelvangivneBeloeb
	{
		public decimal Bruttoloen { get; set; }
		public decimal AtpEgetBidrag { get; set; }
		public decimal PrivatTegnetKapitalPensionsindskud { get; set; }
		public decimal Renteindt { get; set; }
		public decimal Renteudg { get; set; }
		public decimal AndenKapitalIndkomst { get; set; }
		public decimal LigningsmaessigeFradrag { get; set; }

		public override decimal PersonligIndkomstAMIndkomst
		{
			get { return Bruttoloen; }
		}

		public override decimal PersonligIndkomstEjAMIndkomst
		{
			get { return 0m; }
		}

		public override decimal FradragPersonligIndkomst
		{
			get { return base.FradragPersonligIndkomst + AtpEgetBidrag; }
		}

		public override decimal KapitalIndkomst
		{
			get { return Renteindt + AndenKapitalIndkomst; }
		}

		public override decimal FradragKapitalIndkomst
		{
			get { return Renteudg; }
		}

		public override decimal LigningsmaessigeFradragMinusBeskaeftigelsesfradrag
		{
			get { return LigningsmaessigeFradrag; }
		}

		public override decimal KapitalPensionsindskud
		{
			get { return PrivatTegnetKapitalPensionsindskud; }
		}

		public override decimal PrivatTegnetPensionsindskud
		{
			get { return PrivatTegnetKapitalPensionsindskud; }
		}
	}
}