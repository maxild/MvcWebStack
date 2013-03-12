using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using Xunit;

namespace Maxfire.Web.Mvc.UnitTests
{
	public class BetterDefaultModelBinderTesting
	{
		[Fact]
		public void GetKeys()
		{
			var sut = new BetterDefaultModelBinder();
		} 


		[Fact]
		public void Get()
		{
			var sut = new DictionaryValueProvider<string>(new Dictionary<string, string>
				{
					{ "", ""}
				}, CultureInfo.InvariantCulture);

			//sut.GetKeysFromPrefix()
		}
	}
}