using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Maxfire.TestCommons;
using Maxfire.TestCommons.AssertExtensions;
using Xunit;

namespace Maxfire.Web.Mvc.UnitTests
{
	public partial class ValidationModelBinderSpecs
	{
		///////////////////////////////////////////////////////////////
		// Port of the NameValueDeserializer test cases 
		// 
		// Differences are (when using DefaultModelBinder):
		// 1) Empty prefix will not throw.
		// 2) 'true,false' not supported for System.Boolean
		// 3) Collections defaults to null, when no keys 'point' to them
		///////////////////////////////////////////////////////////////
		[UsedImplicitly]
		public class PortedFromNameValueDeserializer
		{
			[Fact]
			public void should_throw_when_context_is_null()
			{
				var binder = new TestableValidationModelBinder<Customer> { Context = null };
				Assert.Throws<ArgumentNullException>(() => binder.BindModel());
			}

			[Fact]
			public void should_throw_when_metadata_is_null()
			{
				var binder = new TestableValidationModelBinder<Customer> { Context = { ModelMetadata = null } };
				Assert.Throws<NullReferenceException>(() => binder.BindModel());
			}

			// Note: Not possible to have metadata with ModelType == null

			// Note: Context.ValueProvider can never be null (see getter in ModelBindingContext)
			
			///////////////////////////////////////////////////////////////////////////////////////////
			// In ControllerActionInvoker when binding to the parameters of an action the binding 
			// context is defined with:
			//
			// ModelName = parameterDescriptor.BindingInfo.Prefix ?? parameterDescriptor.ParameterName;
			// FallbackToEmptyPrefix = (parameterDescriptor.BindingInfo.Prefix == null)
			//
			// I.e. Prefix == "" or Prefix == "customer" means no fallback, where as Prefix == null (as
			// when no BindAttribute defined) means fallback with empty prefix (that is all keys are 
			// interpretated as scoped under Customer). Prefix == null also means the parameter name will
			// be used as the prefix (convention over configuration).
			///////////////////////////////////////////////////////////////////////////////////////////

			[Fact]
			public void when_no_key_have_prefix_and_no_fallback_to_empty_prefix_returns_null_customer()
			{
				var binder = new TestableValidationModelBinder<Customer>
				             	{
				             		Context = { FallbackToEmptyPrefix = false },
				             		Prefix = "prefix"
				             	};
				binder.RequestParams["junk"] = "stuff";
				binder.BindModel().ShouldBeNull();
			}

			[Fact]
			public void when_no_key_have_prefix_and_fallback_to_empty_prefix_returns_empty_customer()
			{
				var binder = new TestableValidationModelBinder<Customer>
				             	{
				             		Context = { FallbackToEmptyPrefix = true },
				             		Prefix = "prefix"
				             	};
				binder.RequestParams["junk"] = "stuff";
				var customer = binder.BindModel();
				customer.ShouldNotBeNull();
				customer.Id.ShouldEqual(0);
				customer.Employee.ShouldBeNull();
			}

			[Fact]
			public void when_prefix_is_empty_returns_empty_customer()
			{
				var binder = new TestableValidationModelBinder<Customer>
				             	{
				             		Context = { FallbackToEmptyPrefix = true },
				             		Prefix = ""
				             	};
				binder.RequestParams["junk"] = "stuff";
				var customer = binder.BindModel();
				customer.ShouldNotBeNull();
				customer.Id.ShouldEqual(0);
				customer.Employee.ShouldBeNull();
			}

			[Fact]
			public void when_prefix_is_empty_returns_empty_customer_again()
			{
				var binder = new TestableValidationModelBinder<Customer>
				             	{
				             		Context = { FallbackToEmptyPrefix = false },
				             		Prefix = ""
				             	};
				binder.RequestParams["junk"] = "stuff";
				var customer = binder.BindModel();
				customer.ShouldNotBeNull();
				customer.Id.ShouldEqual(0);
				customer.Employee.ShouldBeNull();
			}

			[UsedImplicitly]
			class Customer
			{
				public int Id { get; set; }

				public Employee Employee { get; set; }
			}

			[UsedImplicitly]
			class Employee
			{
				private readonly Phone _phone = new Phone();
				private readonly List<Phone> _otherPhones = new List<Phone>();

				public int Id { get; set; }

				public Phone Phone
				{
					get { return _phone; }
				}

				private const Phone _batPhone = null;

