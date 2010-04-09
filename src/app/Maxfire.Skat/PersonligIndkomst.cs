namespace Maxfire.Skat
{
	public class PersonligIndkomst
	{
		public PersonligIndkomst()
		{
			AMIndkomst = new PersonligIndkomstArbejdsmarkedsbidrag();
			IkkeAMIndkomst = new PersonligIndkomstEjArbejdsmarkedsbidrag();
			Fradrag = new PersonligIndkomstFradrag();
		}

		public PersonligIndkomstArbejdsmarkedsbidrag AMIndkomst { get; private set; }
		public PersonligIndkomstEjArbejdsmarkedsbidrag IkkeAMIndkomst { get; private set; }
		public PersonligIndkomstFradrag Fradrag { get; private set; }

		public decimal IAlt
		{
			get { return AMIndkomst.IAlt + IkkeAMIndkomst.IAlt - Fradrag.IAlt; }
		}
	}

	public class PersonligIndkomstArbejdsmarkedsbidrag
	{
		///<summary>
		/// Rubrik 11: L�nindkomst, bestyrelseshonorar, multimedier (fri telefon mv.), 
		/// fri bil, fri kost og logi efter Skatter�dets satser, men f�r fradrag af AM-bidrag.
		/// </summary>
		/// <remarks>
		/// Bel�bet skal v�re efter fradrag af ATP- og arbejdsgiveradministreret pensionsbidrag, men f�r fradrag af AM-bidrag.
		/// </remarks>
		public decimal Loen { get; set; }

		/// <summary>
		/// Anden personlig indkomst, der skal svares arbejdsmarkedsbidrag af.
		/// </summary>
		/// <remarks>
		/// Rubrik 12: Honorarer og vederlag i form af visse goder mv. f�r fradrag af AM-bidrag.
		/// Rubrik 14: Jubil�umsgratiale og fratr�delsesgodtg�relse mv. f�r fradrag af AM-bidrag.
		/// Rubrik 15: Anden personlig indkomst som fx fri telefon, privat dagpleje og hushj�lp mv. f�r fradrag af AM-bidrag.
		/// mv.
		/// </remarks>
		public decimal AndenIndkomst { get; set; }

		public decimal IAlt
		{
			get { return Loen + AndenIndkomst; }
		}
	}

	public class PersonligIndkomstEjArbejdsmarkedsbidrag
	{
		/// <summary>
		/// Stipendier fra SU
		/// </summary>
		// Rubrik 16: Pensioner, dagpenge mv. og stipendier fra SUstyrelsen.
		public decimal SU { get; set; }

		// Rubrik 16: Pensioner, dagpenge mv. og stipendier fra SUstyrelsen.
		public decimal Sygedagpenge { get; set; }

		// Rubrik 16: Pensioner, dagpenge mv. og stipendier fra SUstyrelsen.
		public decimal Arbejdsloeshedsdagpenge { get; set; }

		// Rubrik 16: Pensioner, dagpenge mv. og stipendier fra SUstyrelsen.
		public decimal Kontanthjaelp { get; set; }

		// Rubrik 16: Pensioner, dagpenge mv. og stipendier fra SUstyrelsen.
		public decimal Efterloen { get; set; }

		// Rubrik 16: Pensioner, dagpenge mv. og stipendier fra SUstyrelsen.
		public decimal Pensionsudbetalinger { get; set; }

		/// <summary>
		/// Rubrik 19: Modtaget underholdsbidrag
		/// </summary>
		public decimal ModtagetUnderholdsbidrag { get; set; }

		/// <summary>
		/// Anden personlig indkomst, der ikke skal svares arbejdsmarkedsbidrag af.
		/// </summary>
		/// <remarks>
		/// Rubrik 20: Anden personlig indkomst
		/// </remarks>
		public decimal AndenIndkomst { get; set; }

		// TODO: Mangler
		// Rubrik 17: Uddelinger fra foreninger og fonde mv. Gruppelivsforsikring betalt af pensionskasse. Visse personalegoder.
		// Rubrik 18: H�dersgaver

		public decimal IAlt
		{
			get
			{
				return SU +
					   Sygedagpenge +
					   Arbejdsloeshedsdagpenge +
					   Kontanthjaelp +
					   Efterloen +
					   Pensionsudbetalinger +
					   ModtagetUnderholdsbidrag +
					   AndenIndkomst;
			}
		}
	}

	public class PersonligIndkomstFradrag
	{
		/// <summary>
		/// Privattegnede pensionsordninger fradrages i den personlige indkomst
		/// </summary>
		public Pensionsbidrag PrivatPension { get; set; }

		/// <summary>
		/// Fradragsberettigede indskud p� iv�rks�tterkonto.
		/// </summary>
		/// <remarks>
		/// Indskud p� en iv�rks�tterkonto vil v�re fradrag ved opg�relsen af den personlige indkomst. 
		/// Dermed f�r s�danne fradrag en h�jere skattev�rdi end fradrag for indskud p� etableringskonto, 
		/// der 'kun' er ligningsm�ssige fradrag.
		/// </remarks>
		public decimal IvaerksaetterKonto { get; set; }

		public decimal IAlt
		{
			get
			{
				return PrivatPension.IAlt;
			}
		}
	}
}