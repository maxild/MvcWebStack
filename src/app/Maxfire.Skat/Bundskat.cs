namespace Maxfire.Skat
{
	/// <summary>
	/// §6, stk 1: Bundskattegrundlaget er den personlige indkomst med tillæg af positiv nettokapitalindkomst. 
	/// </summary>
	/// <remarks>
	/// Negativ nettolapitalindkomst og ligningsmæssige fradrag kan altså ikke fratrækkes i grundlaget for bundskat.	
	/// </remarks>
	public class BundskatGrundlagBeregner
	{
		/// <summary>
		/// Beregn bundskattegrundlaget under hensyn til evt. modregnet negativ  negativ nettokapitalindkomst.
		/// </summary>
		public ValueTuple<decimal> BeregnGrundlag(ValueTuple<PersonligeBeloeb> input)
		{
			var personligIndkomst = input.Map(x => x.PersonligIndkomst);
			var nettoKapitalIndkomst = input.Map(x => x.NettoKapitalIndkomst);

			// §6, stk 3: Hvis en gift person har negativ nettokapitalindkomst, modregnes dette beløb 
			// i den anden ægtefælles positive kapitalindkomst inden beregning af bundskatten.
			var nettoKapitalIndkomstEfterModregning = nettoKapitalIndkomst.NedbringPositivtMedEvtNegativt();

			return personligIndkomst + (+nettoKapitalIndkomstEfterModregning);
		}
	}

	public class BundskatBeregner
	{
		public ValueTuple<decimal> BeregnSkat(ValueTuple<PersonligeBeloeb> indkomster)
		{
			var grundlagBeregner = new BundskatGrundlagBeregner();
			var grundlag = grundlagBeregner.BeregnGrundlag(indkomster);
			return Constants.Bundskattesats * grundlag;
		}
	}
}