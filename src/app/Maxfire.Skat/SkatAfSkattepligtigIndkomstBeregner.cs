namespace Maxfire.Skat
{
	public class SkatAfSkattepligtigIndkomstBeregner
	{
		public ValueTuple<SkatterAfSkattepligtigIndkomst> BeregnSkat(ValueTuple<PersonligeBeloeb> indkomster, ValueTuple<KommunaleSatser> kommunaleSatser)
		{
			// TODO: Refactor

			var sundhedsbidragBeregner = new SundhedsbidragBeregner();
			var sundhedsbidrag = sundhedsbidragBeregner.BeregnSkat(indkomster);

			var kommuneskatBeregner = new KommuneskatBeregner();
			var kommuneskat = kommuneskatBeregner.BeregnSkat(indkomster, kommunaleSatser);

			var kirkeskatBeregner = new KirkeskatBeregner();
			var kirkeskat = kirkeskatBeregner.BeregnSkat(indkomster, kommunaleSatser);

			return sundhedsbidrag.Map(index => new SkatterAfSkattepligtigIndkomst(sundhedsbidrag[index], kommuneskat[index], kirkeskat[index]));
		}
	}
}