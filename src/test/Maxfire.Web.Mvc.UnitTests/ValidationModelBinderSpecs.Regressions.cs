using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using Maxfire.TestCommons;
using Maxfire.TestCommons.AssertExtensions;
using MvcContrib;
using Xunit;

namespace Maxfire.Web.Mvc.UnitTests
{
	[UsedImplicitly]
	public partial class ValidationModelBinderSpecs
	{
		public class Regressions
		{
			class FooImmutableList
			{
				public FooImmutableList()
				{
					Beregnere = new Foo[] { };
				}

				public long Id { get; set; }
				public Foo[] Beregnere { get; set; }
			}

			class FooList
			{
				public FooList()
				{
					Beregnere = new List<Foo>();
				}

				public long Id { get; set; }
				public IList<Foo> Beregnere { get; private set; }
			}

			class Foo
			{
				public string DomusGebyrType { get; set; }
				public string Beloeb { get; set; }
				public string Sats { get; set; }
				public string SatsGrundlag { get; set; }
				public bool Apply { get; set; }
			}

			private readonly NameValueCollection _requestParams;

			public Regressions()
			{
				_requestParams = new NameValueCollection
				                 	{
				                 		{ "Input.Id", "12" },
				                 		{ "Input.Beregnere[0].DomusGebyrType", "003" },
				                 		{ "Input.Beregnere[0].Beloeb", "1000" },
				                 		{ "Input.Beregnere[0].Sats", "0" },
				                 		{ "Input.Beregnere[0].SatsGrundlag", "Nul" },
				                 		{ "Input.Beregnere[0].Apply", "true" },
				                 	};
			}

			// The DefaultModelBinder can't modify immutable collections, and arrays are immutable 
			// in the sense that their lengths cannot be changed.
			[Fact]
			public void DefaultModelBinderThrowsOnImmutableCollection()
			{
				var binder = new TestableValidationModelBinder<FooImmutableList>
				             	{
				             		RequestParams = _requestParams,
				             		Prefix = "Input"
				             	};

				using (new CurrentCultureScope(""))
				{
					var ex = Assert2.ThrowsInnerException<NotSupportedException>(() => binder.BindModel());
					ex.Message.ShouldEqual("Collection is read-only.");
				}
			}

			[Fact]
			public void DefaultModelBinderHandlesNonImmutableCollections()
			{
				var binder = new TestableValidationModelBinder<FooList>
				             	{
				             		RequestParams = _requestParams,
				             		Prefix = "Input"
				             	};

				var priser = binder.BindModel();

				verify(priser);
			}

			[Fact]
			public void NameValueDeserializerHandlesImmutableCollection()
			{
				var nvd = new NameValueDeserializer();

				var priser = nvd.Deserialize(_requestParams, "Input", typeof(FooImmutableList)) as FooImmutableList;

				verify(priser);
			}

			[Fact]
			public void NameValueDeserializerHandlesNonImmutableCollections()
			{
				var nvd = new NameValueDeserializer();

				var priser = nvd.Deserialize(_requestParams, "Input", typeof(FooList)) as FooList;

				verify(priser);
			}

			private static void verify(FooList priser)
			{
				priser.ShouldNotBeNull();
				priser.Id.ShouldEqual(12);
				priser.Beregnere.ShouldNotBeNull();
				priser.Beregnere.Count.ShouldEqual(1);
				priser.Beregnere[0].DomusGebyrType.ShouldEqual("003");
				priser.Beregnere[0].Beloeb.ShouldEqual("1000");
				priser.Beregnere[0].Sats.ShouldEqual("0");
				priser.Beregnere[0].SatsGrundlag.ShouldEqual("Nul");
				priser.Beregnere[0].Apply.ShouldBeTrue();
			}

			private static void verify(FooImmutableList priser)
			{
				priser.ShouldNotBeNull();
				priser.Id.ShouldEqual(12);
				priser.Beregnere.ShouldNotBeNull();
				priser.Beregnere.Length.ShouldEqual(1);
				priser.Beregnere[0].DomusGebyrType.ShouldEqual("003");
				priser.Beregnere[0].Beloeb.ShouldEqual("1000");
				priser.Beregnere[0].Sats.ShouldEqual("0");
				priser.Beregnere[0].SatsGrundlag.ShouldEqual("Nul");
				priser.Beregnere[0].Apply.ShouldBeTrue();
			}

			class DictionaryOfBars
			{
				private readonly IDictionary<int, Bar> _bars = new Dictionary<int, Bar>();

				public IDictionary<int, Bar> Bars
				{
					get { return _bars; }
				}
			}

			class ListOfBars
			{
				private readonly IList<Bar> _bars = new List<Bar>();

				public IList<Bar> Bars
				{
					get { return _bars; }
				}
			}

			class Bar
			{
				public int Age { get; set; }
			}

