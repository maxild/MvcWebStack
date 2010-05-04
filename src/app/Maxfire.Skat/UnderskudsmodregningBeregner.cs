using System.Collections.Generic;

namespace Maxfire.Skat
{
	/// <summary>
	/// Underskud i skattepligtig indkomst skal efter � 13, stk. 1, s� vidt muligt modregnes 
	/// med skattev�rdien i bund-, mellem- og topskat samt skat af aktieindkomst over 
	/// progressionsgr�nsen i � 8 a p� 48.300 kr. (2009 og 2010).
	/// </summary>
	//TODO: Ingen fremf�rte underskud mellem indkomst �r, hverken v�rdi fra forrige �r, eller overf�rsel til kommende skatte�r.
	public class UnderskudsmodregningBeregner
	{
		// Omregning af underskud til skattev�rdi, og vice versa, sker.....
		public ValueTuple<Skatter> Beregn(ValueTuple<PersonligeBeloeb> indkomster, 
			ValueTuple<Skatter> skatter, ValueTuple<KommunaleSatser> kommunaleSatser)
		{
			var skattepligtigIndkomst = indkomster.Map(x => x.SkattepligtigIndkomst);
			var bundskat = skatter.Map(x => x.Bundskat);
			var topskat = skatter.Map(x => x.Topskat);
			var skatAfAktieindkomst = skatter.Map(x => x.SkatAfAktieindkomst);
			var kirkeskattesats = kommunaleSatser.Map(x => x.Kirkeskattesats);
			var kommuneskattesats = kommunaleSatser.Map(x => x.Kommuneskattesats);

			var sats = kirkeskattesats + kommuneskattesats + Constants.Sundhedsbidragsats;
			var skattevaerdiAfUnderskud = skattepligtigIndkomst * sats;

			ValueTuple<Skatter> result = null;

			// PSL �13, stk 1:
			if (indkomster.Size == 1)
			{
				var modregnResult = skatter[0].ModregnNegativSkattepligtigIndkomst(skattevaerdiAfUnderskud[0]);
				result = modregnResult.ModregnedeSkatter.ToTuple();
			}

			// PSL �13, stk 2: Gifte

			// Modregning i egne skatter

			// Hvis der er overskydende skattev�rdi tilbage, s� omregn underskud til skattev�rdi og Modregning i �gtef�lles skattepligtige indkomst

			// Modregn �gtef�lles egne skatter

			return result;
		}
	}

	public class PersonFradragBeregner
	{
		public ValueTuple<Skatter> Beregn(ValueTuple<PersonligeBeloeb> indkomster,
			ValueTuple<Skatter> skatter, ValueTuple<KommunaleSatser> kommunaleSatser)
		{
			var skattevaerdier = BeregnSkattevaerdier(kommunaleSatser);

			// Sundhedsbidraget

				// Hvis ikke-udnyttet skattev�rdi => fradrag i bundskat, topskat og skat af aktieindkomst (lav generel rutine)

			// Bundskat

				// Hvis ikke-udnyttet skattev�rdi => fradrag i sundhedsbidrag, topskat og skat af aktieindkomst (lav generel rutine)

			ValueTuple<Skatter> modregnedeSkatter = null;

			return modregnedeSkatter;
		}

		/// <summary>
		/// Beregn skattev�rdier af personfradraget for kommuneskat, kirkeskat, bundskat og sunhedsbidrag.
		/// </summary>
		/// <returns></returns>
		public ValueTuple<Skatter> BeregnSkattevaerdier(ValueTuple<KommunaleSatser> kommunaleSatser)
		{
			var kommuneskattesats = kommunaleSatser.Map(x => x.Kommuneskattesats);
			var kirkeskattesats = kommunaleSatser.Map(x => x.Kirkeskattesats);
			
			decimal skattevaerdiBundskat = Constants.Bundskattesats * Constants.PersonFradrag;
			decimal skattevaerdiSundhedsbidrag = Constants.Sundhedsbidragsats * Constants.PersonFradrag;
			var skattevaerdiKommuneskat = kommuneskattesats * Constants.PersonFradrag;
			var skattevaerdiKirkeskat = kirkeskattesats * Constants.PersonFradrag;

			var list = new List<Skatter>(kommunaleSatser.Size);
			for (int i = 0; i < kommunaleSatser.Size; i++)
			{
				list.Add(new Skatter
				         	{
				         		Bundskat = skattevaerdiBundskat,
				         		Sundhedsbidrag = skattevaerdiSundhedsbidrag,
				         		Kommuneskat = skattevaerdiKommuneskat[i],
				         		Kirkeskat = skattevaerdiKirkeskat[i]
				         	});
			}

			return new ValueTuple<Skatter>(list);
		}
	}
}