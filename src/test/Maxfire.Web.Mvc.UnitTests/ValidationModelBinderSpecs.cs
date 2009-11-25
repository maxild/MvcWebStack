using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Maxfire.Core;
using Maxfire.TestCommons;
using Maxfire.Web.Mvc.TestCommons.AssertExtensions;
using Maxfire.Web.Mvc.Validators;
using Specification = Xunit.FactAttribute;
using Context = System.Diagnostics.CodeAnalysis.UsedImplicitlyAttribute;

namespace Maxfire.Web.Mvc.UnitTests
{
	public partial class ValidationModelBinderSpecs
	{
		[TypeConverter(typeof(EnumerationConverter<FooKategori>))]
		class FooKategori : Enumeration
		{
			public static readonly FooKategori Transport = new FooKategori(1, "Transport");
			public static readonly FooKategori Bolig = new FooKategori(2, "Bolig");
			
			private FooKategori(int id, string name)
				: base(id, name, name)
			{
			}
		}

		class SubInputModel
		{
			[LabeledValidateNonEmpty(ErrorMessage = "Feltet {0} er et krav.")]
			public string ChildProperty { get; set; }
		}

		class DictionaryOfListInputModel
		{
			private readonly IDictionary<FooKategori, IList<SubInputModel>> _childs = new Dictionary<FooKategori, IList<SubInputModel>>();

			[LabeledValidateNonEmpty(ErrorMessage = "Feltet {0} er et krav.")]
			public string ParentProperty { get; set; }

			public IDictionary<FooKategori, IList<SubInputModel>> Childs
			{
				get { return _childs; }
			}
		}

		[Context]
		class WhenValidatingDictionaryOfListInputModel : XunitSpec<TestableValidationModelBinder<DictionaryOfListInputModel>>
		{
			protected override TestableValidationModelBinder<DictionaryOfListInputModel> Establish_context()
			{
				var invalidRequestParams = new NameValueCollection
				                           	{
				                           		{ "ParentProperty", "" },
				                           		{ "Childs[Transport][0].ChildProperty", "" }
				                           	};

				var binder = new TestableValidationModelBinder<DictionaryOfListInputModel>
				             	{
				             		RequestParams = invalidRequestParams
				             	};

				return binder;
			}

			protected override void Because_of()
			{
				// Validate
				Sut.BindModel();
			}

			[Specification]
			public void ShouldContainBothErrorsButNoMore()
			{
				Sut.ModelState.ShouldContainNumberOfInvalidFields(2);
			}

			[Specification]
			public void ShouldInvalidateRootNameProperty()
			{
				Sut.ModelState.ShouldContain<DictionaryOfListInputModel>(x => x.ParentProperty)
					.WithAttemptedValue("")
					.AndErrorMessage("Feltet ParentProperty er et krav.");
			}

			[Specification]
			public void ShouldInvalidateChildTekstProperty()
			{
				Sut.ModelState.ShouldContain<DictionaryOfListInputModel>(x => x.Childs[FooKategori.Transport][0].ChildProperty)
					.WithAttemptedValue("")
					.AndErrorMessage("Feltet ChildProperty er et krav.");
			}
		}
	}
}