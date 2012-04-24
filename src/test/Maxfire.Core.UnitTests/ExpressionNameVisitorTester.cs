using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Maxfire.Core.Reflection;
using Maxfire.TestCommons.AssertExtensions;
using Xunit;
using ExpressionHelper = Maxfire.Core.Reflection.ExpressionHelper;
//using MvcExpressionHelper = System.Web.Mvc.ExpressionHelper;

namespace Maxfire.Core.UnitTests
{
	// TODO: Better naming of tests
	public class ExpressionNameVisitorTester
	{
		class PersonModel
		{
			public DateTime BirthDay { get; set; }
			public DateTime? DeathDay { get; set; }
		}

		[Fact]
		public void can_handle_nullable_datetime()
		{
			ExpressionExtensions.GetNameFor<PersonModel, int>(x => x.BirthDay.Day).ShouldEqual("BirthDay.Day");
			ExpressionExtensions.GetNameFor<PersonModel, int>(x => x.DeathDay.Value.Day).ShouldEqual("DeathDay.Day");
		}

		enum Kategori { Transport }

		class Foo
		{
			private readonly IDictionary<string, Bar> _bars = new Dictionary<string, Bar>();

			public IDictionary<string, Bar> Bars
			{
				get { return _bars; }
			}
		}

		class Bar
		{
			public string Name { get; [UsedImplicitly] private set; }
		}

		[Fact]
		public void CanHandleDictionary()
		{
			ExpressionExtensions.GetNameFor<Foo, object>(x => x.Bars["Morten"].Name).ShouldEqual("Bars[Morten].Name");
		}

		class InputModel
		{
			private IDictionary<Kategori, IList<SubInputModel>> _models = new Dictionary<Kategori, IList<SubInputModel>>();

			public IDictionary<Kategori, IList<SubInputModel>> Models
			{
				get { return _models; }
			}
		}

		class SubInputModel
		{
			public string Tekst { get; [UsedImplicitly] set; }
		}

		[Fact]
		public void CanHandleHashedInputModel()
		{
			ExpressionExtensions.GetNameFor<InputModel, object>(x => x.Models[Kategori.Transport][0].Tekst)
				.ShouldEqual("Models[Transport][0].Tekst");
		}

		[Fact]
		public void CanHandleHashedInputModelWhenCapturingIndexes()
		{
			var i = 0;
			var kategori = Kategori.Transport;
			ExpressionExtensions.GetNameFor<InputModel, object>(x => x.Models[kategori][i].Tekst)
				.ShouldEqual("Models[Transport][0].Tekst");
		}

		[UsedImplicitly]
		class FakeChildModel
		{
			public string FirstName { get; [UsedImplicitly] set; }
			public decimal Balance { get; [UsedImplicitly] set; }
		}

		[UsedImplicitly]
		class FakeModel
		{
			private readonly IList<FakeChildModel> _customers = new List<FakeChildModel>();
			private readonly IList<FakeModel> _fakeModelList = new List<FakeModel>();
			private readonly IDictionary<string, FakeChildModel> _fakeModelDictionary = new Dictionary<string, FakeChildModel>();
			private readonly IDictionary<string, IList<FakeChildModel>> _customersByCity = new Dictionary<string, IList<FakeChildModel>>();

			public FakeChildModel Person { get; [UsedImplicitly] set; }

			public string Title { get; [UsedImplicitly] set; }

			public decimal? Price { get; [UsedImplicitly] set; }

			public IList<int> Numbers { get; [UsedImplicitly] set; }

			public IList<FakeChildModel> Customers
			{
				get { return _customers; }
			}

			public IList<FakeModel> FakeModelList
			{
				get { return _fakeModelList; }
			}

			public IDictionary<string, FakeChildModel> FakeModelDictionary
			{
				get { return _fakeModelDictionary; }
			}

			public IDictionary<string, IList<FakeChildModel>> CustomersByCity
			{
				get { return _customersByCity; }
			}

			public FakeModel[] FakeModelArray { get; [UsedImplicitly] set; }
		}

		[Fact]
		public void can_get_name_for_simple_string_property()
		{
			Expression<Func<FakeModel, object>> expression = x => x.Title;
			string name = expression.GetNameFor();
			name.ShouldEqual("Title");
		}

		[Fact]
		public void can_get_name_for_simple_decimal_property()
		{
			Expression<Func<FakeModel, object>> expression = x => x.Price;
			string name = expression.GetNameFor();
			name.ShouldEqual("Price");
		}

