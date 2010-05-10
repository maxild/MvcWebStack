namespace Maxfire.Skat
{
	// Noter:
	// Beregnerens API benytter altid en kollektion (IList<T> via ReadOnlyCollection wrapper)
	// til repr�sentere en eller to personer. Konventionen er
	//		Count == 1: Beregning af ugift person
	//      Count == 2: beregning af gifte personer (�gtef�ller med sambeskatning)
	// Beregning af to ugifte personer kan laves via facade API
	//

	public interface IInputBeloeb
	{
		/// <summary>
		/// L�nindkomst mv.
		/// </summary>
		/// <remarks>
		/// Skal v�re efter fradrag af eget bidrag til arbejdsgiverpensionsordning.
		/// </remarks>
		decimal AMIndkomst { get; }
		
		/// <summary>
		/// Pension, sociale ydelser og arbejdsl�shedsunderst�ttelse mv.
		/// </summary>
		/// <remarks>
		/// Der betales ikke arbejdsmarkedsbidrag af denne del af den personlige indkomst.
		/// </remarks>
		decimal IkkeAMIndkomst { get;}

		/// <summary>
		/// Bidrag og pr�mier til privattegnede pensionsordninger med l�bende udbetalinger og ratepension 
		/// samt privattegnet kapitalpension dog h�jest 43.100 kr. i 2007.
		/// </summary>
		decimal PrivatPension { get; }
	}

	public interface IInputSatser
	{
		decimal JobFradragSats { get; }
		decimal AMBidragSats { get; }
	}

	public static class Skatteberegner
	{
		public static SkatteberegningResult Beregn(SkatteberegningInput input)
		{
			IInputBeloeb beloeb = null;
			IInputSatser satser = null;
			//
			// 1: Indkomst
			//

			// TODO: Sondring mellem intern indkomst context (f.eks. skal besk�ftigelsesfradrag beregnes, og andre fradrag begr�nses) og input-context

			// TODO: Simpelt interface p� de mange rubrikker (kun det h�jest n�dvendige, mindste f�llesn�vner). Ved at definere dette interface er klienten selv ansvarlig for mapping

			// Beregn besk�ftigelsesfradraget (der er et ligningsm�ssigt fradrag)
			decimal jobFradrag = (beloeb.AMIndkomst - beloeb.PrivatPension) * satser.JobFradragSats;
			
			// Beregn de ligningsm�ssige fradrag


			// Beregn arbejdsmarkedsbidrag
			decimal arbejdsmarkedsBidrag = beloeb.AMIndkomst * satser.AMBidragSats;

			// Beregn personlig indkomst 
			// (L�n - AMBidrag - AllePensionsindbetalinger)
			// (OBS: AMIndkomst er opgjort efter fradrag af evt. _egne_ bidrag til arbejdsgiver administrerede pensionsordninger,
			// _MEN_ det er ulogisk!!!!!
			decimal personligIndkomst = (beloeb.AMIndkomst + beloeb.IkkeAMIndkomst) - beloeb.PrivatPension 
				- arbejdsmarkedsBidrag;

			// Beregn kapital indkomst
			//decimal kapitalIndkomst;

			// Beregn skattepligtig indkomst (TODO: Med �gtef�lle overf�rsel)
			//decimal skattepligtigIndkomst;

			//
			// 2: Ejendomsv�rdiskat (springes over, da den kan ins�ttes hvorsomhelst)
			//

			//
			// 3: Skatter uden progression (kommune, bund og sundhedsbidrag)
			//

			//
			// 4: Topskat
			//

			//
			// 5: Aktieskat
			//

			//
			// 6: Regulering af skattebel�bene, jf. � 13 i personskatteloven
			//

			// Hvis den skattepligtige indkomst udviser underskud....

			//
			// 7: Skattev�rdien af personfradraget opg�res pr. skattetype
			//

			//
			// 8: V�rdien af skatteloftet beregnes
			//

			//
			// 9: Bestem nedslaget i (for�rspakke 2.0)
			//

			//
			// 10: Beregn kompensation (for�rspakke 2.0) for skatte�r 2012, 2013, etc.
			//

			//
			// 11: Beregn gr�n check
			//

			return null;
		}
	}
}