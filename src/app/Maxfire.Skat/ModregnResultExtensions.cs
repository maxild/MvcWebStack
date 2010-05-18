namespace Maxfire.Skat
{
	public static class ModregnResultExtensions
	{
		public static ModregnResultEx<TSkatter> ToModregnResultEx<TSkatter>(this ModregnResult<TSkatter> modregnResult, ISkattevaerdiOmregner skattevaerdiOmregner)
			where TSkatter : ISumable<decimal>, new()
		{
			decimal underskud = skattevaerdiOmregner.BeregnFradragsbeloeb(modregnResult.Skattevaerdi);
			decimal udnyttetUnderskud = skattevaerdiOmregner.BeregnFradragsbeloeb(modregnResult.UdnyttetSkattevaerdi);

			return new ModregnResultEx<TSkatter>(modregnResult.Skatter, modregnResult.Skattevaerdi, 
			                          modregnResult.UdnyttedeSkattevaerdier, underskud, udnyttetUnderskud);
		}
	}
}