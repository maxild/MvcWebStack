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

			var modregnResults = personfradragBeregner.ModregningAfPersonfradrag(skatter, kommunaleSatser);
			var modregninger = modregnResults.Map(x => x.UdnyttedeSkattevaerdier);
			var modregnedeSkatter = modregnResults.Map(x => x.ModregnedeSkatter);

			modregnResults[0].UdnyttetFradrag.ShouldEqual(100);
			modregnResults[0].IkkeUdnyttetFradrag.ShouldEqual(0);
			modregnResults[0].UdnyttetSkattevaerdi.ShouldEqual(41);
			modregnResults[0].IkkeUdnyttetSkattevaerdi.ShouldEqual(0);

			modregninger[0].Sundhedsbidrag.ShouldEqual(10);
			modregninger[0].Bundskat.ShouldEqual(5);
			modregninger[0].Kommuneskat.ShouldEqual(25);
			modregninger[0].Kirkeskat.ShouldEqual(1);

			modregnedeSkatter[0].Sundhedsbidrag.ShouldEqual(90);
			modregnedeSkatter[0].Bundskat.ShouldEqual(195);
			modregnedeSkatter[0].Kommuneskat.ShouldEqual(475);
			modregnedeSkatter[0].Kirkeskat.ShouldEqual(49);
		}

		[Fact]
		public void DelvisUdnyttelseAfSkattevaerdiP�SelveSkatten_Ugift()
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

			var modregnResults = personfradragBeregner.ModregningAfPersonfradrag(skatter, kommunaleSatser);
			var modregnedeSkatter = modregnResults.Map(x => x.ModregnedeSkatter);

			// Resterende skattev�rdi af personfradrag mht. sundhedsbidrag p� 5 overv�ltes i reduktionen af bundskat
			modregnedeSkatter[0].Sundhedsbidrag.ShouldEqual(0);
			modregnedeSkatter[0].Bundskat.ShouldEqual(190); // <-- reduktion her
			modregnedeSkatter[0].Kommuneskat.ShouldEqual(475);
			modregnedeSkatter[0].Kirkeskat.ShouldEqual(49);
		}

		[Fact]
		public void ModregningAfPersonfradragEgneSkatter()
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

			var modregnResults = personfradragBeregner.ModregningAfPersonfradragEgneSkatter(skatter, kommunaleSatser);
			var modregnedeSkatter = modregnResults.Map(x => x.ModregnedeSkatter);
			var modregninger = modregnResults.Map(x => x.UdnyttedeSkattevaerdier);

			modregnResults[0].Skattevaerdi.ShouldEqual(41);
			modregnResults[0].IkkeUdnyttetSkattevaerdi.ShouldEqual(14);
			modregnResults[0].UdnyttetSkattevaerdi.ShouldEqual(27);

			modregninger.ShouldEqual(skatter);

			modregnedeSkatter[0].Sundhedsbidrag.ShouldEqual(0);
			modregnedeSkatter[0].Bundskat.ShouldEqual(0);
			modregnedeSkatter[0].Kommuneskat.ShouldEqual(0);
			modregnedeSkatter[0].Kirkeskat.ShouldEqual(0);
		}

		[Fact]
		public void DelvisUdnyttelseAfSkattevaerdierOgModregningHosAegtefaelle()
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
				},
				new Skatter
				{
					Sundhedsbidrag = 100,
					Bundskat = 200,
					Kommuneskat = 500
				});

			ValueTuple<KommunaleSatser> kommunaleSatser = new ValueTuple<KommunaleSatser>(
				new KommunaleSatser
				{
					Kommuneskattesats = 0.25m,
					Kirkeskattesats = 0.01m
				},
				new KommunaleSatser
				{
					Kommuneskattesats = 0.25m
				});

			var personfradragBeregner = new PersonfradragBeregner();

			var modregnResults = personfradragBeregner.ModregningAfPersonfradrag(skatter, kommunaleSatser);
			var ikkeUdnyttetSkattevaerdi = modregnResults.Map(x => x.IkkeUdnyttetSkattevaerdi);
			var ikkeUdnyttetFradrag = modregnResults.Map(x => x.IkkeUdnyttetFradrag);
			var modregnedeSkatter = modregnResults.Map(x => x.ModregnedeSkatter);

			// 35 == 14 / (sum af satser), 14 = uudnyttet skattev�rdi, der overf�res til �gtef�lle
			// BUG: Forkerkt index p� kommunale satser
			// TODO: Benyt instans metoden p� beregner: BeregnSkattevaerdierAfPersonfradrag (efter BUG er rettet)
			var skattevaerdi = PersonfradragSkattevaerdiOmregner.Create(kommunaleSatser[1]).BeregnSkattevaerdier(35);
			
			// V�rdien af det overf�rte personfradrag
			skattevaerdi.Sundhedsbidrag.ShouldEqual(3.5m);
			skattevaerdi.Bundskat.ShouldEqual(1.75m);
			skattevaerdi.Kommuneskat.ShouldEqual(8.75m);
			skattevaerdi.Kirkeskat.ShouldEqual(0);

			// TODO: Lav den f�rdig

			// Skatterne nulstilles af v�rdien af personfradraget,
			modregnedeSkatter[0].Sundhedsbidrag.ShouldEqual(0);
			modregnedeSkatter[0].Bundskat.ShouldEqual(0);
			modregnedeSkatter[0].Kommuneskat.ShouldEqual(0);
			modregnedeSkatter[0].Kirkeskat.ShouldEqual(0);

			// og det uudnyttede personfradrag overf�res til �gtef�llen
			modregnedeSkatter[1].Sundhedsbidrag.ShouldEqual(90 - 3.5m);
			modregnedeSkatter[1].Bundskat.ShouldEqual(195 - 1.75m);
			modregnedeSkatter[1].Kommuneskat.ShouldEqual(475 - 8.75m);
			modregnedeSkatter[1].Kirkeskat.ShouldEqual(0);
		}
	}
}