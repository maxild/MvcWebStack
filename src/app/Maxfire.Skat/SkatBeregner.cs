using System.Collections.Generic;

namespace Maxfire.Skat
{
	public class SkatBeregner
	{
		public ValueTuple<Skatter> BeregnSkat(ValueTuple<PersonligeBeloeb> indkomster, ValueTuple<KommunaleSatser> kommunaleSatser)
		{
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

			var list = new List<Skatter>(indkomster.Size);

			for (int i = 0; i < indkomster.Size; i++)
			{
				list.Add(new Skatter
				         	{
				         		Bundskat = bundskat[i],
				         		Mellemskat = mellemskat[i],
				         		Topskat = topskat[i],
				         		Sundhedsbidrag = sundhedsbidrag[i],
				         		Kommuneskat = kommuneskat[i],
				         		Kirkeskat = kirkeskat[i]
				         	});
			}

			return new ValueTuple<Skatter>(list);
		}
	}
}