				public Phone BatPhone
				{
					get { return _batPhone; }
				}

				public IList<Phone> OtherPhones
				{
					get { return _otherPhones; }
				}

				public Customer Customer { get; set; }

				private int _age;

				public int Age
				{
					get { return _age; }
					set
					{
						if (value < 0) throw new ArgumentException("Age must be greater than 0");
						_age = value;
					}
				}
			}

			class Phone
			{
				private readonly List<string> _areaCodes = new List<string>();

				public string Number { get; set; }

				public IList<string> AreaCodes
				{
					get { return _areaCodes; }
				}
			}

			[Fact]
			public void list_property_should_be_skipped_if_not_initialized_and_readonly()
			{
				var binder = new TestableValidationModelBinder<GenericListClass>();
				binder.RequestParams["list.ReadonlyIds[0]"] = "10";
				binder.RequestParams["list.ReadonlyIds[1]"] = "20";
				binder.Prefix = "list";

				var list = binder.BindModel();

				list.ShouldNotBeNull();
				list.ReadonlyIds.ShouldBeNull();
			}

			[Fact]
			public void ErrorsSettingPropertiesAreIgnored()
			{
				var binder = new TestableValidationModelBinder<Employee> { Prefix = "emp" };
				binder.RequestParams["emp.Age"] = "-1";

				var emp = binder.BindModel();

				// Todo: model exception error should be asserted here
				emp.ShouldNotBeNull();
				emp.Age.ShouldEqual(0);
			}

			[Fact]
			public void ComplexPropertyIsSkippedIfNotInitializedAndReadOnly()
			{
				var binder = new TestableValidationModelBinder<Employee> { Prefix = "emp" };
				binder.RequestParams["emp.BatPhone.Number"] = "800-DRK-KNGT";

				var emp = binder.BindModel();

				emp.ShouldNotBeNull();
				emp.BatPhone.ShouldBeNull();
			}

			[Fact]
			public void DeserializeSimpleObject()
			{
				var binder = new TestableValidationModelBinder<Customer> { Prefix = "cust" };
				binder.RequestParams["cust.Id"] = "10";

				var cust = binder.BindModel();

				cust.ShouldNotBeNull();
				cust.Id.ShouldEqual(10);
			}

			[Fact]
			public void DeserializeSimpleArray()
			{
				var binder = new TestableValidationModelBinder<ArrayClass> { Prefix = "array" };
				binder.RequestParams["array.Ids[0]"] = "10";
				binder.RequestParams["array.Ids[1]"] = "20";

				var array = binder.BindModel();

				Assert.NotNull(array);
				Assert.Equal(2, array.Ids.Length);
				Assert.Equal(10, array.Ids[0]);
				Assert.Equal(20, array.Ids[1]);
			}

			class ArrayClass
			{
				public string Name { get; set; }

				public int[] Ids { get; set; }
			}

			[Fact]
			public void DeserializeSimpleArrayFromMultiple()
			{
				var binder = new TestableValidationModelBinder<ArrayClass> { Prefix = "array" };

				binder.RequestParams.Add("array.ids", "10");
				binder.RequestParams.Add("array.ids", "20");

				var array = binder.BindModel();

				Assert.NotNull(array);
				Assert.Equal(2, array.Ids.Length);
				Assert.Equal(10, array.Ids[0]);
				Assert.Equal(20, array.Ids[1]);
			}

			[Fact]
			public void DeserializePrimitiveArray()
			{
				var binder = new TestableValidationModelBinder<int[]> { Prefix = "ids" };
				binder.RequestParams["ids[0]"] = "10";
				binder.RequestParams["ids[1]"] = "20";

				var array = binder.BindModel();

				Assert.NotNull(array);
				Assert.Equal(2, array.Length);
				Assert.Equal(10, array[0]);
				Assert.Equal(20, array[1]);
			}

			[Fact]
			public void DeserializePrimitiveArrayFromMultiple()
			{
				var binder = new TestableValidationModelBinder<int[]> { Prefix = "ids" };

				binder.RequestParams.Add("ids", "10");
				binder.RequestParams.Add("ids", "20");

				var array = binder.BindModel();

				Assert.NotNull(array);
				Assert.Equal(2, array.Length);
				Assert.Equal(10, array[0]);
				Assert.Equal(20, array[1]);
			}

