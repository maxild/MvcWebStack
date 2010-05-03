namespace Maxfire.Skat
{
	/// <summary>
	/// TFS beregner benytter forskellen mellem den høje og den lave sats (43 - 28), og
	/// beregner kun udbytteskat med denne 'diff sats' af beløbet ud over progressionsgrænsen.
	/// Dette skyldes at den lave udbytteskat er kildeskat, og allerede løbende betalt.
	/// </summary>
	public class AktieindkomstSkatBeregner
	{
		public ValueTuple<decimal> BeregnSkat(ValueTuple<PersonligeBeloeb> indkomster)
		{
			// TODO: Vi laver den senere, hvis det er nødvendigt
			// TODO: Samme logik som overførsel af bundfradrag for positiv kapitalindkomst i topskatten
			return null;
		}
	}
}