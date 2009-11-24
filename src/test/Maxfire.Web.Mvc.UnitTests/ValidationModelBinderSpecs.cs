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
		[TypeConverter(typeof(EnumerationConverter<Kategori>))]
		public class Kategori : Enumeration
		{
			public static readonly Kategori Transport = new Kategori(1, "Transport", "Transport");
			public static readonly Kategori Bolig = new Kategori(2, "Bolig", "Bolig");
			
			private Kategori(int id, string name, string text)
				: base(id, name, text)
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
			private readonly IDictionary<Kategori, IList<SubInputModel>> _childs = new Dictionary<Kategori, IList<SubInputModel>>();

			[LabeledValidateNonEmpty(ErrorMessage = "Feltet {0} er et krav.")]
			public string ParentProperty { get; set; }

			public IDictionary<Kategori, IList<SubInputModel>> Childs
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
				// Note: This test fails because we cannot control which MetadataProvider is used. We want to use
				// our TestableModelMetadata, because it will crate the CastleModelValidator, but MVC somehow uses
				// ModelMetadataProviders.Current (a global variable yack!!!!!). Find out where and remedy!!!!
				Sut.ModelState.ShouldContain<DictionaryOfListInputModel>(x => x.ParentProperty)
					.WithAttemptedValue("")
					.AndErrorMessage("Feltet ParentProperty er et krav.");
			}

			[Specification]
			public void ShouldInvalidateChildTekstProperty()
			{
				Sut.ModelState.ShouldContain<DictionaryOfListInputModel>(x => x.Childs[Kategori.Transport][0].ChildProperty)
					.WithAttemptedValue("")
					.AndErrorMessage("Feltet ChildProperty er et krav.");
			}
		}
	}
}