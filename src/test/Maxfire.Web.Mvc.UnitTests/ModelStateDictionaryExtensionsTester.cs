using System.Web.Mvc;
using Maxfire.TestCommons.AssertExtensions;
using Xunit;

namespace Maxfire.Web.Mvc.UnitTests
{
	public class ModelStateDictionaryExtensionsTester
	{
		[Fact]
		public void IsInvalid_ShouldBeFalse()
		{
			var sut = new ModelStateDictionary
			{
				{"foo", new ModelState()},
				{"bar", new ModelState()}
			};

			sut.IsInvalid().ShouldBeFalse();
		}

		[Fact]
		public void IsInvalid_ShouldBeTrue()
		{
			var sut = new ModelStateDictionary
			{
				{"foo", new ModelState()},
				{"bar", new ModelState()}
			};
			sut.AddModelError("bar", "why");

			sut.IsInvalid().ShouldBeTrue();
		}

		[Fact]
		public void IsInvalid_Include_ShouldBeFalse()
		{
			var sut = new ModelStateDictionary
			{
				{"foo", new ModelState()},
				{"bar", new ModelState()}
			};
			sut.AddModelError("bar", "why");

			sut.IsInvalid(field => field.StartsWith("foo")).ShouldBeFalse();
		}

		[Fact]
		public void IsInvalid_Exclude_ShouldBeTrue()
		{
			var sut = new ModelStateDictionary
			{
				{"foo", new ModelState()},
				{"bar", new ModelState()}
			};
			sut.AddModelError("bar", "why");

			sut.IsInvalid(field => field.StartsWith("foo") == false).ShouldBeTrue();
		}
	}
}