		[Fact]
		public void can_get_name_for_compound_string_property()
		{
			Expression<Func<FakeModel, object>> expression = x => x.Person.FirstName;
			string name = expression.GetNameFor();
			name.ShouldEqual("Person.FirstName");
		}

		[Fact]
		public void can_get_name_for_instance_of_simple_collection_property()
		{
			Expression<Func<FakeModel, object>> expression = x => x.Numbers[0];
			string name = expression.GetNameFor();
			name.ShouldEqual("Numbers[0]");
		}

		[Fact]
		public void can_get_name_for_instance_of_simple_collection_property_using_a_captured_local_variable_as_index()
		{
			int i = 33;
			Expression<Func<FakeModel, object>> expression = x => x.Numbers[i];
			string name = expression.GetNameFor();
			name.ShouldEqual("Numbers[33]");
		}

		[Fact]
		public void can_get_name_for_instance_of_complex_collection_property()
		{
			Expression<Func<FakeModel, object>> expression = x => x.Customers[0].FirstName;
			string name = expression.GetNameFor();
			name.ShouldEqual("Customers[0].FirstName");
		}

		[Fact]
		public void can_get_name_for_instance_of_complex_dictionary_property()
		{
			Expression<Func<FakeModel, object>> expression = x => x.FakeModelDictionary["Morten"].FirstName;
			string name = expression.GetNameFor();
			name.ShouldEqual("FakeModelDictionary[Morten].FirstName");
		}

		[Fact]
		public void can_get_name_for_instance_of_complex_dictionary_with_nested_collection_property()
		{
			Expression<Func<FakeModel, object>> expression = x => x.CustomersByCity["Copenhagen"][999].FirstName;
			string name = expression.GetNameFor();
			name.ShouldEqual("CustomersByCity[Copenhagen][999].FirstName");
		}

		[Fact]
		public void can_get_name_for_instance_of_complex_dictionary_with_nested_collection_property_also_when_using_captured_arguments()
		{
			var i = 999;
			var city = "Copenhagen";
			Expression<Func<FakeModel, object>> expression = x => x.CustomersByCity[city][i].FirstName;
			string name = expression.GetNameFor();
			name.ShouldEqual("CustomersByCity[Copenhagen][999].FirstName");
		}

		[Fact]
		public void can_get_name_for_instance_of_nested_complex_collection_property()
		{
			int i = 0;
			Expression<Func<FakeModel, object>> expression = x => x.FakeModelList[1].Customers[i].Balance;
			string name = expression.GetNameFor();
			name.ShouldEqual("FakeModelList[1].Customers[0].Balance");
		}

		[Fact]
		public void can_get_name_for_property_of_instance_of_collection()
		{
			Expression<Func<IList<FakeModel>, object>> expression = x => x[999].Title;
			string name = expression.GetNameFor();
			name.ShouldEqual("[999].Title");
		}

		[Fact]
		public void can_get_name_for_property_of_instance_of_array()
		{
			int i = 888;
			Expression<Func<FakeModel, object>> expression = x => x.FakeModelArray[999].FakeModelArray[i].Person.FirstName;
			string name = expression.GetNameFor();
			name.ShouldEqual("FakeModelArray[999].FakeModelArray[888].Person.FirstName");
		}

		[Fact]
		public void Test1()
		{
			// "Model" at the front of the expression is excluded (case insensitively)
			FooModel Model = null;
			Assert.Equal(string.Empty, ExpressionHelper.GetExpressionText(Lambda<object, FooModel>(m => Model)));
			Assert.Equal("StringProperty", ExpressionHelper.GetExpressionText(Lambda<object, string>(m => Model.StringProperty)));

			FooModel mOdeL = null;
			Assert.Equal(String.Empty, ExpressionHelper.GetExpressionText(Lambda<object, FooModel>(m => mOdeL)));
			Assert.Equal("StringProperty", ExpressionHelper.GetExpressionText(Lambda<object, string>(m => mOdeL.StringProperty)));
		}

		[Fact]
		public void Test1b()
		{
			// Model property of model is passed through
			Assert.Equal("Model", ExpressionHelper.GetExpressionText(Lambda<DummyModelContainer, FooModel>(m => m.Model)));
		}

		[Fact]
		public void Test1c()
		{
			// "Model" in the middle of the expression is not excluded
			DummyModelContainer container = null;
			Assert.Equal("container.Model", ExpressionHelper.GetExpressionText(Lambda<object, FooModel>(m => container.Model)));
			Assert.Equal("container.Model.StringProperty", ExpressionHelper.GetExpressionText(Lambda<object, string>(m => container.Model.StringProperty)));

		}

