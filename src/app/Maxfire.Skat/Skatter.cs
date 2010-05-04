namespace Maxfire.Skat
{
	// TODO: make this type immutable
	public class Skatter
	{
		public decimal Kirkeskat { get; set; }
		public decimal Kommuneskat { get; set; }
		public decimal Sundhedsbidrag { get; set; }
		public decimal Bundskat { get; set; }
		public decimal Mellemskat { get; set; }
		public decimal Topskat { get; set; }
		
		public decimal KommunalIndkomstskatOgKirkeskat
		{
			get { return Kommuneskat + Kirkeskat; }
		}

		/// <summary>
		/// Kun den del angivet ved PSL § 8a, stk. 2, der angiver den del af skat af aktieindkomst, der
		/// overstiger progressionsgrænsen, og ikke allerede er betalt som kildeskat (A-skat). Dermed 
		/// angiver beløbet den del der er B-skat.
		/// </summary>
		public decimal SkatAfAktieindkomst { get; set; }

		public ModregnResult ModregnNegativSkattepligtigIndkomst(decimal skattevaerdiAfUnderskud)
		{
			if (skattevaerdiAfUnderskud >= 0)
			{
				// Intet underskud, ingen grund til at modregne i skatterne
				return new ModregnResult(this, skattevaerdiAfUnderskud);
			}

			// Følgende skatter (statsskatter der beregnes af skattepligtig indkomst) 
			// bliver nedbragt med skatteværdien i nævnte rækkefølge
			decimal modregnetBundskat = Bundskat;
			decimal modregnetTopskat = Topskat;
			decimal modregnetSkatAfAktieindkomst = SkatAfAktieindkomst;

			decimal underskud = modregnetBundskat += skattevaerdiAfUnderskud;
			if (modregnetBundskat < 0)
			{
				underskud = modregnetTopskat += modregnetBundskat;
				modregnetBundskat = 0;
				if (modregnetTopskat < 0)
				{
					underskud = modregnetSkatAfAktieindkomst += modregnetTopskat;
					modregnetTopskat = 0; 
					if (modregnetSkatAfAktieindkomst < 0)
					{
						underskud = modregnetSkatAfAktieindkomst;
						modregnetSkatAfAktieindkomst = 0;
					}
				}
			}

			var modregnedeSkatter = new Skatter
			{
				Kirkeskat = Kirkeskat,
				Kommuneskat = Kommuneskat,
				Sundhedsbidrag = Sundhedsbidrag,
				Bundskat = modregnetBundskat,
				Topskat = modregnetTopskat,
				SkatAfAktieindkomst = modregnetSkatAfAktieindkomst
			};

			return new ModregnResult(modregnedeSkatter, underskud);
		}

		public Skatter ModregnNegativPersonligIndkomst(decimal underskud)
		{
			// TODO
			return this;
		}
	}

	public class ModregnResult
	{
		public ModregnResult(Skatter modregnedeSkatter, decimal underskud)
		{
			ModregnedeSkatter = modregnedeSkatter;
			Underskud = underskud;
		}

		public Skatter ModregnedeSkatter { get; private set; }
		public decimal Underskud { get; private set; }
	}
}