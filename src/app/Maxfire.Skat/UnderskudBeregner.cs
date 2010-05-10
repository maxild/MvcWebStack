using Maxfire.Core.Reflection;

namespace Maxfire.Skat
{
	/// <summary>
	/// Underskud i skattepligtig indkomst skal efter � 13, stk. 1, s� vidt muligt modregnes 
	/// med skattev�rdien i bund-, mellem- og topskat samt skat af aktieindkomst over 
	/// progressionsgr�nsen i � 8 a p� 48.300 kr. (2009 og 2010).
	/// </summary>
	// TODO: Ingen fremf�rte underskud mellem indkomst �r, hverken v�rdi fra forrige �r, eller overf�rsel til kommende skatte�r.
	// TODO: Skal hedde UnderskudSkattepligtigIndkomstBeregner
	public class UnderskudBeregner
	{
		public static SkatteModregner GetSkattepligtigIndkomstUnderskudModregner()
		{
			return new SkatteModregner(
				IntrospectionOf<Skatter>.GetAccessorFor(x => x.Bundskat),
				IntrospectionOf<Skatter>.GetAccessorFor(x => x.Mellemskat),
				IntrospectionOf<Skatter>.GetAccessorFor(x => x.Topskat),
				IntrospectionOf<Skatter>.GetAccessorFor(x => x.SkatAfAktieindkomst));
		}

		public ValueTuple<ModregnResult2> Beregn(ValueTuple<PersonligeBeloeb> indkomster, 
			ValueTuple<Skatter> skatter, ValueTuple<KommunaleSatser> kommunaleSatser)
		{
			var skattepligtigIndkomster = indkomster.Map(x => x.SkattepligtigIndkomst);
			var underskudSkattevaerdiBeregnere = GetUnderskudSkattevaerdiBeregnere(kommunaleSatser);
			// TODO: Sondring mellem skattev�rdi af fremf�rt underskud og skattev�rdi af �rets underskud
			
			// Modregning i egne skatter
			var modregnResults = BeregnSkatEfterModregningEgneSkatter(skatter, underskudSkattevaerdiBeregnere, skattepligtigIndkomster);

			// PSL �13, stk 2: Gifte

			// Hvis der er overskydende skattev�rdi tilbage, s� omregn underskud til skattev�rdi og Modregning i �gtef�lles skattepligtige indkomst

			// Modregn �gtef�lles egne skatter

			return modregnResults;
		}

		public ValueTuple<ModregnResult2> BeregnSkatEfterModregningEgneSkatter(ValueTuple<Skatter> skatter, 
			ValueTuple<UnderskudSkattevaerdiBeregner> underskudSkattevaerdiBeregnere, ValueTuple<decimal> skattepligtigeIndkomster)
		{
			return skatter.Map(
				(skat, index) => BeregnSkatEfterModregningEgneSkatter(skat, underskudSkattevaerdiBeregnere[index], skattepligtigeIndkomster[index]));
		}

		/// <summary>
		/// Beregn skatter efter modregning af skattev�rdi i den del af egne indkomstskatter, der ikke beregnes af skattepligtig indkomst.
		/// </summary>
		public ModregnResult2 BeregnSkatEfterModregningEgneSkatter(Skatter skatter, 
			UnderskudSkattevaerdiBeregner underskudSkattevaerdiBeregner, decimal skattepligtigIndkomst)
		{
			var skattevaerdiAfUnderskud = underskudSkattevaerdiBeregner.BeregnSkattevaerdiAfUnderskud(skattepligtigIndkomst);
			var skatteModregner = GetSkattepligtigIndkomstUnderskudModregner();
			var modregnResult = skatteModregner.Modregn(skatter, skattevaerdiAfUnderskud);
			decimal ikkeUdnyttetSkattevaerdi = modregnResult.IkkeUdnyttetSkattevaerdi;
			decimal ikkeUdnyttetUnderskud = underskudSkattevaerdiBeregner.BeregnUnderskudAfSkattevaerdi(ikkeUdnyttetSkattevaerdi);
			return new ModregnResult2(modregnResult.ModregnedeSkatter, ikkeUdnyttetSkattevaerdi, ikkeUdnyttetUnderskud);
		}

		public ValueTuple<UnderskudSkattevaerdiBeregner> GetUnderskudSkattevaerdiBeregnere(ValueTuple<KommunaleSatser> kommunaleSatser)
		{
			return kommunaleSatser.Map(satser => GetUnderskudSkattevaerdiBeregner(satser));
		}

		public UnderskudSkattevaerdiBeregner GetUnderskudSkattevaerdiBeregner(KommunaleSatser kommunaleSatser)
		{
			return new UnderskudSkattevaerdiBeregner(kommunaleSatser);
		}

		//public ValueTuple<decimal> BeregnSkattevaerdiAfUnderskud(ValueTuple<decimal> skattepligtigIndkomster, ValueTuple<KommunaleSatser> kommunaleSatser)
		//{
		//    return skattepligtigIndkomster.Map(
		//        (skattepligtigIndkomst, index) => BeregnSkattevaerdiAfUnderskud(skattepligtigIndkomst, kommunaleSatser[index]));
		//}

		//public decimal BeregnSkattevaerdiAfUnderskud(decimal skattepligtigIndkomst, KommunaleSatser kommunaleSatser)
		//{
		//    if (skattepligtigIndkomst >= 0)
		//    {
		//        return 0;
		//    }
		//    var sats = kommunaleSatser.KommuneOgKirkeskattesats + Constants.Sundhedsbidragsats;
		//    var skattevaerdiAfUnderskud = -skattepligtigIndkomst * sats;
		//    return skattevaerdiAfUnderskud;
		//}

		//public ValueTuple<decimal> BeregnUnderskudAfSkattevaerdi(ValueTuple<decimal> skattevaerdier, ValueTuple<KommunaleSatser> kommunaleSatser)
		//{
		//    return skattevaerdier.Map(
		//        (skattevaerdi, index) => BeregnUnderskudAfSkattevaerdi(skattevaerdi, kommunaleSatser[index]));
		//}

		//public decimal BeregnUnderskudAfSkattevaerdi(decimal skattevaerdi, KommunaleSatser kommunaleSatser)
		//{
		//    if (skattevaerdi <= 0)
		//    {
		//        return 0;
		//    }
		//    var sats = kommunaleSatser.KommuneOgKirkeskattesats + Constants.Sundhedsbidragsats;
		//    var underskud = skattevaerdi / sats;
		//    return underskud;
		//}
	}
}