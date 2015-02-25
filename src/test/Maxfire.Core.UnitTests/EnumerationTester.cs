using System;
using Maxfire.TestCommons.AssertExtensions;
using Xunit;

namespace Maxfire.Core.UnitTests
{
	public class EnumerationTester
	{
		[Fact]
		public void CanGetAllUsingGenericType()
		{
			Enumeration.GetAll<FooBarEnumeration>().ShouldBeEqualTo(FooBarEnumeration.Foo, FooBarEnumeration.Bar);
		}

		[Fact]
		public void CanGetAllUsingType()
		{
			Enumeration.GetAll(typeof(FooBarEnumeration)).ShouldBeEqualTo(FooBarEnumeration.Foo, FooBarEnumeration.Bar);
		}

		[Fact]
		public void CanGetAllNamesUsingGenericType()
		{
			Enumeration.GetAllNames<FooBarEnumeration>().ShouldBeEqualTo("Foo", "Bar");
		}

		[Fact]
		public void CanGetAllNamesUsingType()
		{
			Enumeration.GetAllNames(typeof(FooBarEnumeration)).ShouldBeEqualTo("Foo", "Bar");
		}

		[Fact]
		public void FromValue()
		{
			Enumeration.FromValue<FooBarEnumeration>(FooBarEnumeration.Foo.Value).ShouldEqual(FooBarEnumeration.Foo);
		}

		[Fact]
		public void FromValueOrDefault()
		{
			Enumeration.FromValueOrDefault<FooBarEnumeration>(FooBarEnumeration.Foo.Value).ShouldEqual(FooBarEnumeration.Foo);
		}

		[Fact]
		public void FromValueThrowsIfNotFound()
		{
			var ex = Assert.Throws<ArgumentException>(() => Enumeration.FromValue<FooBarEnumeration>(0));
			ex.Message.ShouldEqual("'0' is not a valid value for '{0}'.", typeof(FooBarEnumeration).FullName);
		}

		[Fact]
		public void FromValueOrDefaultReturnsNullIfNotFound()
		{
			Enumeration.FromValueOrDefault<FooBarEnumeration>(0).ShouldBeNull();
		}

		[Fact]
		public void FromName()
		{
			Enumeration.FromName<FooBarEnumeration>(FooBarEnumeration.Foo.Name).ShouldEqual(FooBarEnumeration.Foo);
		}

		[Fact]
		public void FromNameOrDefault()
		{
			Enumeration.FromNameOrDefault<FooBarEnumeration>(FooBarEnumeration.Foo.Name).ShouldEqual(FooBarEnumeration.Foo);
		}

		[Fact]
		public void FromNameThrowsIfNotFound()
		{
			var ex = Assert.Throws<ArgumentException>(() => Enumeration.FromName<FooBarEnumeration>("Rubbish"));
			ex.Message.ShouldEqual("'Rubbish' is not a valid name for '{0}'.", typeof(FooBarEnumeration).FullName);
		}

		[Fact]
		public void FromNameOrDefaultReturnsNullIfNotFound()
		{
			Enumeration.FromNameOrDefault<FooBarEnumeration>("Rubbish").ShouldBeNull();
		}

		[Fact]
		public void CanTellDifferenceBetweenEnumerationsWithDifferentValueOfSameType()
		{
			FooBarEnumeration.Foo.ShouldNotEqual(FooBarEnumeration.Bar);
		}

		[Fact]
		public void CanTellDifferenceBetweenEnumerationsWithSameValueOfDifferentTypes()
		{
			FooBarEnumeration.Foo.ShouldNotEqual(AnotherFooBarEnumeration.Foo);
		}

		[Fact]
		public void CanTellDifferenceBetweenEnumerationsWithDifferentValueOfDifferentTypes()
		{
			FooBarEnumeration.Foo.ShouldNotEqual(AnotherFooBarEnumeration.Bar);
		}

		[Fact]
		public void CanTellEqualityBetweenStrategiesWithSameIdAndType()
		{
			Strategy.Foo.ShouldEqual(new FooStrategy(1, "Rubbish", "Rubbish"));
		}

		[Fact]
		public void ShouldCompareCorrectlyForEquality()
		{
			FooBarEnumeration.Foo.CompareTo(FooBarEnumeration.Bar).ShouldEqual(-1);
			FooBarEnumeration.Foo.CompareTo(FooBarEnumeration.Foo).ShouldEqual(0);
			FooBarEnumeration.Bar.CompareTo(FooBarEnumeration.Foo).ShouldEqual(1);
		}

		[Fact]
		public void ShouldCompareObjectsOfOtherTypes()
		{
			FooBarEnumeration.Foo.Equals(DBNull.Value).ShouldBeFalse();
		}

		[Fact]
		public void CanSortEnumerationValues()
		{
			var array = new[] { FooBarEnumeration.Bar, FooBarEnumeration.Foo };
			Array.Sort(array);
			array.ShouldBeEqualTo(FooBarEnumeration.Foo, FooBarEnumeration.Bar);
		}

		[Fact]
		public void ShouldCorrectlyReturnHashCodeOfValue()
		{
			FooBarEnumeration.Foo.GetHashCode().ShouldEqual(FooBarEnumeration.Foo.Value.GetHashCode());
		}

		public abstract class Strategy : EntityEnumeration<Strategy>
		{
			public static readonly Strategy Foo = new FooStrategy(1, "Foo", "Foo");
			public static readonly Strategy Bar = new BarStrategy(2, "Bar", "Bar");

			protected Strategy(int id, string name, string text)
				: base(id, name, text)
			{
			}

			public abstract string GetString();
		}

		public class FooStrategy : Strategy
		{
			public FooStrategy(int value, string name, string text)
				: base(value, name, text)
			{
			}

			public override string GetString()
			{
				return "Foo";
			}
		}

		public class BarStrategy : Strategy
		{
			public BarStrategy(int value, string name, string text)
				: base(value, name, text)
			{
			}

			public override string GetString()
			{
				return "Bar";
			}
		}

		public class FooBarEnumeration : ComparableEnumeration<FooBarEnumeration>
		{
			public static readonly FooBarEnumeration Foo = new FooBarEnumeration(1, "Foo", "Foo");
			public static readonly FooBarEnumeration Bar = new FooBarEnumeration(2, "Bar", "Bar");

			private FooBarEnumeration(int value, string name, string text)
				: base(value, name, text)
			{
			}
		}

		public class AnotherFooBarEnumeration : ComparableEnumeration<AnotherFooBarEnumeration>
		{
			public static readonly AnotherFooBarEnumeration Foo = new AnotherFooBarEnumeration(1, "Foo", "Foo");
			public static readonly AnotherFooBarEnumeration Bar = new AnotherFooBarEnumeration(2, "Bar", "Bar");

			private AnotherFooBarEnumeration(int value, string name, string text)
				: base(value, name, text)
			{
			}
		}
	}
}