﻿namespace Maxfire.Skat.UnitTests
{
	public class SkatBeregner
	{
		private readonly ISkattelovRegistry _skattelovRegistry;

		public SkatBeregner(ISkattelovRegistry skattelovRegistry)
		{
			_skattelovRegistry = skattelovRegistry;
		}

		public ValueTuple<Skatter> BeregnSkat(ValueTuple<PersonligeBeloeb> indkomster, ValueTuple<KommunaleSatser> kommunaleSatser, int skatteAar)
		{
			var skatAfPersonligIndkomstBeregner = new SkatterAfPersonligIndkomstBeregner(_skattelovRegistry);
			var skatterAfPersonligIndkomst = skatAfPersonligIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser, skatteAar);

			var skatAfSkattepligtigIndkomstBeregner = new SkatterAfSkattepligtigIndkomstBeregner(_skattelovRegistry);
			var skatterAfSkattepligtigIndkomst = skatAfSkattepligtigIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser, skatteAar);

			return skatterAfPersonligIndkomst.Map(index =>
			                                      new Skatter(skatterAfPersonligIndkomst[index], skatterAfSkattepligtigIndkomst[index]));
		}
	}
}