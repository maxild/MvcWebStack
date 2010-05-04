namespace Maxfire.Skat
{
	public class MellemskatGrundlagBeregner
	{
		public ValueTuple<decimal> BeregnGrundlag(ValueTuple<PersonligeBeloeb> input)
		{
			var personligIndkomst = input.Map(x => x.PersonligIndkomst);
			var nettoKapitalIndkomst = input.Map(x => x.NettoKapitalIndkomst);

			var samletNettoKapitalIndkomst = nettoKapitalIndkomst.NedbringPositivtMedEvtNegativt();

			return +(personligIndkomst + (+samletNettoKapitalIndkomst) - Constants.MellemskatBundfradrag);
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