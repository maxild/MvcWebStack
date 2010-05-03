namespace Maxfire.Skat
{
	public class SundhedsbidragBeregner
	{
		public ValueTuple<decimal> BeregnSkat(ValueTuple<PersonligeBeloeb> indkomster)
		{
			var skattepligtigIndkomst = indkomster.Map(x => x.SkattepligtigIndkomst);
			return Constants.SundhedsbidragSats * (+skattepligtigIndkomst);
		}
	}
}