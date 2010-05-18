namespace Maxfire.Skat
{
	public class SkatBeregner
	{
		public ValueTuple<Skatter> BeregnSkat(ValueTuple<PersonligeBeloeb> indkomster, ValueTuple<KommunaleSatser> kommunaleSatser)
		{
			var skatAfPersonligIndkomstBeregner = new SkatAfPersonligIndkomstBeregner();
			var skatterAfPersonligIndkomst = skatAfPersonligIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser);

			var skatAfSkattepligtigIndkomstBeregner = new SkatAfSkattepligtigIndkomstBeregner();
			var skatterAfSkattepligtigIndkomst = skatAfSkattepligtigIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser);

			return skatterAfPersonligIndkomst.Map(index => 
				new Skatter(skatterAfPersonligIndkomst[index], skatterAfSkattepligtigIndkomst[index]));
		}
	}
}