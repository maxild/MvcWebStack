namespace Maxfire.Skat
{
	public class SkatBeregner
	{
		public ValueTuple<Skatter> BeregnSkat(ValueTuple<PersonligeBeloeb> indkomster, ValueTuple<KommunaleSatser> kommunaleSatser)
		{
			// TODO: Refactor

			var bundskatBeregner = new BundskatBeregner();
			var bundskat = bundskatBeregner.BeregnSkat(indkomster);

			var mellemskatBeregner = new MellemskatBeregner();
			var mellemskat = mellemskatBeregner.BeregnSkat(indkomster);

			var topskatBeregner = new TopskatBeregner();
			var topskat = topskatBeregner.BeregnSkat(indkomster);

			var sundhedsbidragBeregner = new SundhedsbidragBeregner();
			var sundhedsbidrag = sundhedsbidragBeregner.BeregnSkat(indkomster);

			var kommuneskatBeregner = new KommuneskatBeregner();
			var kommuneskat = kommuneskatBeregner.BeregnSkat(indkomster, kommunaleSatser);

			var kirkeskatBeregner = new KirkeskatBeregner();
			var kirkeskat = kirkeskatBeregner.BeregnSkat(indkomster, kommunaleSatser);

			var aktieindkomstskatLavesteTrinBeregner = new AktieindkomstskatLavesteTrinBeregner();
			var aktieindkomstskatLavesteTrin = aktieindkomstskatLavesteTrinBeregner.BeregnSkat(indkomster);

			var aktieindkomstskatMellemsteTrinBeregner = new AktieindkomstskatMellemsteTrinBeregner();
			var aktieindkomstskatMellemsteTrin = aktieindkomstskatMellemsteTrinBeregner.BeregnSkat(indkomster);

			var aktieindkomstskatHoejesteTrinBeregner = new AktieindkomstskatHoejesteTrinBeregner();
			var aktieindkomstskatHoejesteTrin = aktieindkomstskatHoejesteTrinBeregner.BeregnSkat(indkomster);

			return bundskat.Map(index => new Skatter
			{
				Bundskat = bundskat[index],
				Mellemskat = mellemskat[index],
				Topskat = topskat[index],
				Sundhedsbidrag = sundhedsbidrag[index],
				Kommuneskat = kommuneskat[index],
				Kirkeskat = kirkeskat[index],
				AktieindkomstskatUnderGrundbeloebet = aktieindkomstskatLavesteTrin[index],
				AktieindkomstskatOverGrundbeloebet = aktieindkomstskatMellemsteTrin[index] + aktieindkomstskatHoejesteTrin[index]
			});
		}
	}
}