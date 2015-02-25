using System;
using System.Collections.Generic;
using Maxfire.Core.Extensions;
using Maxfire.TestCommons.AssertExtensions;
using Xunit;

namespace Maxfire.Core.UnitTests.Extensions
{
	public class EnumerableExtensionsTester
	{
		[Fact]
		public void HasCountEqualTo()
		{
			Assert.Throws<ArgumentNullException>(() => ((IEnumerable<string>) null).HasCountEqualTo(0));
			new string[] {}.HasCountEqualTo(-1).ShouldBeFalse();
			new string[] {}.HasCountEqualTo(0).ShouldBeTrue();
			new string[] {}.HasCountEqualTo(1).ShouldBeFalse();
			new[] {"Morten"}.HasCountEqualTo(0).ShouldBeFalse();
			new[] {"Morten"}.HasCountEqualTo(1).ShouldBeTrue();
			new[] {"Morten"}.HasCountEqualTo(2).ShouldBeFalse();
			new[] {"Morten", "Maxild"}.HasCountEqualTo(1).ShouldBeFalse();
			new[] {"Morten", "Maxild"}.HasCountEqualTo(2).ShouldBeTrue();
			new[] {"Morten", "Maxild"}.HasCountEqualTo(3).ShouldBeFalse();
		}

		[Fact]
		public void HasCountGreaterThan()
		{
			new string[] { }.HasCountGreaterThan(-1).ShouldBeTrue();
			new string[] { }.HasCountGreaterThan(0).ShouldBeFalse();
			new string[] { }.HasCountGreaterThan(1).ShouldBeFalse();
			new[] { "Morten" }.HasCountGreaterThan(0).ShouldBeTrue();
			new[] { "Morten" }.HasCountGreaterThan(1).ShouldBeFalse();
			new[] { "Morten" }.HasCountGreaterThan(2).ShouldBeFalse();
			new[] { "Morten" }.HasCountGreaterThan(2).ShouldBeFalse();
			new[] { "Morten", "Maxild" }.HasCountGreaterThan(0).ShouldBeTrue();
			new[] { "Morten", "Maxild" }.HasCountGreaterThan(1).ShouldBeTrue();
			new[] { "Morten", "Maxild" }.HasCountGreaterThan(2).ShouldBeFalse();
			new[] { "Morten", "Maxild" }.HasCountGreaterThan(3).ShouldBeFalse();
		}
	}
}