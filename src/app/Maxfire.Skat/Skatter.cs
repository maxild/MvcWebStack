using System;

namespace Maxfire.Skat
{
	// TODO: make this type immutable
	public class Skatter
	{
		public decimal Kirkeskat { get; set; }
		public decimal Kommuneskat { get; set; }
		public decimal Sundhedsbidrag { get; set; }
		public decimal Bundskat { get; set; }
		public decimal Topskat { get; set; }
		
		/// <summary>
		/// Kun den del angivet ved PSL § 8a, stk. 2, der angiver den del af skat af aktieindkomst, der
		/// overstiger progressionsgrænsen, og ikke allerede er betalt som kildeskat (A-skat). Dermed 
		/// angiver beløbet den del der er B-skat.
		/// </summary>
		public decimal SkatAfAktieindkomst { get; set; }

		public Skatter ModregnNegativSkattepligtigIndkomst(decimal skattevaerdiAfUnderskud)
		{
			if (skattevaerdiAfUnderskud >= 0)
			{
				// Intet underskud, ingen grund til at modregne i skatterne
				return this;
			}

			// Følgende skatter bliver nedbragt i nævnte rækkefølge
			decimal modregnetBundskat = Bundskat;
			decimal modregnetTopskat = Topskat;
			decimal modregnetSkatAfAktieindkomst = SkatAfAktieindkomst;

			modregnetBundskat += skattevaerdiAfUnderskud;
			if (modregnetBundskat < 0)
			{
				modregnetTopskat += modregnetBundskat;
				modregnetBundskat = 0;
				if (modregnetTopskat < 0)
				{
					modregnetSkatAfAktieindkomst += modregnetTopskat;
					modregnetTopskat = 0;
					if (modregnetSkatAfAktieindkomst < 0)
					{
						// TODO: Hvis der efter modregningen er en overskydende negativ skatteværdi, omregnes 
						// denne til negativ skattepligtig indkomst, der fremføres til fradrag i den skattepligtige 
						// indkomst for de følgende indkomstår.
						decimal sats = 0; // Todo: Både skatteværdi, sats og underskud skal kendes
						decimal negativSkattepligtigIndkomstFradrag = modregnetSkatAfAktieindkomst / sats;
						modregnetSkatAfAktieindkomst = 0;
					}
				}
			}

			return new Skatter
			{
				Kirkeskat = Kirkeskat,
				Kommuneskat = Kommuneskat,
				Sundhedsbidrag = Sundhedsbidrag,
				Bundskat = modregnetBundskat,
				Topskat = modregnetTopskat,
				SkatAfAktieindkomst = modregnetSkatAfAktieindkomst
			};
		}

		public Skatter ModregnNegativPersonligIndkomst(decimal underskud)
		{
			// TODO
			return this;
		}
	}
}