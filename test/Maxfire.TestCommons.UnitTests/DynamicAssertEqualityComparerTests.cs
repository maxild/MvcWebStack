using System;
using Maxfire.TestCommons.AssertExtensibility;
using Xunit;

namespace Maxfire.TestCommons.UnitTests
{
	public class DynamicAssertEqualityComparerTests
	{
		class MyInt : IEquatable<MyInt>, IEquatable<int>
		{
			public MyInt(int value)
			{
				Value = value;
			}

			public int Value { get; private set; }

			public bool Equals(MyInt other)
			{
				return other != null && Value == other.Value;
			}

			public bool Equals(int other)
			{
				return Value == other;
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj)) return false;
				if (ReferenceEquals(this, obj)) return true;
				if (obj is int) return Equals((int) obj);
				return Equals(obj as MyInt);
			}

			public override int GetHashCode()
			{
				return Value;
			}
		}

		class MyOtherInt : IEquatable<MyOtherInt>, IEquatable<object>
		{
			public MyOtherInt(int value)
			{
				Value = value;
			}

			public int Value { get; private set; }

			public bool Equals(MyOtherInt other)
			{
				return other != null && Value == other.Value;
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj)) return false;
				if (ReferenceEquals(this, obj)) return true;
				if (obj is int) return Value == (int)obj;
				return Equals(obj as MyInt);
			}

			public override int GetHashCode()
			{
				return Value;
			}
		}

		[Fact]
		public void HandlesNull()
		{
			var sut = new DynamicAssertEqualityComparer(skipTypeCheck: true);
			Assert.True(sut.Equals(null, null));
			Assert.False(sut.Equals(null, 12));
			Assert.False(sut.Equals(12, null));
			Assert.True(sut.Equals(12, 12));
		}

		[Fact]
		public void IsSymmetric()
		{
			var sut = new DynamicAssertEqualityComparer(skipTypeCheck: true);

			Assert.True(sut.Equals(12, new MyInt(12)));
			Assert.True(sut.Equals(new MyInt(12), 12));
			Assert.False(sut.Equals(13, new MyInt(12)));
			Assert.False(sut.Equals(new MyInt(12), 13));

			Assert.True(sut.Equals(12, new MyOtherInt(12)));
			Assert.True(sut.Equals(new MyOtherInt(12), 12));
			Assert.False(sut.Equals(13, new MyOtherInt(12)));
			Assert.False(sut.Equals(new MyOtherInt(12), 13));
		}

		[Fact]
		public void TypeCheck()
		{
			var sut = new DynamicAssertEqualityComparer();
			Assert.False(sut.Equals(12, new MyInt(12)));
		}
	}
}