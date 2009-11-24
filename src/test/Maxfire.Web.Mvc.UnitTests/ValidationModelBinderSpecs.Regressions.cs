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
		[UsedImplicitly]
		public class Regressions
		{
			class FooImmutableList
			{
				public FooImmutableList()
				{
					Beregnere = new Foo[] { };
				}

				public long Id { get; [UsedImplicitly] set; }
				public Foo[] Beregnere { get; [UsedImplicitly] set; }
			}

			class FooList
			{
				public FooList()
				{
					Beregnere = new List<Foo>();
				}

				public long Id { get; [UsedImplicitly] set; }
				public IList<Foo> Beregnere { get; private set; }
			}

			[UsedImplicitly]
			class Foo
			{
				public string DomusGebyrType { get; [UsedImplicitly] set; }
				public string Beloeb { get; [UsedImplicitly] set; }
				public string Sats { get; [UsedImplicitly] set; }
				public string SatsGrundlag { get; [UsedImplicitly] set; }
				public bool Apply { get; [UsedImplicitly] set; }
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
				NameValueDeserializer nvd = new NameValueDeserializer();

				var priser = nvd.Deserialize(_requestParams, "Input", typeof(FooImmutableList)) as FooImmutableList;

				verify(priser);
			}

			[Fact]
			public void NameValueDeserializerHandlesNonImmutableCollections()
			{
				NameValueDeserializer nvd = new NameValueDeserializer();

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

			[UsedImplicitly]
			class DictionaryBar
			{
				private readonly IDictionary<int, Bar> _bars = new Dictionary<int, Bar>();

				public IDictionary<int, Bar> Bars
				{
					get { return _bars; }
				}
			}

			[UsedImplicitly]
			class ListBar
			{
				private readonly IList<Bar> _bars = new List<Bar>();

				public IList<Bar> Bars
				{
					get { return _bars; }
				}
			}

			[UsedImplicitly]
			class Bar
			{
				public int Age { get; [UsedImplicitly] set; }
			}

			[Fact]
			public void CanHandleDictionary()
			{
				var requestParams = new NameValueCollection { { "Bars[4].Age", "39" } };

				var binder = new TestableValidationModelBinder<DictionaryBar>
				             	{
				             		RequestParams = requestParams
				             	};

				binder.BindModel().Bars[4].Age.ShouldEqual(39);
			}

			[Fact]
			public void CanHandleList()
			{
				var requestParams = new NameValueCollection { { "Bars[Morten].Age", "39" } };

				var binder = new TestableValidationModelBinder<ListBar>
				             	{
				             		RequestParams = requestParams
				             	};

				binder.BindModel().Bars[0].Age.ShouldEqual(39);
			}

			[UsedImplicitly]
			class InputModel
			{
				private readonly IDictionary<Kategori, IList<SubInputModel>> _models = new Dictionary<Kategori, IList<SubInputModel>>();

				public IDictionary<Kategori, IList<SubInputModel>> Models
				{
					get { return _models; }
				}
			}

			[UsedImplicitly]
			class SubInputModel
			{
				public string Tekst { get; [UsedImplicitly] set; }
			}

			[Fact]
			public void CanHandleHashedInputModel()
			{
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

				model.Models[Kategori.Transport][0].Tekst.ShouldEqual("TransportTekst1");
				model.Models[Kategori.Transport][1].Tekst.ShouldEqual("TransportTekst2");
				model.Models[Kategori.Transport][2].Tekst.ShouldEqual("TransportTekst3");
				model.Models[Kategori.Bolig][0].Tekst.ShouldEqual("BoligTekst1");
				model.Models[Kategori.Bolig][1].Tekst.ShouldEqual("BoligTekst2");
				model.Models[Kategori.Bolig][2].Tekst.ShouldEqual("BoligTekst3");
			}

			[Fact]
			public void CanHandleHashedInputModelWhenUsingEnumerationValues()
			{
				var requestParams = new NameValueCollection { { "Models[1][0].Tekst", "Tekst" } };

				var binder = new TestableValidationModelBinder<InputModel>
				             	{
				             		RequestParams = requestParams
				             	};

				binder.BindModel().Models[Kategori.Transport][0].Tekst.ShouldEqual("Tekst");
			}

			[UsedImplicitly]
			class ComplexInputModel
			{
				private readonly IDictionary<Kategori, IList<SubInputModel>> _models = new Dictionary<Kategori, IList<SubInputModel>>();

				public string Name { get; [UsedImplicitly] set; }

				public IDictionary<Kategori, IList<SubInputModel>> Models
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
				input.Models[Kategori.Transport].Count.ShouldEqual(3);
				input.Models[Kategori.Transport][0].Tekst.ShouldEqual("TransportTekst1");
				input.Models[Kategori.Transport][1].Tekst.ShouldEqual("TransportTekst2");
				input.Models[Kategori.Transport][2].Tekst.ShouldEqual("TransportTekst3");
				input.Models[Kategori.Bolig].Count.ShouldEqual(2);
				input.Models[Kategori.Bolig][0].Tekst.ShouldEqual("BoligTekst1");
				input.Models[Kategori.Bolig][1].Tekst.ShouldEqual("BoligTekst2");
			}
		}
	}
}