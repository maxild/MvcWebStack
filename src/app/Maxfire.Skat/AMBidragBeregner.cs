namespace Maxfire.Skat
{
	public class AMBidragBeregner
	{
		public ValueTuple<decimal> BeregnSkat(ValueTuple<PersonligeBeloeb> indkomster)
		{
			var amIndkomst = indkomster.Map(x => x.AMIndkomst);
			return Constants.AMBidragsats * amIndkomst;
		}
	}
}