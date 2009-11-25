using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using Castle.Components.Validator;
using Maxfire.Core.Reflection;
using Maxfire.TestCommons;
using Maxfire.TestCommons.AssertExtensions;
using Maxfire.Web.Mvc.Validators;
using Xunit;

namespace Maxfire.Web.Mvc.UnitTests
{
	[UsedImplicitly]
	public class ValidationModelBinderTester : IDisposable
	{
		class Order
		{
			[LabeledValidateInteger]
			public int Id { get; set; }

			public Address Address { get; set; }

			public Orderline[] Orderlines { get; set; }

			public override string ToString()
			{
				return "Order with id = " + Id;
			}
		}

		class Address
		{
			[DisplayName("PostNr"), LabeledValidateLength(4, ExecutionOrder = 1), LabeledValidateInteger(ExecutionOrder = 2)]
			public string PostalCode { get; set; }

			public override string ToString()
			{
				return "Address with postalcode = " + PostalCode;
			}
		}

		class Orderline
		{
			[DisplayName("Pris"), LabeledValidateMoney]
			public decimal Price { get; set; }

			[DisplayName("Antal"), LabeledValidateInteger]
			public int Qty { get; set; }

			public override string ToString()
			{
				return "OrderLine with (Qty, Price) = (" + Qty + ", " + Price + ")";
			}
		}

