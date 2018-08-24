using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Web.Mvc;
using Maxfire.TestCommons.AssertExtensions;
using Maxfire.Web.Mvc.ValueProviders;
using Xunit;

namespace Maxfire.Web.Mvc.UnitTests
{
	public class BetterDefaultModelBinderTests
	{
		[Fact]
		public void UpdateCollection_AnyIndexArrayIsSavedInModelState()
		{
			var formParams = new NameValueCollection
				{
					{"foo[0]", "10"},
					{"foo[4]", "20"},
					{"foo[6]", "30"},
					{"foo.index", "0"},
					{"foo.index", "4"},
					{"foo.index", "6"}
				};

			var coll = new List<int>();
			var controllerContext = new ControllerContext();
			var bindingContext = new ModelBindingContext
				{
					ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => coll, typeof(List<int>)),
					ModelName = "foo",
					ModelState = new ModelStateDictionary(),
					ValueProvider = new BetterNameValueCollectionValueProvider(formParams, formParams, CultureInfo.InvariantCulture)
				};

			var sut = new TestableBetterDefaultModelBinder();

			var collection = sut.PublicUpdateCollection(controllerContext, bindingContext, typeof (int)) as IList<int>;

			bindingContext.ModelState["foo.index"].Value.AttemptedValue.ShouldEqual("0,4,6");
			var indexes = bindingContext.ModelState["foo.index"].Value.ConvertTo<string[]>();

			indexes.ShouldNotBeNull();
			indexes[0].ShouldEqual("0");
			indexes[1].ShouldEqual("4");
			indexes[2].ShouldEqual("6");

			Assert.NotNull(collection);
			collection[0].ShouldEqual(10);
			collection[1].ShouldEqual(20);
			collection[2].ShouldEqual(30);
		}

		class MyModel
		{
		}

		[Fact]
		public void BindModel_ForSimpleTypeAndFallbackToEmptyPrefix_ReturnsNullIfAnyNonRequiredKeysAreFound()
		{
			// Arrange
			var bindingContext = new ModelBindingContext
				{
					FallbackToEmptyPrefix = true,
					ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(int)),
					ModelName = "foo",
					ValueProvider = new SimpleValueProvider { { "something", "in the way she smiles" } }
				};

			var binder = new BetterDefaultModelBinder();

			// Act
			object returnedModel = binder.BindModel(new ControllerContext(), bindingContext);

			// Assert
			Assert.Null(returnedModel);
		}

		[Fact]
		public void BindModel_ForComplexTypeAndFallbackToEmptyPrefix_ReturnsNullIfKeyNotFound()
		{
			// Arrange
			var bindingContext = new ModelBindingContext
				{
					FallbackToEmptyPrefix = true,
					ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(MyModel)),
					ModelName = "foo",
					ValueProvider = new SimpleValueProvider()
				};

			var binder = new BetterDefaultModelBinder();

			// Act
			object returnedModel = binder.BindModel(new ControllerContext(), bindingContext);

			// Assert
			Assert.Null(returnedModel);
		}

		[Fact]
		public void BindModel_ForComplexTypeAndFallbackToEmptyPrefix_ReturnsNewModelIfAnyNonRequiredKeysAreFound()
		{
			// Arrange
			var bindingContext = new ModelBindingContext
				{
					FallbackToEmptyPrefix = true,
					ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(MyModel)),
					ModelName = "foo",
					ValueProvider = new SimpleValueProvider { { "something", "in the way she smiles" } }
				};

			var binder = new BetterDefaultModelBinder();

			// Act
			object returnedModel = binder.BindModel(new ControllerContext(), bindingContext);

			// Assert
			Assert.NotNull(returnedModel);
		}
	}
}
