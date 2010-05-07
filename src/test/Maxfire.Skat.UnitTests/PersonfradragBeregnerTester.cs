using Maxfire.TestCommons.AssertExtensions;
using Xunit;

namespace Maxfire.Skat.UnitTests
{
	public class PersonfradragBeregnerTester
	{
		[Fact]
		public void BeregnSkattevaerdierAfPersonfradrag()
		{
			Constants.Personfradrag = 100;
			Constants.Sundhedsbidragsats = 0.1m;
			Constants.Bundskattesats = 0.05m;

			ValueTuple<KommunaleSatser> kommunaleSatser = new ValueTuple<KommunaleSatser>(
				new KommunaleSatser
				{
					Kommuneskattesats = 0.25m,
					Kirkeskattesats = 0.01m
				});

			var personfradragBeregner = new PersonfradragBeregner();

			var skattevaerdier = personfradragBeregner.BeregnSkattevaerdierAfPersonfradrag(kommunaleSatser);

			skattevaerdier[0].Sundhedsbidrag.ShouldEqual(10);
			skattevaerdier[0].Bundskat.ShouldEqual(5);
			skattevaerdier[0].Kommuneskat.ShouldEqual(25);
			skattevaerdier[0].Kirkeskat.ShouldEqual(1);
		}

		[Fact]
		public void FuldUdnyttelseAfSkattevaerdiPaaSelveSkatten_Ugift()
		{
			Constants.Personfradrag = 100;
			Constants.Sundhedsbidragsats = 0.1m;
			Constants.Bundskattesats = 0.05m;
			
			ValueTuple<Skatter> skatter = new ValueTuple<Skatter>(
				new Skatter
				{
					Sundhedsbidrag = 100,
					Bundskat = 200,
					Kommuneskat = 500,
					Kirkeskat = 50
				});

			ValueTuple<KommunaleSatser> kommunaleSatser = new ValueTuple<KommunaleSatser>(
				new KommunaleSatser
				{
					Kommuneskattesats = 0.25m,
					Kirkeskattesats = 0.01m
				});

			var personfradragBeregner = new PersonfradragBeregner();

			var modregnedeSkatter = personfradragBeregner.BeregnSkatEfterPersonfradrag(skatter, kommunaleSatser);

			modregnedeSkatter[0].Sundhedsbidrag.ShouldEqual(90);
			modregnedeSkatter[0].Bundskat.ShouldEqual(195);
			modregnedeSkatter[0].Kommuneskat.ShouldEqual(475);
			modregnedeSkatter[0].Kirkeskat.ShouldEqual(49);
		}

		[Fact]
		public void DelvisUdnyttelseAfSkattevaerdiPåSelveSkatten_Ugift()
		{
			Constants.Personfradrag = 100;
			Constants.Sundhedsbidragsats = 0.1m;
			Constants.Bundskattesats = 0.05m;

			ValueTuple<Skatter> skatter = new ValueTuple<Skatter>(
				new Skatter
				{
					Sundhedsbidrag = 5, 
					Bundskat = 200,
					Kommuneskat = 500,
					Kirkeskat = 50
				});

			ValueTuple<KommunaleSatser> kommunaleSatser = new ValueTuple<KommunaleSatser>(
				new KommunaleSatser
				{
					Kommuneskattesats = 0.25m,
					Kirkeskattesats = 0.01m
				});

			var personfradragBeregner = new PersonfradragBeregner();

			var modregnedeSkatter = personfradragBeregner.BeregnSkatEfterPersonfradrag(skatter, kommunaleSatser);

			// Resterende skatteværdi af personfradrag mht. sundhedsbidrag på 5 overvæltes i reduktionen af bundskat
			modregnedeSkatter[0].Sundhedsbidrag.ShouldEqual(0);
			modregnedeSkatter[0].Bundskat.ShouldEqual(190); // <-- reduktion her
			modregnedeSkatter[0].Kommuneskat.ShouldEqual(475);
			modregnedeSkatter[0].Kirkeskat.ShouldEqual(49);
		}

		[Fact]
		public void BeregnSkatEfterPersonfradragEgneSkatter()
		{
			Constants.Personfradrag = 100;
			Constants.Sundhedsbidragsats = 0.1m;
			Constants.Bundskattesats = 0.05m;

			ValueTuple<Skatter> skatter = new ValueTuple<Skatter>(
				new Skatter
				{
					Sundhedsbidrag = 5, // Underskud = 5
					Bundskat = 2,		// Underskud = 3
					Kommuneskat = 20,	// Underskud = 5
					Kirkeskat = 0		// Underskud = 1
				});

			ValueTuple<KommunaleSatser> kommunaleSatser = new ValueTuple<KommunaleSatser>(
				new KommunaleSatser
				{
					Kommuneskattesats = 0.25m,
					Kirkeskattesats = 0.01m
				});

			var personfradragBeregner = new PersonfradragBeregner();

			var modregnPersonfradragResults = personfradragBeregner.BeregnSkatEfterPersonfradragEgneSkatter(skatter, kommunaleSatser);
			var modregnedeSkatter = modregnPersonfradragResults.Map(x => x.ModregnedeSkatter);
			var ikkeUdnyttedeSkattevaerdier = modregnPersonfradragResults.Map(x => x.IkkeUdnyttedeSkattevaerdier);

			// Ikke udnyttede skatteværdier overvæltes i den rækkefølge, der er nævnt i loven
			ikkeUdnyttedeSkattevaerdier[0].Sundhedsbidrag.ShouldEqual(0);
			ikkeUdnyttedeSkattevaerdier[0].Bundskat.ShouldEqual(0);
			ikkeUdnyttedeSkattevaerdier[0].Kommuneskat.ShouldEqual(13);
			ikkeUdnyttedeSkattevaerdier[0].Kirkeskat.ShouldEqual(1);
			ikkeUdnyttedeSkattevaerdier[0].Sum().ShouldEqual(14);

			// Skatterne nulstilles af værdien af personfradraget, og dey uudnyttede
			modregnedeSkatter[0].Sundhedsbidrag.ShouldEqual(0);
			modregnedeSkatter[0].Bundskat.ShouldEqual(0);
			modregnedeSkatter[0].Kommuneskat.ShouldEqual(0);
			modregnedeSkatter[0].Kirkeskat.ShouldEqual(0);
		}
	}
}