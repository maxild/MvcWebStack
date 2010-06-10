namespace Maxfire.Skat
{
	public class AMBidragBeregner
	{
		private readonly ISkattelovRegistry _skattelovRegistry;

		public AMBidragBeregner(ISkattelovRegistry skattelovRegistry)
		{
			_skattelovRegistry = skattelovRegistry;
		}

		public ValueTuple<decimal> BeregnSkat(ValueTuple<PersonligeBeloeb> indkomster, int skatteAar)
		{
			var amIndkomst = indkomster.Map(x => x.AMIndkomst);
			return _skattelovRegistry.GetAMBidragSkattesats(skatteAar) * amIndkomst;
		}
	}
}