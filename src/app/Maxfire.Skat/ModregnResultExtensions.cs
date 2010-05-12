namespace Maxfire.Skat
{
	public static class ModregnResultExtensions
	{
		public static ModregnResultEx ToModregnResultEx(this ModregnResult modregnResult, ISkattevaerdiOmregner skattevaerdiOmregner)
		{
			decimal underskud = skattevaerdiOmregner.BeregnFradragsbeloeb(modregnResult.Skattevaerdi);
			decimal udnyttetUnderskud = skattevaerdiOmregner.BeregnFradragsbeloeb(modregnResult.UdnyttetSkattevaerdi);

			return new ModregnResultEx(modregnResult.Skatter, modregnResult.Skattevaerdi, 
			                          modregnResult.UdnyttedeSkattevaerdier, underskud, udnyttetUnderskud);
		}
	}
}