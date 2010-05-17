namespace Maxfire.Skat
{
	/// <summary>
	/// Ægtefæller kan f.eks have forskellige skatteprocenter, hvis de bor i hver sin kommune, 
	/// men de kan være samlevende i skattemæssig forstand, eller hvis kun den ene er medlem af 
	/// folkekirken, eller de er blevet gift og flyttet sammen efter 5. september året forud 
	/// for indkomståret.
	/// </summary>
	public class KommunaleSatser
	{
		public decimal Kommuneskattesats { get; set; }
		public decimal Kirkeskattesats { get; set; }
		public decimal KommuneOgKirkeskattesats
		{
			get { return Kommuneskattesats + Kirkeskattesats; }
		}
	}
}