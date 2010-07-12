﻿namespace Maxfire.Skat
{
	public abstract class AbstractSelvangivneBeloeb : ISelvangivneBeloeb
	{
		/// <summary>
		/// Den del af den personlige indkomst hvoraf der skal betales arbejdsmarkedsbidrag.
		/// </summary>
		public abstract decimal PersonligIndkomstAMIndkomst { get; }

		/// <summary>
		/// Den del af den personlige indkomst hvoraf der ikke skal betales arbejdsmarkedsbidrag.
		/// </summary>
		public abstract decimal PersonligIndkomstEjAMIndkomst { get; }

		/// <summary>
		/// Fradrag i personlig indkomst.
		/// </summary>
		public virtual decimal FradragPersonligIndkomst
		{
			// TODO: iværksætter konto + atp eget bidrag
			get { return PrivatTegnetPensionsindskud; }
		}

		///// <summary>
		///// Bidrag og præmier til privat tegnede ratepensioner og ophørende livrenter
		///// </summary>
		///// <remarks>
		///// Fradrages i den personlige indkomst, og 
		///// må højest udgøre 100.000 kr. årligt i 2010.
		///// </remarks>
		//public abstract decimal PrivatTegnetRatePensionsindskud { get; }

		///// <summary>
		///// Bidrag og præmie til privattegnet kapital pension.
		///// </summary>
		///// <remarks>
		///// Medregnes i topskattegrundlaget, og 
		///// må højest udgøre 46.000 kr. årligt i 2010.
		///// </remarks>
		//public abstract decimal PrivatTegnetKapitalPensionsindskud { get; }

		///// <summary>
		///// Indskud på arbejdsgiveradministrerede ratepensioner mv.
		///// </summary>
		///// <remarks>
		///// Fradrages i den personlige indkomst.
		///// </remarks>
		//public abstract decimal ArbejdsgiverAdministreretRatePensionsindskud { get; }

		///// <summary>
		///// Bidrag og præmie til arbejdsgiveradministreret kapitalpension og 
		///// supplerende engangsydelse (efter fradrag af AM-bidrag),
		///// </summary>
		///// <remarks>
		///// Medregnes i topskattegrundlaget.
		///// </remarks>
		//public abstract decimal ArbejdsgiverAdministreretKapitalPensionsindskud { get; }

		public abstract decimal KapitalPensionsindskud { get; }

		public abstract decimal PrivatTegnetPensionsindskud { get; }

		/// <summary>
		/// Renteindtægter, lejeindtægt mv.
		/// </summary>
		public abstract decimal KapitalIndkomst { get; }

		/// <summary>
		/// Renteudgifter mv.
		/// </summary>
		public abstract decimal FradragKapitalIndkomst { get; }

		/// <summary>
		/// Summen af de ligningsmæssige fradrag (befordring, fagligt kontingent, 
		/// betaling af underholdsbidrag, gaver til foreninger mv.), dog uden 
		/// beskæftigelsesfradraget.
		/// </summary>
		public abstract decimal LigningsmaessigeFradragMinusBeskaeftigelsesfradrag { get; }
	}
}