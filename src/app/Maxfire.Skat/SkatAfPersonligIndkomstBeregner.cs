﻿namespace Maxfire.Skat
{
	public class SkatAfPersonligIndkomstBeregner
	{
		public ValueTuple<SkatterAfPersonligIndkomst> BeregnSkat(ValueTuple<PersonligeBeloeb> indkomster, ValueTuple<KommunaleSatser> kommunaleSatser)
		{
			// TODO: Refactor

			var bundskatBeregner = new BundskatBeregner();
			var bundskat = bundskatBeregner.BeregnSkat(indkomster);

			var mellemskatBeregner = new MellemskatBeregner();
			var mellemskat = mellemskatBeregner.BeregnSkat(indkomster);

			var topskatBeregner = new TopskatBeregner();
			var topskat = topskatBeregner.BeregnSkat(indkomster);

			var aktieindkomstskatLavesteTrinBeregner = new AktieindkomstskatLavesteTrinBeregner();
			var aktieindkomstskatLavesteTrin = aktieindkomstskatLavesteTrinBeregner.BeregnSkat(indkomster);

			var aktieindkomstskatMellemsteTrinBeregner = new AktieindkomstskatMellemsteTrinBeregner();
			var aktieindkomstskatMellemsteTrin = aktieindkomstskatMellemsteTrinBeregner.BeregnSkat(indkomster);

			var aktieindkomstskatHoejesteTrinBeregner = new AktieindkomstskatHoejesteTrinBeregner();
			var aktieindkomstskatHoejesteTrin = aktieindkomstskatHoejesteTrinBeregner.BeregnSkat(indkomster);

			return bundskat.Map(index => new SkatterAfPersonligIndkomst
			                             	{
			                             		Bundskat = bundskat[index],
			                             		Mellemskat = mellemskat[index],
			                             		Topskat = topskat[index],
			                             		AktieindkomstskatUnderGrundbeloebet = aktieindkomstskatLavesteTrin[index],
			                             		AktieindkomstskatOverGrundbeloebet = aktieindkomstskatMellemsteTrin[index] + aktieindkomstskatHoejesteTrin[index]
			                             	});
		}
	}
}