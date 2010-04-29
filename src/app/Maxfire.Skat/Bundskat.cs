using System.Linq;

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
		public ValueTupple<decimal> BeregnGrundlag(ValueTupple<Indkomster> input)
		{
			var personligIndkomst = input.Map(x => x.PersonligIndkomst);
			var nettoKapitalIndkomst = input.Map(x => x.NettoKapitalIndkomst);

			// §6, stk 3: Hvis en gift person har negativ nettokapitalindkomst, modregnes dette beløb 
			// i den anden ægtefælles positive kapitalindkomst inden beregning af bundskatten.
			var samletNettoKapitalIndkomst = nettoKapitalIndkomst.NedbringPositivtMedEvtNegativt();

			return personligIndkomst + (+samletNettoKapitalIndkomst);
		}
	}
}