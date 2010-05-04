using System;
using System.Linq;

namespace Maxfire.Skat
{
	public class MellemskatGrundlagBeregner
	{
		public ValueTuple<decimal> BeregnBruttoGrundlag(ValueTuple<PersonligeBeloeb> input)
		{
			var personligIndkomst = input.Map(x => x.PersonligIndkomst);
			var nettoKapitalIndkomst = input.Map(x => x.NettoKapitalIndkomst);

			// Hvis en gift person har negativ nettokapitalindkomst, modregnes dette beløb
			// i den anden ægtefælles positive nettokapitalindkomst, inden mellemskatten beregnes
			var samletNettoKapitalIndkomst = nettoKapitalIndkomst.NedbringPositivtMedEvtNegativt();

			return (personligIndkomst + (+samletNettoKapitalIndkomst));
		}

		public ValueTuple<decimal> BeregnBundfradrag(ValueTuple<PersonligeBeloeb> indkomster)
		{
			var bruttoGrundlag = BeregnBruttoGrundlag(indkomster);

			Func<decimal, bool> ikkeFuldUdnyttelseAfBundfradrag = x =>
				x < Constants.MellemskatBundfradrag;

			int fraIndex = bruttoGrundlag.IndexOf(ikkeFuldUdnyttelseAfBundfradrag);
			if (fraIndex == -1 || bruttoGrundlag.All(ikkeFuldUdnyttelseAfBundfradrag))
			{
				// Begge ægtefæller har enten fuld udnyttelse eller ikke-fuld udnyttelse
				return Constants.MellemskatBundfradrag.ToTuple(Constants.MellemskatBundfradrag);
			}

			// Der er ikke-udnyttet bundfradrag hos en ægtefælle, der kan det udnyttes hos den anden ægtefælle
			decimal fraBundfradrag = bruttoGrundlag[fraIndex];
			decimal tilBundfradrag = 2 * Constants.MellemskatBundfradrag - fraBundfradrag;

			return fraIndex == 0 ? fraBundfradrag.ToTuple(tilBundfradrag) :
				tilBundfradrag.ToTuple(fraBundfradrag);
		}

		public ValueTuple<decimal> BeregnGrundlag(ValueTuple<PersonligeBeloeb> indkomster)
		{

			var bruttoGrundlag = BeregnBruttoGrundlag(indkomster);
			var bundfradrag = BeregnBundfradrag(indkomster);
			
			return +(bruttoGrundlag - bundfradrag);
		}
	}

	public class MellemskatBeregner
	{
		public ValueTuple<decimal> BeregnSkat(ValueTuple<PersonligeBeloeb> indkomster)
		{
			var grundlagBeregner = new MellemskatGrundlagBeregner();
			var grundlag = grundlagBeregner.BeregnGrundlag(indkomster);
			return Constants.Mellemskattesats * grundlag;
		}
	}
}