		[Fact]
		public void Test()
		{
			// The parameter is excluded
			Assert.Equal(string.Empty, ExpressionHelper.GetExpressionText(Lambda<FooModel, FooModel>(m => m)));
		}

		[Fact]
		public void Test2()
		{
			// The parameter is excluded
			Assert.Equal("StringProperty", ExpressionHelper.GetExpressionText(Lambda<FooModel, string>(m => m.StringProperty)));
		}

		[Fact]
		public void Test3()
		{
			// Integer indexer is included and properly computed from captured values
			int x = 2;
			Assert.Equal("[42]", ExpressionHelper.GetExpressionText(Lambda<int[], int>(m => m[x * 21])));
		}

		[Fact]
		public void Test3b()
		{
			// Integer indexer is included and properly computed from captured values
			const int x = 2;
			Assert.Equal("[42]", ExpressionHelper.GetExpressionText(Lambda<int[], int>(m => m[x * 21])));
		}

		[Fact]
		public void Test4()
		{
			// Integer indexer is included and properly computed from captured values
			DummyModelContainer container = null;
			int x = 2;
			Assert.Equal("container.Model[42].Length", ExpressionHelper.GetExpressionText(Lambda<object, int>(m => container.Model[x * 21].Length)));
		}

		[Fact]
		public void Test4b()
		{
			DummyModelContainer container = null;

			// Integer indexer is included and properly computed from captured values
			int x = 2;
			Assert.Equal("container.Model[42].Length", ExpressionHelper.GetExpressionText(Lambda<object, int>(m => container.Model[x * 21].Length)));
		}

		[Fact]
		public void Test5()
		{
			DummyModelContainer container = null;

			// string indexer is included and properly computed from captured values
			string y = "Hello World";
			Assert.Equal(@"container.Model[Hello].Length", ExpressionHelper.GetExpressionText(Lambda<object, int>(m => container.Model[y.Substring(0, 5)].Length)));
		}

		[Fact]
		public void Test6()
		{
			DummyModelContainer container = null;
			int x = 2;
			
			// Back to back indexer is included
			Assert.Equal("container.Model[1024][2]", ExpressionHelper.GetExpressionText(Lambda<object, char>(m => container.Model[x * 512][x])));
		}

		[Fact]
		public void Test7()
		{
			DummyModelContainer container = null;
			
			// Multi-parameter indexer is supported
			Assert.Equal(@"container.Model[42, Hello World].Length", ExpressionHelper.GetExpressionText(Lambda<object, int>(m => container.Model[42, "Hello World"].Length)));
		}

		[Fact]
		public void Test8()
		{
			DummyModelContainer container = null;
			int x = 2;
			
			// Single array indexer is included
			Assert.Equal("container.Model.Array[1024]", ExpressionHelper.GetExpressionText(Lambda<object, int>(m => container.Model.Array[x * 512])));
		}

		[Fact]
		public void Test9()
		{
			DummyModelContainer container = null;

			// Double array indexer is supported
			Assert.Equal("container.Model.DoubleArray[1, 2]", ExpressionHelper.GetExpressionText(Lambda<object, int>(m => container.Model.DoubleArray[1, 2])));
		}

		[Fact]
		public void Test10()
		{
			DummyModelContainer container = null;

			// Non-indexer method call is supported
			Assert.Equal("container.Model.SomeMethod().Length", ExpressionHelper.GetExpressionText(Lambda<object, int>(m => container.Model.SomeMethod().Length)));
		}

		[Fact]
		public void Test11()
		{
			// Lambda expression which involves indexer which references lambda parameter throws
			Assert.Throws<InvalidOperationException>(() => ExpressionHelper.GetExpressionText(Lambda<string, char>(s => s[s.Length - 4])));
		}

		private static LambdaExpression Lambda<T1, T2>(Expression<Func<T1, T2>> expression)
		{
			return expression;
		}

		class FooModel
		{
			public string StringProperty { get; set; }

			public string this[int index] { get { return index.ToString(); } }

			public string this[string index] { get { return index; } }

			public string this[int index, string index2] { get { return index + index2; } }

			public int[] Array { get; set; }

			public int[,] DoubleArray { get; set; }

			public string SomeMethod() { return ToString(); }
		}

		class DummyModelContainer
		{
			public FooModel Model { get; set; }
		}
	}
}