using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using Maxfire.TestCommons.AssertExtensions;
using Maxfire.Web.Mvc.ValueProviders;
using Xunit;

namespace Maxfire.Web.Mvc.UnitTests
{
	public class PrefixContainerSpecs
	{
		[Fact]
		public void ContainsNullPrefix_Throws()
		{
			var sut = new BetterPrefixContainer(new string[] {});
			Assert.Throws<ArgumentNullException>(() => sut.ContainsPrefix(null));
		}

		[Fact]
		public void ContainsEmptyPrefix_ForEmptyKeys_ReturnsFalse()
		{
			var sut = new BetterPrefixContainer(new string[] {});
			sut.ContainsPrefix("").ShouldBeFalse();
		}

		[Fact]
		public void ContainsPrefixForNonEmptyKeys()
		{
			var sut = new BetterPrefixContainer(new[] { "foo" });
			sut.ContainsPrefix("").ShouldBeTrue();
		}

		[Fact]
		public void Bug()
		{
			// http://aspnetwebstack.codeplex.com/workitem/616
			var sut = new BetterPrefixContainer(new[] { "SomeData", "ValueIsSequence", "Value[0].Data" });
			sut.ContainsPrefix("Value").ShouldBeTrue();
		}

		[Fact]
		public void Get()
		{
			// Given "foo.bar", "foo.hello", "something.other", foo[abc].baz and asking for prefix "foo" will return:
			// - "bar"/"foo.bar"
			// - "hello"/"foo.hello"
			// - "abc"/"foo[abc]"

			var sut = new DictionaryValueProvider<string>(new Dictionary<string, string>
				{
					{ "foo.bar", "1"},
					{ "foo.hello", "2"},
					{ "something.other", "3"},
					{ "foo[abc].baz", "4"}
				}, CultureInfo.InvariantCulture);

		    //var keys = sut.GetKeysFromPrefix("foo");
			sut.GetKeysFromPrefix("foo");

			//foreach (var key in keys)
			//{
			//	Debug.WriteLine("{0} = {1}", key.Key, key.Value);
			//}
		}

		[Fact]
		public void GetKeysFromPrefix_()
		{
			var container = new BetterPrefixContainer(new[]
				{
					"foo[bar]",
					"something[other]",
					"foo.baz",
					"foot[hello]",
					"fo[nothing]",
					"foo",
					"foo.bar[0]",
					"foo.bar[1]",
					"foo.user[0].name",
					"foo.user[0].age",
					"foo.user[1].name",
					"foo.user[1].age",
				});

			//IDictionary<string, string> result = container.GetKeysFromPrefix("foo.user");
		    container.GetKeysFromPrefix("foo.user");

			//foreach (var kvp in result)
			//{
			//	Debug.WriteLine("result[{0}] = {1}", kvp.Key, kvp.Value);
			//}
		}
	}
}
