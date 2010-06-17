namespace Maxfire.Skat
{
	public class MellemskatBeregner
	{
		private readonly ISkattelovRegistry _skattelovRegistry;

		public MellemskatBeregner(ISkattelovRegistry skattelovRegistry)
		{
			_skattelovRegistry = skattelovRegistry;
		}

		public ValueTuple<decimal> BeregnSkat(ValueTuple<PersonligeBeloeb> indkomster, int skatteAar)
		{
			var grundlag = BeregnGrundlag(indkomster, skatteAar);
			return _skattelovRegistry.GetMellemSkattesats(skatteAar) * grundlag;
		}

		public ValueTuple<decimal> BeregnGrundlag(ValueTuple<PersonligeBeloeb> indkomster, int skatteAar)
		{
			var bruttoGrundlag = BeregnBruttoGrundlag(indkomster);
			var udnyttetBundfradrag = BeregnSambeskattetBundfradrag(indkomster, skatteAar);
			return +(bruttoGrundlag - udnyttetBundfradrag);
		}

		/// <summary>
		/// Beregn mellemskattegrundlaget før bundfradrag.
		/// </summary>
		public static ValueTuple<decimal> BeregnBruttoGrundlag(ValueTuple<PersonligeBeloeb> input)
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
		public ValueTuple<decimal> BeregnSambeskattetBundfradrag(ValueTuple<PersonligeBeloeb> indkomster, int skatteAar)
		{
			var bruttoGrundlag = BeregnBruttoGrundlag(indkomster);
			return bruttoGrundlag.BeregnSambeskattetBundfradrag(_skattelovRegistry.GetMellemskatBundfradrag(skatteAar));
		}
	}
}