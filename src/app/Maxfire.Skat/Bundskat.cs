using System.Collections.Generic;

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
		public IList<decimal> BeregnGrundlag(IList<Indkomster> input)
		{
			var first = input.First();
				
			if (input.IsIndividual())
			{
				return (first.PersonligIndkomst + first.NettoKapitalIndkomst.NonNegative()).AsIndividual();
			}

			var second = input.Second();

			// §6, stk 3: Hvis en gift person har negativ kapitalindkomst, modregnes dette beløb 
			// i den anden ægtefælles positive kapitalindkomst inden beregning af bundskatten.
			var samletNettoKapitalIndkomst = new ModregningPair(first.NettoKapitalIndkomst, second.NettoKapitalIndkomst);

			decimal firstGrundlag = first.PersonligIndkomst + samletNettoKapitalIndkomst.First;
			decimal secondGrundlag = second.PersonligIndkomst + samletNettoKapitalIndkomst.Second;

			return firstGrundlag.AsMarried(secondGrundlag);			
		}
	}
}