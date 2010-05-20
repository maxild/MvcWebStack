namespace Maxfire.Skat
{
	public static class ModregnSkatterResultExtensions
	{
		public static ModregnSkatterResultEx<TSkatter> ToModregnResultEx<TSkatter>(this ModregnSkatterResult<TSkatter> modregnSkatterResult, ISkattevaerdiOmregner skattevaerdiOmregner)
			where TSkatter : ISumable<decimal>, new()
		{
			decimal underskud = skattevaerdiOmregner.BeregnFradragsbeloeb(modregnSkatterResult.Skattevaerdi);
			decimal udnyttetUnderskud = skattevaerdiOmregner.BeregnFradragsbeloeb(modregnSkatterResult.UdnyttetSkattevaerdi);

			return new ModregnSkatterResultEx<TSkatter>(modregnSkatterResult.Skatter, modregnSkatterResult.Skattevaerdi, 
			                          modregnSkatterResult.UdnyttedeSkattevaerdier, underskud, udnyttetUnderskud);
		}
	}
}