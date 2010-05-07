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
				IntrospectionOf<Skatter>.GetAccessorFor(x => x.Topskat),
				IntrospectionOf<Skatter>.GetAccessorFor(x => x.SkatAfAktieindkomst));
		}

		// TODO: ModregnResult skal erstattes med UnderskudResult (med IkkeUdnyttetUnderskud istedet for...eller suppleret oveni...IkkeUdnyttetSkattevaerdi)
		public ValueTuple<ModregnResult> Beregn(ValueTuple<PersonligeBeloeb> indkomster, 
			ValueTuple<Skatter> skatter, ValueTuple<KommunaleSatser> kommunaleSatser)
		{
			var skattepligtigIndkomst = indkomster.Map(x => x.SkattepligtigIndkomst);
			// TODO: Sondring mellem skattev�rdi af fremf�rt underskud og skattev�rdi af �rets underskud
			var skattevaerdier = BeregnSkattevaerdiAfUnderskud(skattepligtigIndkomst, kommunaleSatser);

			// Modregning i egne skatter
			var modregnResults = BeregnSkatEfterModregningEgneSkatter(skatter, skattevaerdier);

			// PSL �13, stk 2: Gifte

			// Hvis der er overskydende skattev�rdi tilbage, s� omregn underskud til skattev�rdi og Modregning i �gtef�lles skattepligtige indkomst

			// Modregn �gtef�lles egne skatter

			return modregnResults;
		}

		public ValueTuple<ModregnResult> BeregnSkatEfterModregningEgneSkatter(ValueTuple<Skatter> skatter, ValueTuple<decimal> skattevaerdier)
		{
			return skatter.Map(
				(skat, index) => BeregnSkatEfterModregningAfSkattevaerdi(skat, skattevaerdier[index]));
		}

		/// <summary>
		/// Beregn skatter efter modregning af skattev�rdi i den del af egne indkomstskatter, der ikke beregnes af skattepligtig indkomst.
		/// </summary>
		public ModregnResult BeregnSkatEfterModregningAfSkattevaerdi(Skatter skatter, decimal skattevaerdi)
		{
			var skatteModregner = GetSkattepligtigIndkomstUnderskudModregner();
			return skatteModregner.Modregn(skatter, skattevaerdi);
		}

		public ValueTuple<decimal> BeregnSkattevaerdiAfUnderskud(ValueTuple<decimal> skattepligtigIndkomster, ValueTuple<KommunaleSatser> kommunaleSatser)
		{
			return skattepligtigIndkomster.Map(
				(skattepligtigIndkomst, index) => BeregnSkattevaerdiAfUnderskud(skattepligtigIndkomst, kommunaleSatser[index]));
		}

		public decimal BeregnSkattevaerdiAfUnderskud(decimal skattepligtigIndkomst, KommunaleSatser kommunaleSatser)
		{
			if (skattepligtigIndkomst >= 0)
			{
				return 0;
			}
			var sats = kommunaleSatser.KommuneOgKirkeskattesats + Constants.Sundhedsbidragsats;
			var skattevaerdiAfUnderskud = -skattepligtigIndkomst * sats;
			return skattevaerdiAfUnderskud;
		}

		public ValueTuple<decimal> BeregnUnderskudAfSkattevaerdi(ValueTuple<decimal> skattevaerdier, ValueTuple<KommunaleSatser> kommunaleSatser)
		{
			return skattevaerdier.Map(
				(skattevaerdi, index) => BeregnUnderskudAfSkattevaerdi(skattevaerdi, kommunaleSatser[index]));
		}

		public decimal BeregnUnderskudAfSkattevaerdi(decimal skattevaerdi, KommunaleSatser kommunaleSatser)
		{
			if (skattevaerdi <= 0)
			{
				return 0;
			}
			var sats = kommunaleSatser.KommuneOgKirkeskattesats + Constants.Sundhedsbidragsats;
			var underskud = skattevaerdi / sats;
			return underskud;
		}
	}
}