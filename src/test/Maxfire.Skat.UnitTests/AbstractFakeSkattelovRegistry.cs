namespace Maxfire.Skat.UnitTests
{
	public abstract class AbstractFakeSkattelovRegistry : ISkattelovRegistry
	{
		public virtual decimal GetAktieIndkomstLavesteProgressionsgraense(int skatteAar)
		{
			return 0m;
		}

		public virtual decimal GetAktieIndkomstHoejesteProgressionsgraense(int skatteAar)
		{
			return 0m;
		}

		public virtual decimal GetAktieIndkomstLavesteSkattesats(int skatteAar)
		{
			return 0m;
		}

		public virtual decimal GetAktieIndkomstMellemsteSkattesats(int skatteAar)
		{
			return 0m;
		}

		public virtual decimal GetAktieIndkomstHoejesteSkattesats(int skatteAar)
		{
			return 0m;
		}

		public virtual decimal GetAMBidragSkattesats(int skatteAar)
		{
			return 0m;
		}

		public virtual decimal GetSundhedsbidragSkattesats(int skatteAar)
		{
			return 0m;
		}

		public virtual decimal GetBundSkattesats(int skatteAar)
		{
			return 0m;
		}

		public virtual decimal GetMellemSkattesats(int skatteAar)
		{
			return 0m;
		}

		public virtual decimal GetTopSkattesats(int skatteAar)
		{
			return 0m;
		}

		public virtual decimal GetSkatteloftSkattesats(int skatteAar)
		{
			return 0m;
		}

		public virtual decimal GetPersonfradrag(int skatteAar)
		{
			return 0m;
		}

		public virtual decimal GetMellemskatBundfradrag(int skatteAar)
		{
			return 0m;
		}

		public virtual decimal GetTopskatBundfradrag(int skatteAar)
		{
			return 0m;
		}

		public virtual decimal GetPositivNettoKapitalIndkomstBundfradrag(int skatteAar)
		{
			return 0m;
		}
	}
}