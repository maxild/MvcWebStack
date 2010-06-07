namespace Maxfire.Skat
{
	public class MellemskatGrundlagBeregner
	{
		/// <summary>
		/// Beregn mellemskattegrundlaget før bundfradrag.
		/// </summary>
		public ValueTuple<decimal> BeregnBruttoGrundlag(ValueTuple<PersonligeBeloeb> input)
		{
			var personligIndkomst = input.Map(x => x.PersonligIndkomstSkattegrundlag);
			var nettoKapitalIndkomst = input.Map(x => x.NettoKapitalIndkomstSkattegrundlag);

			// Hvis en gift person har negativ nettokapitalindkomst, modregnes dette beløb
			// i den anden ægtefælles positive nettokapitalindkomst, inden mellemskatten beregnes
			var nettoKapitalIndkomstEfterModregning = nettoKapitalIndkomst.NedbringPositivtMedEvtNegativt();

			return (personligIndkomst + (+nettoKapitalIndkomstEfterModregning));
		}

		/// <summary>
		/// Beregn det udnyttede bundfradrag efter at der er sket overførsel af bundfradrag mellem ægtefæller.
		/// </summary>
		public ValueTuple<decimal> BeregnSambeskattetBundfradrag(ValueTuple<PersonligeBeloeb> indkomster)
		{
			var bruttoGrundlag = BeregnBruttoGrundlag(indkomster);
			return bruttoGrundlag.BeregnSambeskattetBundfradrag(Constants.MellemskatBundfradrag);
		}

		public ValueTuple<decimal> BeregnGrundlag(ValueTuple<PersonligeBeloeb> indkomster)
		{
			var bruttoGrundlag = BeregnBruttoGrundlag(indkomster);
			var udnyttetBundfradrag = BeregnSambeskattetBundfradrag(indkomster);
			return +(bruttoGrundlag - udnyttetBundfradrag);
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