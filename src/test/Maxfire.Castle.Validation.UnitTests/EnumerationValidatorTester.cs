using System;
using Maxfire.Castle.Validation.Validators;
using Maxfire.Core;
using Xunit;

namespace Maxfire.Castle.Validation.UnitTests
{
	public class EnumerationValidatorTester
	{
		enum Foo {}
		
		class YesOrNo : Enumeration
		{
			public static readonly YesOrNo No  = new YesOrNo(0, "No");
			public static readonly YesOrNo Yes  = new YesOrNo(1, "Yes");
			
			private YesOrNo(int value, string name)
				: base(value, name, name)
			{
			}
		}

		[Fact]
		public void CtorThrowsOnAssignemtIncompatibleType()
		{
			Assert.Throws<ArgumentException>(() => new EnumerationValidator(typeof (Foo)));
		}

		[Fact]
		public void Ctor()
		{
			new EnumerationValidator(typeof(YesOrNo));
		}
	}
}