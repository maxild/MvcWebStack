using System.Web.Mvc;
using Maxfire.TestCommons;
using Xunit;

namespace Maxfire.Web.Mvc.UnitTests
{
	public class BetterDefaultModelBinderTests
	{
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
			AssertEx.Null(returnedModel);
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
			AssertEx.Null(returnedModel);
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
			AssertEx.NotNull(returnedModel);
		}
	}
}