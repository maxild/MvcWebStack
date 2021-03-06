using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Maxfire.TestCommons.AssertExtensions;
using Xunit;

namespace Maxfire.Web.Mvc.UnitTests
{
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
	public class DefaultNameValueSerializerTester
	{
		private readonly TestableDefaultNameValueSerializer _serializer;

		public DefaultNameValueSerializerTester()
		{
			_serializer = new TestableDefaultNameValueSerializer();
		}

		[Fact]
		public void CanHandleComplexType()
		{
			var values = _serializer.GetValues(new {Navn = "Morten", Alder = 40});

			values.Count.ShouldEqual(2);
			values["Navn"].ShouldEqual("Morten");
			values["Alder"].ShouldEqual("40");
		}

		[Fact]
		public void CanHandleComplexTypeWithPrefix()
		{
			var values = _serializer.GetValues(new { Navn = "Morten", Alder = 40 }, "prefix");

			values.Count.ShouldEqual(2);
			values["prefix.Navn"].ShouldEqual("Morten");
			values["prefix.Alder"].ShouldEqual("40");
		}

		[Fact]
		public void CanHandleCustomDateTimeSerializer()
		{
			var values = _serializer.GetValues(new DateTime(1970, 6, 3));

			values.Count.ShouldEqual(3);
			values["Day"].ShouldEqual("3");
			values["Month"].ShouldEqual("6");
			values["Year"].ShouldEqual("1970");
		}

		[Fact]
		public void CanHandleCustomDateTimeSerializerWithPrefix()
		{
			var values = _serializer.GetValues(new DateTime(1970, 6, 3), "foedselsdato");

			values.Count.ShouldEqual(3);
			values["foedselsdato.Day"].ShouldEqual("3");
			values["foedselsdato.Month"].ShouldEqual("6");
			values["foedselsdato.Year"].ShouldEqual("1970");
		}

		[Fact]
		public void CanHandleList()
		{
			var values = _serializer.GetValues(
				new List<Foo>
					{
						new Foo {Navn = "John", Alder = 67},
						new Foo {Navn = "Liselotte", Alder = 33}
					});

			values.Count.ShouldEqual(4);
			values["[0].Navn"].ShouldEqual("John");
			values["[0].Alder"].ShouldEqual("67");
			values["[1].Navn"].ShouldEqual("Liselotte");
			values["[1].Alder"].ShouldEqual("33");
		}

		[Fact]
		public void CanHandleListWithPrefix()
		{
			var values = _serializer.GetValues(
				new List<Foo>
					{
						new Foo {Navn = "John", Alder = 67},
						new Foo {Navn = "Liselotte", Alder = 33}
					}, "prefix");

			values.Count.ShouldEqual(4);
			values["prefix[0].Navn"].ShouldEqual("John");
			values["prefix[0].Alder"].ShouldEqual("67");
			values["prefix[1].Navn"].ShouldEqual("Liselotte");
			values["prefix[1].Alder"].ShouldEqual("33");
		}

		class Foo
		{
			public string Navn { get; set; }
			public int Alder { get; set; }
		}

		class TestableDefaultNameValueSerializer : DefaultNameValueSerializer
		{
			protected override INameValueSerializer GetSerializerCore(Type modelType)
			{
				if (modelType == typeof(DateTime))
				{
					return new DateTimeNameValueSerializer();
				}

				return base.GetSerializerCore(modelType);
			}
		}

		class DateTimeNameValueSerializer : SimpleNameValueSerializer<DateTime>
		{
			protected override IDictionary<string, object> GetValuesCore(DateTime value, string prefix)
			{
				return string.IsNullOrEmpty(prefix) ? 
					new Dictionary<string, object> { { "Day", value.Day.ToString() }, { "Month", value.Month.ToString() }, { "Year", value.Year.ToString() } } : 
					new Dictionary<string, object> { { prefix + ".Day", value.Day.ToString() }, { prefix + ".Month", value.Month.ToString() }, { prefix + ".Year", value.Year.ToString() } };
			}
		}
	}
}
