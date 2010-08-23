namespace Maxfire.Skat
{
	public static class KommunaleSatserExtensions
	{
		public static decimal GetKommuneOgKirkeskattesats(this IKommunaleSatser kommunaleSatser)
		{
			return kommunaleSatser.Kommuneskattesats + kommunaleSatser.Kirkeskattesats;
		}
	}
}