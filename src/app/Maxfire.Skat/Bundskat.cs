namespace Maxfire.Skat
{
	public class BundskatBeregner
	{
		private readonly ISkattelovRegistry _skattelovRegistry;

		public BundskatBeregner(ISkattelovRegistry skattelovRegistry)
		{
			_skattelovRegistry = skattelovRegistry;
		}

		public ValueTuple<decimal> BeregnSkat(ValueTuple<PersonligeBeloeb> indkomster, int skatteAar)
		{
			var grundlag = BeregnGrundlag(indkomster);
			return _skattelovRegistry.GetBundSkattesats(skatteAar) * grundlag;
		}

		/// <summary>
		/// Beregn bundskattegrundlaget under hensyn til evt. modregnet negativ  negativ nettokapitalindkomst.
		/// </summary>
		public ValueTuple<decimal> BeregnGrundlag(ValueTuple<PersonligeBeloeb> input)
		{
			var personligIndkomst = input.Map(x => x.PersonligIndkomstSkattegrundlag);
			var nettoKapitalIndkomst = input.Map(x => x.NettoKapitalIndkomstSkattegrundlag);

			// §6, stk 3: Hvis en gift person har negativ nettokapitalindkomst, modregnes dette beløb 
			// i den anden ægtefælles positive kapitalindkomst inden beregning af bundskatten.
			var nettoKapitalIndkomstEfterModregning = nettoKapitalIndkomst.NedbringPositivtMedEvtNegativt();

			return personligIndkomst + (+nettoKapitalIndkomstEfterModregning);
		}
	}
}