			[Fact]
			public void DeserializePrimitiveGenericList()
			{
				var binder = new TestableValidationModelBinder<List<int>> { Prefix = "ids" };

				binder.RequestParams["ids[0]"] = "10";
				binder.RequestParams["ids[1]"] = "20";

				var list = binder.BindModel();

				Assert.NotNull(list);
				Assert.Equal(2, list.Count);
				Assert.Equal(10, list[0]);
				Assert.Equal(20, list[1]);
			}

			[Fact]
			public void DeserializePrimitiveGenericListInterface()
			{
				var binder = new TestableValidationModelBinder<IList<int>> { Prefix = "ids" };

				binder.RequestParams["ids[0]"] = "10";
				binder.RequestParams["ids[1]"] = "20";

				var list = binder.BindModel();

				Assert.NotNull(list);
				Assert.Equal(2, list.Count);
				Assert.Equal(10, list[0]);
				Assert.Equal(20, list[1]);
			}

			[Fact]
			public void DeserializePrimitiveGenericEnumerableInterface()
			{
				var binder = new TestableValidationModelBinder<IEnumerable<int>> { Prefix = "ids" };

				binder.RequestParams["ids[0]"] = "10";
				binder.RequestParams["ids[1]"] = "20";

				var list = binder.BindModel();

				Assert.NotNull(list);
				Assert.Equal(2, list.Count());
				Assert.Equal(10, list.ToArray()[0]);
				Assert.Equal(20, list.ToArray()[1]);
			}

			[Fact]
			public void DeserializePrimitiveGenericCollectionInterface()
			{
				var binder = new TestableValidationModelBinder<ICollection<int>> { Prefix = "ids" };

				binder.RequestParams["ids[0]"] = "10";
				binder.RequestParams["ids[1]"] = "20";

				var list = binder.BindModel();

				Assert.NotNull(list);
				Assert.Equal(2, list.Count);
				Assert.Equal(10, list.ToArray()[0]);
				Assert.Equal(20, list.ToArray()[1]);
			}

			[Fact]
			public void DeserializePrimitiveGenericListFromMultiple()
			{
				var binder = new TestableValidationModelBinder<List<int>> { Prefix = "ids" };

				binder.RequestParams.Add("ids", "10");
				binder.RequestParams.Add("ids", "20");

				var list = binder.BindModel();

				Assert.NotNull(list);
				Assert.Equal(2, list.Count);
				Assert.Equal(10, list[0]);
				Assert.Equal(20, list[1]);
			}

			enum TestEnum
			{
				One,
				Two,
				Three
			}

			[Fact]
			public void DeserializeEnumGenericListFromMultiple()
			{
				var binder = new TestableValidationModelBinder<List<TestEnum>> { Prefix = "testEnum" };

				binder.RequestParams.Add("testEnum", "0");
				binder.RequestParams.Add("testEnum", "2");

				var list = binder.BindModel();

				Assert.NotNull(list);
				Assert.Equal(2, list.Count);
				Assert.Equal(TestEnum.One, list[0]);
				Assert.Equal(TestEnum.Three, list[1]);
			}

			[Fact]
			public void DeserializeSimpleGenericList()
			{
				var binder = new TestableValidationModelBinder<GenericListClass> { Prefix = "list" };

				binder.RequestParams["list.Ids[0]"] = "10";
				binder.RequestParams["list.Ids[1]"] = "20";

				var list = binder.BindModel();

				Assert.NotNull(list);
				Assert.Equal(2, list.Ids.Count);
				Assert.Equal(10, list.Ids[0]);
				Assert.Equal(20, list.Ids[1]);
			}

			[Fact]
			public void DeserializeSimpleGenericListFromMultiple()
			{
				var binder = new TestableValidationModelBinder<GenericListClass> { Prefix = "list" };

				binder.RequestParams.Add("list.Ids", "10");
				binder.RequestParams.Add("list.Ids", "20");

				var list = binder.BindModel();

				Assert.NotNull(list);
				Assert.Equal(2, list.Ids.Count);
				Assert.Equal(10, list.Ids[0]);
				Assert.Equal(20, list.Ids[1]);
			}

