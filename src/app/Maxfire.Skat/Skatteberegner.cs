namespace Maxfire.Skat
{
	public static class Skatteberegner
	{
		public static SkatteberegningResult Beregn(SkatteberegningInput input)
		{
			//
			// 1: Indkomst
			//

			// TODO: Sondring mellem intern indkomst context (f.eks. skal besk�ftigelsesfradrag beregnes, og andre fradrag begr�nses) og input-context

			// TODO: Simpelt interface p� de mange rubrikker (kun det h�jest n�dvendige, mindste f�llesn�vner). Ved at definere dette interface er klienten selv ansvarlig for mapping

			// Beregn besk�ftigelsesfradraget

			// Beregn de ligningsm�ssige fradrag

			// Beregn arbejdsmarkedsbidrag

			// Beregn personlig indkomst

			// Beregn kapital indkomst

			// Beregn skattepligtig indkomst (TODO: Med �gtef�lle overf�rsel)

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