			[Fact]
			public void CanHandleDictionary()
			{
				// This is is testing the changes I have made to the DefaultModelBinder 
				// to support dictionaries with simple keys. 

				var requestParams = new NameValueCollection { { "Bars[4].Age", "39" } };

				var binder = new TestableValidationModelBinder<DictionaryOfBars>
				             	{
				             		RequestParams = requestParams
				             	};

				binder.BindModel().Bars[4].Age.ShouldEqual(39);
			}

			[Fact]
			public void CanHandleList()
			{
				// This is testing the changes I have made to DefaultModelBinder
				// to support lists with arbitrary indices (possibly with 'holes' 
				// in the indices)

				var requestParams = new NameValueCollection { { "Bars[Morten].Age", "39" } };

				var binder = new TestableValidationModelBinder<ListOfBars>
				             	{
				             		RequestParams = requestParams
				             	};

				binder.BindModel().Bars[0].Age.ShouldEqual(39);
			}

			class InputModel
			{
				private readonly IDictionary<FooKategori, IList<SubInputModel>> _models = new Dictionary<FooKategori, IList<SubInputModel>>();

				public IDictionary<FooKategori, IList<SubInputModel>> Models
				{
					get { return _models; }
				}
			}

			
			class SubInputModel
			{
				public string Tekst { get;  set; }
			}

			[Fact]
			public void CanHandleHashedInputModel()
			{
				// This is is testing the changes I have made to the DefaultModelBinder 
				// to support dictionaries with simple keys. 

				var requestParams = new NameValueCollection
				                    	{
				                    		{ "Models[Transport][0].Tekst", "TransportTekst1" },
				                    		{ "Models[Transport][1].Tekst", "TransportTekst2" },
				                    		{ "Models[Transport][2].Tekst", "TransportTekst3" },
				                    		{ "Models[Bolig][0].Tekst", "BoligTekst1" },
				                    		{ "Models[Bolig][1].Tekst", "BoligTekst2" },
				                    		{ "Models[Bolig][2].Tekst", "BoligTekst3" }
				                    	};

				var binder = new TestableValidationModelBinder<InputModel>
				             	{
				             		RequestParams = requestParams
				             	};

				var model = binder.BindModel();

				model.Models[FooKategori.Transport][0].Tekst.ShouldEqual("TransportTekst1");
				model.Models[FooKategori.Transport][1].Tekst.ShouldEqual("TransportTekst2");
				model.Models[FooKategori.Transport][2].Tekst.ShouldEqual("TransportTekst3");
				model.Models[FooKategori.Bolig][0].Tekst.ShouldEqual("BoligTekst1");
				model.Models[FooKategori.Bolig][1].Tekst.ShouldEqual("BoligTekst2");
				model.Models[FooKategori.Bolig][2].Tekst.ShouldEqual("BoligTekst3");
			}

			[Fact]
			public void CanHandleHashedInputModelWhenUsingEnumerationValues()
			{
				// This is is testing the changes I have made to the DefaultModelBinder 
				// to support dictionaries with simple keys. 

				var requestParams = new NameValueCollection { { "Models[1][0].Tekst", "Tekst" } };

				var binder = new TestableValidationModelBinder<InputModel>
				             	{
				             		RequestParams = requestParams
				             	};

				binder.BindModel().Models[FooKategori.Transport][0].Tekst.ShouldEqual("Tekst");
			}

			
			class ComplexInputModel
			{
				private readonly IDictionary<FooKategori, IList<SubInputModel>> _models = new Dictionary<FooKategori, IList<SubInputModel>>();

				public string Name { get;  set; }

				public IDictionary<FooKategori, IList<SubInputModel>> Models
				{
					get { return _models; }
				}
			}

			[Fact]
			public void CanHandleHashedInputModelWithNestedCollection()
			{
				var requestParams = new NameValueCollection
				                    	{
				                    		{ "Name", "Name1" },
				                    		{ "Models[Transport][0].Tekst", "TransportTekst1" },
				                    		{ "Models[Transport][4].Tekst", "TransportTekst2" },
				                    		{ "Models[Transport][5].Tekst", "TransportTekst3" },
				                    		{ "Models[Bolig][5].Tekst", "BoligTekst1" },
				                    		{ "Models[Bolig][9].Tekst", "BoligTekst2" }
				                    	};

				var binder = new TestableValidationModelBinder<ComplexInputModel>
				             	{
				             		RequestParams = requestParams
				             	};

				var input = binder.BindModel();

				input.Name.ShouldEqual("Name1");
				input.Models[FooKategori.Transport].Count.ShouldEqual(3);
				input.Models[FooKategori.Transport][0].Tekst.ShouldEqual("TransportTekst1");
				input.Models[FooKategori.Transport][1].Tekst.ShouldEqual("TransportTekst2");
				input.Models[FooKategori.Transport][2].Tekst.ShouldEqual("TransportTekst3");
				input.Models[FooKategori.Bolig].Count.ShouldEqual(2);
				input.Models[FooKategori.Bolig][0].Tekst.ShouldEqual("BoligTekst1");
				input.Models[FooKategori.Bolig][1].Tekst.ShouldEqual("BoligTekst2");
			}
		}
	}
}