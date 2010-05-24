﻿namespace Maxfire.Skat
{
	public class PersonfradragSkattevaerdiOmregner : ISkattevaerdiOmregner
	{
		public static PersonfradragSkattevaerdiOmregner Create(KommunaleSatser kommunaleSatser)
		{
			return new PersonfradragSkattevaerdiOmregner(
				new Skatter(sundhedsbidrag: Constants.Sundhedsbidragsats,
				            kommuneskat: kommunaleSatser.Kommuneskattesats,
				            bundskat: Constants.Bundskattesats, kirkeskat: kommunaleSatser.Kirkeskattesats));
		}
		private readonly Skatter _skattesatser;

		public PersonfradragSkattevaerdiOmregner(Skatter skattesatser)
		{
			_skattesatser = skattesatser;
		}

		public Skatter BeregnSkattevaerdier(decimal personfradrag)
		{
			var skattevaerdier = _skattesatser * personfradrag;
			return skattevaerdier.RoundMoney();
		}

		public decimal BeregnFradragsbeloeb(decimal skattevaerdi)
		{
			decimal fradragsbeloeb = skattevaerdi / _skattesatser.Sum();
			return fradragsbeloeb.RoundMoney();
		}
	}
}