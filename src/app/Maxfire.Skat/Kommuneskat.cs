namespace Maxfire.Skat
{	
	public class KommuneskatBeregner
	{
		public ValueTuple<decimal> BeregnSkat(ValueTuple<PersonligeBeloeb> indkomster, ValueTuple<IKommunaleSatser> kommunaleSatser)
		{
			var skattepligtigIndkomst = indkomster.Map(x => x.SkattepligtigIndkomst);
			var kommuneskattesats = kommunaleSatser.Map(x => x.Kommuneskattesats);
			return kommuneskattesats * (+skattepligtigIndkomst);
		}
	}

	public class KirkeskatBeregner
	{
		public ValueTuple<decimal> BeregnSkat(ValueTuple<PersonligeBeloeb> indkomster, ValueTuple<IKommunaleSatser> kommunaleSatser)
		{
			var skattepligtigIndkomst = indkomster.Map(x => x.SkattepligtigIndkomst);
			var kirkeskattesats = kommunaleSatser.Map(x => x.Kirkeskattesats);
			return kirkeskattesats * (+skattepligtigIndkomst);
		}
	}
}