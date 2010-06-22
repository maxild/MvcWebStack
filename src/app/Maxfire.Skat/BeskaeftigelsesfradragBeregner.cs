namespace Maxfire.Skat
{
	public class BeskaeftigelsesfradragBeregner
	{
		private readonly ISkattelovRegistry _skattelovRegistry;

		public BeskaeftigelsesfradragBeregner(ISkattelovRegistry skattelovRegistry)
		{
			_skattelovRegistry = skattelovRegistry;
		}

		public ValueTuple<decimal> BeregnFradrag(ValueTuple<PersonligeBeloeb> indkomster, int skatteAar)
		{
			decimal sats = _skattelovRegistry.GetBeskaeftigelsesfradragSats(skatteAar);
			decimal grundbeloeb = _skattelovRegistry.GetBeskaeftigelsesfradragGrundbeloeb(skatteAar);
			return BeregnFradrag(indkomster, sats, grundbeloeb);
		}

// ReSharper disable MemberCanBeMadeStatic.Global
		public ValueTuple<decimal> BeregnFradrag(ValueTuple<PersonligeBeloeb> indkomster, decimal sats, decimal grundbeloeb)
// ReSharper restore MemberCanBeMadeStatic.Global
		{
			return indkomster.Map(x => sats * x.AMIndkomst).Loft(grundbeloeb);
		}
	}
}