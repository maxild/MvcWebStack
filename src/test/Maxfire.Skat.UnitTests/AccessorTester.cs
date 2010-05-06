using System.ComponentModel;
using Maxfire.Core.Reflection;
using Maxfire.TestCommons.AssertExtensions;
using Xunit;

namespace Maxfire.Skat.UnitTests
{
	public class AccessorTester
	{
		class Point
		{
			public int X { get; set; }
			public int Y { get; set; }
		}

		[Fact]
		public void UsingPropertyAccessor()
		{
			var p = new Point { X = 10, Y = 20 };

			var propertyX = IntrospectionOf<Point>.GetAccessorFor(x => x.X);
			var propertyY = IntrospectionOf<Point>.GetAccessorFor(x => x.Y);
			
			propertyX.GetValue(p).ShouldEqual(10);
			propertyY.GetValue(p).ShouldEqual(20);

			propertyX.SetValue(p, 12);
			propertyY.SetValue(p, 24);

			p.X.ShouldEqual(12);
			p.Y.ShouldEqual(24);
		}

		[Fact]
		public void UsingTypeDescriptor()
		{
			var p = new Point { X = 10, Y = 20 };

			var propertyDescriptorForX = TypeDescriptor.GetProperties(p)["X"];
			var propertyDescriptorForY = TypeDescriptor.GetProperties(p)["Y"];

			propertyDescriptorForX.GetValue(p).ShouldEqual(10);
			propertyDescriptorForY.GetValue(p).ShouldEqual(20);
			
			propertyDescriptorForX.SetValue(p, 12);
			propertyDescriptorForY.SetValue(p, 24);

			p.X.ShouldEqual(12);
			p.Y.ShouldEqual(24);
		}
	}
}