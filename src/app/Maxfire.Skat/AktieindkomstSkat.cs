using System;

namespace Maxfire.Skat
{
	// TODO: Mangler sambeskatningseffekter og skattefradrag for negativ aktieindkomst

	/// <summary>
	/// Det laveste trin beskattes med 28 pct i 2009 og 2010.
	/// </summary>
	public class AktieindkomstskatLavesteTrinBeregner
	{
		private readonly ISkattelovRegistry _skattelovRegistry;

		public AktieindkomstskatLavesteTrinBeregner(ISkattelovRegistry skattelovRegistry)
		{
			_skattelovRegistry = skattelovRegistry;
		}

		public ValueTuple<decimal> BeregnSkat(ValueTuple<PersonligeBeloeb> indkomster, int skatteAar)
		{
			return indkomster.Map(indkomst => BeregnSkat(indkomst, skatteAar));
		}

		public decimal BeregnSkat(PersonligeBeloeb indkomst, int skatteAar)
		{
			decimal beloebUnderLavesteProgressionsgraense 
				= Math.Min(_skattelovRegistry.GetAktieIndkomstLavesteProgressionsgraense(skatteAar), indkomst.AktieIndkomst).NonNegative();
			return _skattelovRegistry.GetAktieIndkomstLavesteSkattesats(skatteAar) * beloebUnderLavesteProgressionsgraense;
		}
	}

	/// <summary>
	/// Det mellemste trin beskattes med 43 pct i 2009, og 42 pct i 2010.
	/// </summary>
	public class AktieindkomstskatMellemsteTrinBeregner
	{
		private readonly ISkattelovRegistry _skattelovRegistry;

		public AktieindkomstskatMellemsteTrinBeregner(ISkattelovRegistry skattelovRegistry)
		{
			_skattelovRegistry = skattelovRegistry;
		}

		public ValueTuple<decimal> BeregnSkat(ValueTuple<PersonligeBeloeb> indkomster, int skatteAar)
		{
			return indkomster.Map(indkomst => BeregnSkat(indkomst, skatteAar));
		}

		public decimal BeregnSkat(PersonligeBeloeb indkomst, int skatteAar)
		{
			decimal beloebUnderProgressionsgraense 
				= Math.Min(_skattelovRegistry.GetAktieIndkomstHoejesteProgressionsgraense(skatteAar),
															  indkomst.AktieIndkomst);
			decimal beloebOverLavesteProgressionsgraenseOgUnderHoejesteProgressionsgraense 
				= (beloebUnderProgressionsgraense - _skattelovRegistry.GetAktieIndkomstLavesteProgressionsgraense(skatteAar)).NonNegative();
			return _skattelovRegistry.GetAktieIndkomstMellemsteSkattesats(skatteAar) * beloebOverLavesteProgressionsgraenseOgUnderHoejesteProgressionsgraense;
		}
	}

	/// <summary>
	/// Det højeste trin er beskattes med 45 pct. i 2009, og dette trin bortfalder i 2010.
	/// </summary>
	public class AktieindkomstskatHoejesteTrinBeregner
	{
		private readonly ISkattelovRegistry _skattelovRegistry;

		public AktieindkomstskatHoejesteTrinBeregner(ISkattelovRegistry skattelovRegistry)
		{
			_skattelovRegistry = skattelovRegistry;
		}

		public ValueTuple<decimal> BeregnSkat(ValueTuple<PersonligeBeloeb> indkomster, int skatteAar)
		{
			return indkomster.Map(indkomst => BeregnSkat(indkomst, skatteAar));
		}

		public decimal BeregnSkat(PersonligeBeloeb indkomst, int skatteAar)
		{
			decimal beloebOverHoejesteProgressionsgraense 
				= (indkomst.AktieIndkomst - _skattelovRegistry.GetAktieIndkomstHoejesteProgressionsgraense(skatteAar)).NonNegative();
			return _skattelovRegistry.GetAktieIndkomstHoejesteSkattesats(skatteAar) * beloebOverHoejesteProgressionsgraense;
		}
	}
}