			[Fact]
			public void DeserializeComplexGenericList()
			{
				var binder = new TestableValidationModelBinder<Employee> { Prefix = "emp" };

				binder.RequestParams["emp.OtherPhones[0].Number"] = "800-555-1212";
				binder.RequestParams["emp.OtherPhones[1].Number"] = "800-867-5309";
				binder.RequestParams["emp.OtherPhones[1].AreaCodes[0]"] = "800";
				binder.RequestParams["emp.OtherPhones[1].AreaCodes[1]"] = "877";

				var emp = binder.BindModel();

				Assert.NotNull(emp);
				Assert.Equal(2, emp.OtherPhones.Count);
				Assert.Equal("800-555-1212", emp.OtherPhones[0].Number);
				Assert.Equal("800-867-5309", emp.OtherPhones[1].Number);
				Assert.Equal(2, emp.OtherPhones[1].AreaCodes.Count);
				Assert.Equal("800", emp.OtherPhones[1].AreaCodes[0]);
				Assert.Equal("877", emp.OtherPhones[1].AreaCodes[1]);
			}

			[Fact]
			public void WithoutAnyPostedArrayDataAnArrayPropertyIsInitializedNull()
			{
				var binder = new TestableValidationModelBinder<ArrayClass> { Prefix = "array" };

				binder.RequestParams["array.Name"] = "test";

				var array = binder.BindModel();

				Assert.NotNull(array);
				Assert.Null(array.Ids);
			}

			[Fact]
			public void DeserializeComplexObject()
			{
				var binder = new TestableValidationModelBinder<Employee> { Prefix = "emp" };

				binder.RequestParams["emp.Id"] = "20";
				binder.RequestParams["emp.Phone.Number"] = "800-555-1212";

				var emp = binder.BindModel();

				Assert.NotNull(emp);
				Assert.Equal(20, emp.Id);
				Assert.Equal("800-555-1212", emp.Phone.Number);
			}

			[Fact]
			public void EmptyValuesUseDefaultOfType()
			{
				var binder = new TestableValidationModelBinder<Customer> { Prefix = "cust" };

				binder.RequestParams["cust.Id"] = "";

				var cust = binder.BindModel();

				Assert.NotNull(cust);
				Assert.Equal(0, cust.Id);
			}

			class BoolClass
			{
				public bool MyBool { get; set; }
			}

			[Fact]
			public void DeserializeTrueBool()
			{
				var binder = new TestableValidationModelBinder<BoolClass> { Prefix = "bool" };

				binder.RequestParams["bool.myBool"] = "true";

				var boolClass = binder.BindModel();

				Assert.NotNull(boolClass);
				Assert.True(boolClass.MyBool);
			}

			[Fact]
			public void DeserializeTrueFalseBool()
			{
				var binder = new TestableValidationModelBinder<BoolClass> { Prefix = "bool" };

				binder.RequestParams["bool.myBool"] = "true,false";

				BoolClass boolClass;
				using (new CurrentCultureScope(""))
				{
					// We use invariant culture in case a language specific resource with key 'PropertyValueInvalid' has been added.
					boolClass = binder.BindModel();
				}

				Assert.NotNull(boolClass);
				Assert.False(boolClass.MyBool); // defaults to false because of format exception
				Assert.False(binder.IsInputValid);
				Assert.False(binder.IsInputValidFor("bool.myBool"));
				Assert.Equal(1, binder.GetModelErrorsFor("bool.myBool").Count);
				Assert.Equal("The value 'true,false' is not valid for MyBool.", binder.GetModelErrorsFor("bool.myBool")[0].ErrorMessage);
			}

			[Fact]
			public void DeserializeFalseBool()
			{
				var binder = new TestableValidationModelBinder<BoolClass> { Prefix = "bool" };

				binder.RequestParams["bool.myBool"] = "false";

				var boolClass = binder.BindModel();

				Assert.False(boolClass.MyBool);
			}

			[Fact]
			public void NoRequestForPropertyShouldNotInstantiateProperty()
			{
				var binder = new TestableValidationModelBinder<Employee> { Prefix = "emp" };

				binder.RequestParams["emp.Id"] = "20";
				binder.RequestParams["emp.Phone.Number"] = "800-555-1212";

				var emp = binder.BindModel();

				Assert.NotNull(emp);
				Assert.NotNull(emp.Phone);
				Assert.Null(emp.BatPhone);
			}

			[UsedImplicitly]
			class GenericListClass
			{
				public string Name { get; set; }

				public IList<int> Ids { get; set; }

				private const IList<int> _readonlyIds = null;

				public IList<int> ReadonlyIds
				{
					get { return _readonlyIds; }
				}
			}
		}
	}
}