namespace Maxfire.Skat
{
	public class ModregnPersonfradragResult
	{
		public ModregnPersonfradragResult(Skatter modregnedeSkatter, Skatter ikkeUdnyttedeSkattevaerdier)
		{
			ModregnedeSkatter = modregnedeSkatter;
			IkkeUdnyttedeSkattevaerdier = ikkeUdnyttedeSkattevaerdier;
		}

		public Skatter ModregnedeSkatter { get; private set; }
		public Skatter IkkeUdnyttedeSkattevaerdier { get; private set; }
	}
}