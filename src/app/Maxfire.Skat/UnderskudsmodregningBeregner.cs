namespace Maxfire.Skat
{
	/// <summary>
	/// Underskud i skattepligtig indkomst skal efter § 13, stk. 1, så vidt muligt modregnes 
	/// med skatteværdien i bund-, mellem- og topskat samt skat af aktieindkomst over 
	/// progressionsgrænsen i § 8 a på 48.300 kr. (2009 og 2010).
	/// </summary>
	//TODO: Ingen fremførte underskud mellem indkomst år, hverken værdi fra forrige år, eller overførsel til kommende skatteår.
	public class UnderskudsmodregningBeregner
	{
		// Omregning af underskud til skatteværdi, og vice versa, sker.....
		public ValueTuple<Skatter> Beregn(ValueTuple<PersonligeBeloeb> indkomster, 
			ValueTuple<Skatter> skatter, ValueTuple<KommunaleSatser> kommunaleSatser)
		{
			var skattepligtigIndkomst = indkomster.Map(x => x.SkattepligtigIndkomst);
			var bundskat = skatter.Map(x => x.Bundskat);
			var topskat = skatter.Map(x => x.Topskat);
			var skatAfAktieindkomst = skatter.Map(x => x.SkatAfAktieindkomst);
			var kirkeskattesats = kommunaleSatser.Map(x => x.Kirkeskattesats);
			var kommuneskattesats = kommunaleSatser.Map(x => x.Kommuneskattesats);

			var sats = kirkeskattesats + kommuneskattesats + Constants.SundhedsbidragSats;
			var skattevaerdiAfUnderskud = skattepligtigIndkomst * sats;

			// PSL §13, stk 1:
			if (indkomster.Size == 1)
			{
				var modregnedeSkatter = skatter[0].ModregnNegativSkattepligtigIndkomst(skattevaerdiAfUnderskud[0]);
				return modregnedeSkatter.ToTuple();
			}

			// PSL §13, stk 2: Gifte
			return null;
		}
	}
}