namespace Maxfire.Skat
{
	public static class Skatteberegner
	{
		public static SkatteberegningResult Beregn(SkatteberegningInput input)
		{
			//
			// 1: Indkomst
			//

			// TODO: Sondring mellem intern indkomst context (f.eks. skal beskæftigelsesfradrag beregnes, og andre fradrag begrænses) og input-context

			// TODO: Simpelt interface på de mange rubrikker (kun det højest nødvendige, mindste fællesnævner). Ved at definere dette interface er klienten selv ansvarlig for mapping

			// Beregn beskæftigelsesfradraget

			// Beregn de ligningsmæssige fradrag

			// Beregn arbejdsmarkedsbidrag

			// Beregn personlig indkomst

			// Beregn kapital indkomst

			// Beregn skattepligtig indkomst (TODO: Med ægtefælle overførsel)

			//
			// 2: Ejendomsværdiskat (springes over, da den kan insættes hvorsomhelst)
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
			// 6: Regulering af skattebeløbene, jf. § 13 i personskatteloven
			//

			// Hvis den skattepligtige indkomst udviser underskud....

			//
			// 7: Skatteværdien af personfradraget opgøres pr. skattetype
			//

			//
			// 8: Værdien af skatteloftet beregnes
			//

			//
			// 9: Bestem nedslaget i (forårspakke 2.0)
			//

			//
			// 10: Beregn kompensation (forårspakke 2.0) for skatteår 2012, 2013, etc.
			//

			//
			// 11: Beregn grøn check
			//

			return null;
		}
	}
}