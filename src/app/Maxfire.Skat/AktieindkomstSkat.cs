using System;

namespace Maxfire.Skat
{
	// TODO: Mangler sambeskatningseffekter og skattefradrag for negativ aktieindkomst

	/// <summary>
	/// Det laveste trin beskattes med 28 pct i 2009 og 2010.
	/// </summary>
	public class AktieindkomstskatLavesteTrinBeregner
	{
		public ValueTuple<decimal> BeregnSkat(ValueTuple<PersonligeBeloeb> indkomster)
		{
			return indkomster.Map(indkomst => BeregnSkat(indkomst));
		}

		public decimal BeregnSkat(PersonligeBeloeb indkomst)
		{
			decimal beloebUnderLavesteProgressionsgraense 
				= Math.Min(Constants.AktieIndkomstLavesteProgressionsgraense, indkomst.AktieIndkomst).NonNegative();
			return Constants.AktieIndkomstLavesteSkattesats * beloebUnderLavesteProgressionsgraense;
		}
	}

	/// <summary>
	/// Det mellemste trin beskattes med 43 pct i 2009, og 42 pct i 2010.
	/// </summary>
	public class AktieindkomstskatMellemsteTrinBeregner
	{
		public ValueTuple<decimal> BeregnSkat(ValueTuple<PersonligeBeloeb> indkomster)
		{
			return indkomster.Map(indkomst => BeregnSkat(indkomst));
		}

		public decimal BeregnSkat(PersonligeBeloeb indkomst)
		{
			decimal beloebUnderProgressionsgraense = Math.Min(Constants.AktieIndkomstHoejesteProgressionsgraense,
															  indkomst.AktieIndkomst);
			decimal beloebOverLavesteProgressionsgraenseOgUnderHoejesteProgressionsgraense 
				= (beloebUnderProgressionsgraense - Constants.AktieIndkomstLavesteProgressionsgraense).NonNegative();
			return Constants.AktieIndkomstMellemsteSkattesats * beloebOverLavesteProgressionsgraenseOgUnderHoejesteProgressionsgraense;
		}
	}

	/// <summary>
	/// Det højeste trin er beskattes med 45 pct. i 2009, og dette trin bortfalder i 2010.
	/// </summary>
	public class AktieindkomstskatHoejesteTrinBeregner
	{
		public ValueTuple<decimal> BeregnSkat(ValueTuple<PersonligeBeloeb> indkomster)
		{
			return indkomster.Map(indkomst => BeregnSkat(indkomst));
		}

		public decimal BeregnSkat(PersonligeBeloeb indkomst)
		{
			decimal beloebOverHoejesteProgressionsgraense 
				= (indkomst.AktieIndkomst - Constants.AktieIndkomstHoejesteProgressionsgraense).NonNegative();
			return Constants.AktieIndkomstHoejesteSkattesats * beloebOverHoejesteProgressionsgraense;
		}
	}
}