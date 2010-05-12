using Maxfire.TestCommons.AssertExtensions;
using Xunit;

namespace Maxfire.Skat.UnitTests
{
	public class SkatterTester
	{
		[Fact]
		public void CanAdd()
		{
			var x = new Skatter
			{
				Kommuneskat = 1,
				Kirkeskat = 2,
				Sundhedsbidrag = 3,
				Bundskat = 4,
				Mellemskat = 5,
				Topskat = 6,
				AktieindkomstskatUnderGrundbeloebet = 7,
				AktieindkomstskatOverGrundbeloebet = 8
			};

			var y = new Skatter
			{
				Kommuneskat = 2,
				Kirkeskat = 4,
				Sundhedsbidrag = 6,
				Bundskat = 8,
				Mellemskat = 10,
				Topskat = 12,
				AktieindkomstskatUnderGrundbeloebet = 14,
				AktieindkomstskatOverGrundbeloebet = 16
			};

			(x + y).ShouldEqual(new Skatter
			                    	{
			                    		Kommuneskat = 3,
			                    		Kirkeskat = 6,
			                    		Sundhedsbidrag = 9,
			                    		Bundskat = 12,
			                    		Mellemskat = 15,
			                    		Topskat = 18,
			                    		AktieindkomstskatUnderGrundbeloebet = 21,
			                    		AktieindkomstskatOverGrundbeloebet = 24
			                    	});
		}
	}
}