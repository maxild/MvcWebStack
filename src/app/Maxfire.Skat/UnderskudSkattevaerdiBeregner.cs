namespace Maxfire.Skat
{
	public class UnderskudSkattevaerdiBeregner
	{
		private readonly KommunaleSatser _kommunaleSatser;

		public UnderskudSkattevaerdiBeregner(KommunaleSatser kommunaleSatser)
		{
			_kommunaleSatser = kommunaleSatser;
		}

		public decimal BeregnUnderskudAfSkattevaerdi(decimal skattevaerdi)
		{
			if (skattevaerdi <= 0)
			{
				return 0;
			}
			var sats = _kommunaleSatser.KommuneOgKirkeskattesats + Constants.Sundhedsbidragsats;
			var underskud = skattevaerdi / sats;
			return underskud.RoundMoney();
		}

		public decimal BeregnSkattevaerdiAfUnderskud(decimal skattepligtigIndkomst)
		{
			if (skattepligtigIndkomst >= 0)
			{
				return 0;
			}
			var sats = _kommunaleSatser.KommuneOgKirkeskattesats + Constants.Sundhedsbidragsats;
			var skattevaerdiAfUnderskud = -skattepligtigIndkomst * sats;
			return skattevaerdiAfUnderskud.RoundMoney();
		}
	}
}