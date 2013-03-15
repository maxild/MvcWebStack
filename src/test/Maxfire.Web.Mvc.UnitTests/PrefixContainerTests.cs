using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Maxfire.TestCommons.AssertExtensions;
using Xunit;
using Xunit.Extensions;
using Assert = Maxfire.TestCommons.AssertEx;

namespace Maxfire.Web.Mvc.UnitTests
{
	public class PrefixContainerTests
    {
        [Fact]
        public void Constructor_GuardClauses()
        {
            // Act & assert
            Assert.ThrowsArgumentNull(() => new BetterPrefixContainer(null), "values");
        }

        [Fact]
        public void ContainsPrefix_GuardClauses()
        {
            // Arrange
            var container = new BetterPrefixContainer(new string[0]);

            // Act & assert
            Assert.ThrowsArgumentNull(() => container.ContainsPrefix(null), "prefix");
        }

        [Fact]
        public void ContainsPrefix_EmptyCollectionReturnsFalse()
        {
            // Arrange
            var container = new BetterPrefixContainer(new string[0]);

            // Act & Assert
            Assert.False(container.ContainsPrefix(""));
        }

        [Fact]
        public void ContainsPrefix_ExactMatch()
        {
            // Arrange
            var container = new BetterPrefixContainer(new[] { "Hello" });

            // Act & Assert
            Assert.True(container.ContainsPrefix("Hello"));
        }

        [Fact]
        public void ContainsPrefix_MatchIsCaseInsensitive()
        {
            // Arrange
            var container = new BetterPrefixContainer(new[] { "Hello" });

            // Act & Assert
            Assert.True(container.ContainsPrefix("hello"));
        }

        [Fact]
        public void ContainsPrefix_MatchIsNotSimpleSubstringMatch()
        {
            // Arrange
            var container = new BetterPrefixContainer(new[] { "Hello" });

            // Act & Assert
            Assert.False(container.ContainsPrefix("He"));
        }

        [Fact]
        public void ContainsPrefix_NonEmptyCollectionReturnsTrueIfPrefixIsEmptyString()
        {
            // Arrange
            var container = new BetterPrefixContainer(new[] { "Hello" });

            // Act & Assert
            Assert.True(container.ContainsPrefix(""));
        }

        [Fact]
        public void ContainsPrefix_PrefixBoundaries()
        {
            // Arrange
            var container = new BetterPrefixContainer(new[] { "Hello.There[0]" });

            // Act & Assert
            Assert.True(container.ContainsPrefix("hello"));
            Assert.True(container.ContainsPrefix("hello.there"));
            Assert.True(container.ContainsPrefix("hello.there[0]"));
            Assert.False(container.ContainsPrefix("hello.there.0"));
        }

        [Theory]
        [InlineData("a")]
        [InlineData("a[d]")]
        [InlineData("c.b")]
        [InlineData("c.b.a")]
        public void ContainsPrefix_PositiveTests(string testValue)
        {
            // Arrange
            var container = new BetterPrefixContainer(new[] { "a.b", "c.b.a", "a[d]", "a.c" });

            // Act & Assert
            Assert.True(container.ContainsPrefix(testValue));
        }

        [Theory]
        [InlineData("a.d")]
        [InlineData("b")]
        [InlineData("c.a")]
        [InlineData("c.b.a.a")]
        public void ContainsPrefix_NegativeTests(string testValue)
        {
            // Arrange
            var container = new BetterPrefixContainer(new[] { "a.b", "c.b.a", "a[d]", "a.c" });

            // Act & Assert
            Assert.False(container.ContainsPrefix(testValue));
        }

        [Fact]
        public void GetKeysFromPrefix_DotsNotation()
        {
            // Arrange
            var container = new BetterPrefixContainer(new[] { "foo.bar.baz", "something.other", "foo.baz", "foot.hello", "fo.nothing", "foo" });

            // Act
            IDictionary<string, string> result = container.GetKeysFromPrefix("foo");

            // Assert
            Assert.Equal(2, result.Count());
            Assert.True(result.ContainsKey("bar"));
            Assert.True(result.ContainsKey("baz"));
            Assert.Equal("foo.bar", result["bar"]);
            Assert.Equal("foo.baz", result["baz"]);
        }

        [Fact]
        public void GetKeysFromPrefix_BracketsNotation()
        {
            // Arrange
            var container = new BetterPrefixContainer(new[] { "foo[bar]baz", "something[other]", "foo[baz]", "foot[hello]", "fo[nothing]", "foo" });

            // Act
            IDictionary<string, string> result = container.GetKeysFromPrefix("foo");

            // Assert
            Assert.Equal(2, result.Count());
            Assert.True(result.ContainsKey("bar"));
            Assert.True(result.ContainsKey("baz"));
            Assert.Equal("foo[bar]", result["bar"]);
            Assert.Equal("foo[baz]", result["baz"]);
        }

        [Fact]
        public void GetKeysFromPrefix_MixedDotsAndBrackets()
        {
            // Arrange
            var container = new BetterPrefixContainer(new[] { "foo[bar]baz", "something[other]", "foo.baz", "foot[hello]", "fo[nothing]", "foo" });

            // Act
            IDictionary<string, string> result = container.GetKeysFromPrefix("foo");

            // Assert
            Assert.Equal(2, result.Count());
            Assert.True(result.ContainsKey("bar"));
            Assert.True(result.ContainsKey("baz"));
            Assert.Equal("foo[bar]", result["bar"]);
            Assert.Equal("foo.baz", result["baz"]);
        }

        [Fact]
        public void GetKeysFromPrefix_AllValues()
        {
            // Arrange
            var container = new BetterPrefixContainer(new[] { "foo[bar]baz", "something[other]", "foo.baz", "foot[hello]", "fo[nothing]", "foo" });

            // Act
			IDictionary<string, string> result = container.GetKeysFromPrefix("");

            // Assert
            Assert.Equal(4, result.Count());
            Assert.Equal("foo", result["foo"]);
            Assert.Equal("something", result["something"]);
            Assert.Equal("foot", result["foot"]);
            Assert.Equal("fo", result["fo"]);
        }

        [Fact]
        public void GetKeysFromPrefix_PrefixNotFound()
        {
            // Arrange
            var container = new BetterPrefixContainer(new[] { "foo[bar]", "something[other]", "foo.baz", "foot[hello]", "fo[nothing]", "foo" });

            // Act
            IDictionary<string, string> result = container.GetKeysFromPrefix("notfound");

            // Assert
            Assert.Empty(result);
        }

		[Fact]
		public void Bug()
		{
			var sut = new BetterPrefixContainer(new[] { "SomeData", "ValueIsSequence", "Value[0].Data" });
			sut.ContainsPrefix("Value").ShouldBeTrue();
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

			IDictionary<string, string> result = container.GetKeysFromPrefix("foo.user");

			foreach (var kvp in result)
			{
				Debug.WriteLine("result[{0}] = {1}", kvp.Key, kvp.Value);
			}
		}
    }
}