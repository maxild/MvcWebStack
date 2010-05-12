namespace Maxfire.Skat
{
	public class PersonfradragSkattevaerdiOmregner : ISkattevaerdiOmregner
	{
		public static PersonfradragSkattevaerdiOmregner Create(KommunaleSatser kommunaleSatser)
		{
			return new PersonfradragSkattevaerdiOmregner(new Skatter
			                                             	{
			                                             		Sundhedsbidrag = Constants.Sundhedsbidragsats,
			                                             		Bundskat = Constants.Bundskattesats,
			                                             		Kommuneskat = kommunaleSatser.Kommuneskattesats,
			                                             		Kirkeskat = kommunaleSatser.Kirkeskattesats
			                                             	});
		}
		private readonly Skatter _skattesatser;

		public PersonfradragSkattevaerdiOmregner(Skatter skattesatser)
		{
			_skattesatser = skattesatser;
		}

		public Skatter BeregnSkattevaerdier(decimal personfradrag)
		{
			var skattevaerdier = _skattesatser * personfradrag;
			return skattevaerdier;
		}

		public decimal BeregnFradragsbeloeb(decimal skattevaerdi)
		{
			decimal fradragsbeloeb = skattevaerdi / _skattesatser.Sum();
			return fradragsbeloeb;
		}
	}
}