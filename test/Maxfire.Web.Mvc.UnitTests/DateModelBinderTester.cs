using System;
using System.Web.Mvc;
using Maxfire.TestCommons.AssertExtensions;
using Xunit;

namespace Maxfire.Web.Mvc.UnitTests
{
	public class DateModelBinderTester
	{
		public DateModelBinderTester()
		{
			Sut = new DateModelBinder();
		}

		public DateModelBinder Sut { get; }

		[Fact]
		public void NoFieldReturnsNullWithNoBindingErrors()
		{
			var valueProvider = new SimpleValueProvider();

			var bindingContext = new ModelBindingContext
			{
				ModelName = "birthday",
				ValueProvider = valueProvider
			};

			object result = Sut.BindModel(null, bindingContext);

			bindingContext.ModelState.IsValid.ShouldBeTrue();
			result.ShouldBeNull();
		}

		[Fact]
		public void SingleEmptyFieldReturnsNullWithNoBindingErrors()
		{
			var valueProvider = new SimpleValueProvider { { "birthday", "" } };

			var bindingContext = new ModelBindingContext
			{
				ModelName = "birthday",
				ValueProvider = valueProvider
			};

			object result = Sut.BindModel(null, bindingContext);

			bindingContext.ModelState.IsValid.ShouldBeTrue();
			result.ShouldBeNull();
		}

		[Fact]
		public void YearMonthDayEmptyFieldsReturnsNullWithNoBindingErrors()
		{
			var valueProvider = new SimpleValueProvider { { "birthday.year", "" }, { "birthday.month", "" }, { "birthday.day", "" } };

			var bindingContext = new ModelBindingContext
			{
				ModelName = "birthday",
				ValueProvider = valueProvider
			};

			object result = Sut.BindModel(null, bindingContext);

			bindingContext.ModelState.IsValid.ShouldBeTrue();
			result.ShouldBeNull();
		}

		[Fact]
		public void YearMonthDayMissingOrEmptyFieldsReturnsNullWithNoBindingErrors()
		{
			var valueProvider = new SimpleValueProvider { { "birthday.month", "" }, { "birthday.day", "" } };

			var bindingContext = new ModelBindingContext
			{
				ModelName = "birthday",
				ValueProvider = valueProvider
			};

			object result = Sut.BindModel(null, bindingContext);

			bindingContext.ModelState.IsValid.ShouldBeTrue();
			result.ShouldBeNull();
		}

		[Fact]
		public void SomeMissingFieldsReturnsNullWithoutBindingErrors()
		{
			var valueProvider = new SimpleValueProvider { { "birthday.month", "12" }, { "birthday.day", "1" } };

			var bindingContext = new ModelBindingContext
			{
				ModelName = "birthday",
				ValueProvider = valueProvider
			};

			object result = Sut.BindModel(null, bindingContext);

			bindingContext.ModelState.IsValid.ShouldBeTrue();
			result.ShouldBeNull();
		}

		[Fact]
		public void SomeEmptyFieldsReturnsNullWithoutBindingErrors()
		{
			var valueProvider = new SimpleValueProvider { { "birthday.year", "" }, { "birthday.month", "12" }, { "birthday.day", "1" } };

			var bindingContext = new ModelBindingContext
			{
				ModelName = "birthday",
				ValueProvider = valueProvider
			};

			object result = Sut.BindModel(null, bindingContext);

			bindingContext.ModelState.IsValid.ShouldBeTrue();
			result.ShouldBeNull();
		}

		[Fact]
		public void CanHandleSingleField()
		{
			var valueProvider = new SimpleValueProvider { { "", "3/6-1970" } };

			var bindingContext = new ModelBindingContext
			{
				ModelName = "",
				FallbackToEmptyPrefix = true,
				ValueProvider = valueProvider
			};

			DateTime? result = (DateTime?)Sut.BindModel(null, bindingContext);

			bindingContext.ModelState.IsValid.ShouldBeTrue();
			result.ShouldEqual(new DateTime(1970, 6, 3));
		}

		[Fact]
		public void CanHandleSingleNamedField()
		{
			var valueProvider = new SimpleValueProvider { { "birthday", "3/6-1970" } };

			var bindingContext = new ModelBindingContext
			{
				ModelName = "birthday",
				ValueProvider = valueProvider
			};

			DateTime? result = (DateTime?)Sut.BindModel(null, bindingContext);

			bindingContext.ModelState.IsValid.ShouldBeTrue();
			result.ShouldEqual(new DateTime(1970, 6, 3));
		}

		[Fact]
		public void CanHandleSingleNamedField2()
		{
			var valueProvider = new SimpleValueProvider { { "birthday", "03-06-1970" } };

			var bindingContext = new ModelBindingContext
			{
				ModelName = "birthday",
				ValueProvider = valueProvider
			};

			DateTime? result = (DateTime?)Sut.BindModel(null, bindingContext);

			bindingContext.ModelState.IsValid.ShouldBeTrue();
			result.ShouldEqual(new DateTime(1970, 6, 3));
		}

		[Fact]
		public void CanHandleSingleNamedField3()
		{
			var valueProvider = new SimpleValueProvider { { "birthday", "1970-06-03" } };

			var bindingContext = new ModelBindingContext
			{
				ModelName = "birthday",
				ValueProvider = valueProvider
			};

			DateTime? result = (DateTime?)Sut.BindModel(null, bindingContext);

			bindingContext.ModelState.IsValid.ShouldBeTrue();
			result.ShouldEqual(new DateTime(1970, 6, 3));
		}

		[Fact]
		public void CanHandleMultipleFields()
		{
			var valueProvider = new SimpleValueProvider
			{ 
				{ "day", "12" },
				{ "month", "2" },
				{ "year", "1964" }
			};

			var bindingContext = new ModelBindingContext
			{
				FallbackToEmptyPrefix = true,
				ModelName = "",
				ValueProvider = valueProvider
			};

			var result = (DateTime?)Sut.BindModel(null, bindingContext);

			bindingContext.ModelState.IsValid.ShouldBeTrue();
			result.ShouldEqual(new DateTime(1964, 2, 12));
		}

		[Fact]
		public void CanHandleNamedMultipleFields()
		{
			var valueProvider = new SimpleValueProvider
			{ 
				{ "birthday.day", "12" },
				{ "birthday.month", "2" },
				{ "birthday.year", "1964" }
			};

			var bindingContext = new ModelBindingContext
			{
				ModelName = "birthday",
				ValueProvider = valueProvider
			};

			var result = (DateTime?)Sut.BindModel(null, bindingContext);

			bindingContext.ModelState.IsValid.ShouldBeTrue();
			result.ShouldEqual(new DateTime(1964, 2, 12));
		}

		[Fact]
		public void CanSerialize()
		{
			var values = Sut.GetValues(new DateTime(1970, 6, 3), string.Empty);

			values.Count.ShouldEqual(3);
			values["Day"].ShouldEqual("3");
			values["Month"].ShouldEqual("6");
			values["Year"].ShouldEqual("1970");
		}

		[Fact]
		public void CanSerializeWithPrefix()
		{
			var values = Sut.GetValues(new DateTime(1970, 6, 3), "foedselsdato");

			values.Count.ShouldEqual(3);
			values["foedselsdato.day"].ShouldEqual("3");
			values["foedselsdato.month"].ShouldEqual("6");
			values["foedselsdato.year"].ShouldEqual("1970");
		}
	}
}