		public ValidationModelBinderTester()
		{
			string appPath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(ValidationModelBinderTester)).CodeBase);
			string file = Path.Combine(appPath, "log.txt");
			// we cannot use URL "file:\\C:\\dev\\projects\\totalberegner\\src\\test\\UnitTests\\bin\\Debug\\log.txt"
			file = file.Replace("file:\\", "");
			Debug.Listeners.Add(new TextWriterTraceListener(file, "file-listener"));
		}

		public void Dispose()
		{
			Debug.Flush();
			Debug.Listeners.Remove("file-listener");
		}

		class Foo
		{
			[DisplayName("Alder")]
			public int Age { get; set; }

			public string Name { get; set; }
		}

		[Fact]
		public void GetDisplayNameOfPropertyWithLabelAttribute()
		{
			Assert.Equal("Alder", ReflectionHelper.GetProperty<Foo>(x => x.Age).GetDisplayName());
		}

		[Fact]
		public void GetDisplayNameOfPropertyWithoutLabelAttribute()
		{
			Assert.Equal("Name", ReflectionHelper.GetProperty<Foo>(x => x.Name).GetDisplayName());
		}

		[Fact]
		public void CanDeserializeValidOrder()
		{
			// Arrange
			
			var binder = new TestableValidationModelBinder<Order>();
			binder.RequestParams = new NameValueCollection
			                       	{
			                       		{ "Id", "1" },
			                       		{ "Address.PostalCode", "3400" },
			                       		{ "OrderLines[0].Price", "10" },
			                       		{ "OrderLines[0].Qty", "1" },
			                       		{ "OrderLines[1].Price", "20" },
			                       		{ "OrderLines[1].Qty", "2" },
			                       	};
			
			// Act
			var order = binder.BindModel();

			// Assert
			binder.ModelState.IsValid.ShouldBeTrue();

			order.ShouldNotBeNull();
			order.Id.ShouldEqual(1);
			order.Address.ShouldNotBeNull();
			order.Address.PostalCode.ShouldEqual("3400");
			order.Orderlines.ShouldNotBeNull();
			order.Orderlines.Length.ShouldEqual(2);
			order.Orderlines[0].Price.ShouldEqual(10);
			order.Orderlines[0].Qty.ShouldEqual(1);
			order.Orderlines[1].Price.ShouldEqual(20);
			order.Orderlines[1].Qty.ShouldEqual(2);
		}

		[Fact]
		public void CanDeserializeInvalidOrder()
		{
			// Arrange
			var binder = new TestableValidationModelBinder<Order>();
			binder.RequestParams = new NameValueCollection
			                       	{
			                       		{ "Id", "not_number" },
			                       		{ "Address.PostalCode", "too_long" },
			                       		{ "OrderLines[0].Price", "invalid_format" },
			                       		{ "OrderLines[0].Qty", "invalid_format" },
			                       		{ "OrderLines[1].Price", "20" },
			                       		{ "OrderLines[1].Qty", "2.5" },
			                       	};

			// Act
			Order order;
			using (new CurrentCultureScope(""))
			{
				// We use invariant culture in case a language specific resource with key 'PropertyValueInvalid' has been added.
				order = binder.BindModel();
			}

			// Assert
			binder.IsInputValid.ShouldBeFalse();

			binder.IsInputValidFor(x => x.Id).ShouldBeFalse();
			binder.GetModelErrorsFor(x => x.Id).Count.ShouldEqual(1);
			binder.GetModelErrorsFor(x => x.Id)[0].ErrorMessage.ShouldEqual("The value 'not_number' is not valid for Id.");
			
			binder.IsInputValidFor(x => x.Address.PostalCode).ShouldBeFalse();
			binder.GetModelErrorsFor(x => x.Address.PostalCode).Count.ShouldEqual(2);
			binder.GetModelErrorsFor(x => x.Address.PostalCode)[0].ErrorMessage.ShouldEqual("Feltet 'PostNr' skal indeholde 4 karakterer.");
			binder.GetModelErrorsFor(x => x.Address.PostalCode)[1].ErrorMessage.ShouldEqual("Feltet 'PostNr' indeholder ikke et validt heltal.");
			
			binder.IsInputValidFor(x => x.Orderlines[0].Price).ShouldBeFalse();
			binder.GetModelErrorsFor(x => x.Orderlines[0].Price).Count.ShouldEqual(1);
			binder.GetModelErrorsFor(x => x.Orderlines[0].Price)[0].ErrorMessage.ShouldEqual("The value 'invalid_format' is not valid for Pris.");
			
			binder.IsInputValidFor(x => x.Orderlines[0].Qty).ShouldBeFalse();
			binder.GetModelErrorsFor(x => x.Orderlines[0].Qty).Count.ShouldEqual(1);
			binder.GetModelErrorsFor(x => x.Orderlines[0].Qty)[0].ErrorMessage.ShouldEqual("The value 'invalid_format' is not valid for Antal.");
			
			binder.IsInputValidFor(x => x.Orderlines[1].Price).ShouldBeTrue();
			binder.GetModelErrorsFor(x => x.Orderlines[1].Price).Count.ShouldEqual(0);
			
			binder.IsInputValidFor(x => x.Orderlines[1].Qty).ShouldBeFalse();
			binder.GetModelErrorsFor(x => x.Orderlines[1].Qty).Count.ShouldEqual(1);
			binder.GetModelErrorsFor(x => x.Orderlines[1].Qty)[0].ErrorMessage.ShouldEqual("The value '2.5' is not valid for Antal.");
			
			// Values are default (if format exception), or attempted (if no format exception)
			order.ShouldNotBeNull();
			order.Id.ShouldEqual(0);
			order.Address.ShouldNotBeNull();
			order.Address.PostalCode.ShouldEqual("too_long");
			order.Orderlines.ShouldNotBeNull();
			order.Orderlines.Length.ShouldEqual(2);
			order.Orderlines[0].Price.ShouldEqual(0);
			order.Orderlines[0].Qty.ShouldEqual(0);
			order.Orderlines[1].Price.ShouldEqual(20);
			order.Orderlines[1].Qty.ShouldEqual(0);
		}

		public class RangeWithValidateSelf
		{
			public int Minimum { get; set; }
			public int Maximum { get; set; }

			[ValidateSelf]
			public void Validate(ErrorSummary summary)
			{
				if (Minimum > Maximum)
				{
					summary.RegisterErrorMessage("Minimum", "Minimum must be less than or equal to Maximum");
					summary.RegisterErrorMessage("Maximum", "Minimum must be less than or equal to Maximum");
				}
			}
		}

		[Fact]
		public void OnModelUpdatedWithValidateSelfAttribute()
		{
			var model = new RangeWithValidateSelf { Minimum = 250, Maximum = 100 };
			var modelBinder = new TestableValidationModelBinder<RangeWithValidateSelf>(model);

			modelBinder.OnModelUpdated();

			modelBinder.IsInputValid.ShouldBeFalse();
			modelBinder.IsInputValidFor(x => x.Minimum).ShouldBeFalse();
			modelBinder.IsInputValidFor(x => x.Maximum).ShouldBeFalse();
			modelBinder.ModelState.Count.ShouldEqual(2);
			var stateModel1 = modelBinder.GetModelStateFor(x => x.Minimum);
			var stateModel2 = modelBinder.GetModelStateFor(x => x.Maximum);
			stateModel1.ShouldNotBeNull();
			stateModel2.ShouldNotBeNull();
			stateModel1.Errors.Single().ErrorMessage.ShouldEqual("Minimum must be less than or equal to Maximum");
			stateModel2.Errors.Single().ErrorMessage.ShouldEqual("Minimum must be less than or equal to Maximum");
		}

		[Fact]
		public void test()
		{
			Type t = typeof (IList<>).GetGenericTypeDefinition();

			Type t2 = t.MakeGenericType(typeof (int));

			Assert.True(t.IsGenericType);
			Assert.True(t.IsGenericTypeDefinition);
			Assert.True(t2.IsGenericType);
			Assert.False(t2.IsGenericTypeDefinition);
			
		}
	}
}