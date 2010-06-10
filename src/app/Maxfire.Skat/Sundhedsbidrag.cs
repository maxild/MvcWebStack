namespace Maxfire.Skat
{
	public class SundhedsbidragBeregner
	{
		private readonly ISkattelovRegistry _skattelovRegistry;

		public SundhedsbidragBeregner(ISkattelovRegistry skattelovRegistry)
		{
			_skattelovRegistry = skattelovRegistry;
		}

		public ValueTuple<decimal> BeregnSkat(ValueTuple<PersonligeBeloeb> indkomster, int skatteAar)
		{
			var skattepligtigIndkomst = indkomster.Map(x => x.SkattepligtigIndkomst);
			return _skattelovRegistry.GetSundhedsbidragSkattesats(skatteAar) * (+skattepligtigIndkomst);
		}
	}
}