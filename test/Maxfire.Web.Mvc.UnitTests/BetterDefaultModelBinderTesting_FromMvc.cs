using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Maxfire.TestCommons;
using Moq;
using Moq.Protected;
using Xunit;

namespace Maxfire.Web.Mvc.UnitTests
{
	[SuppressMessage("ReSharper", "UnusedMember.Local")]
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
	[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
	public class ExtensibleDefaultModelBinderTests
	{
		[Fact]
		public void BindComplexElementalModelReturnsIfOnModelUpdatingReturnsFalse()
		{
			// Arrange
			ControllerContext controllerContext = new Mock<ControllerContext>().Object;

			var model = new MyModel { ReadWriteProperty = 3 };
			var bindingContext = new ModelBindingContext
			{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
			};

			var mockHelper = new Mock<TestableExtensibleDefaultModelBinder> { CallBase = true };
			mockHelper
				.Setup(b => b.PublicOnModelUpdating(controllerContext, It.IsAny<ModelBindingContext>()))
				.Returns(false);
			TestableExtensibleDefaultModelBinder helper = mockHelper.Object;

			// Act
			helper.PublicBindComplexElementalModel(controllerContext, bindingContext, model);

			// Assert
			Assert.Equal(3, model.ReadWriteProperty);
			mockHelper.Verify();
			mockHelper.Verify(b => b.PublicGetModelProperties(controllerContext, It.IsAny<ModelBindingContext>()), Times.Never());
			mockHelper.Verify(b => b.PublicBindProperty(controllerContext, It.IsAny<ModelBindingContext>(), It.IsAny<PropertyDescriptor>()), Times.Never());
		}

		[Fact]
		public void BindComplexModelCanBindArrays()
		{
			// Arrange
			var controllerContext = new Mock<ControllerContext>().Object;

			var bindingContext = new ModelBindingContext
			{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(int[])),
				ModelName = "foo",
				PropertyFilter = _ => false,
				ValueProvider = new SimpleValueProvider
				{
					{ "foo[0]", null },
					{ "foo[1]", null },
					{ "foo[2]", null }
				}
			};

			var mockInnerBinder = new Mock<IModelBinder>();
			mockInnerBinder
				.Setup(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
				.Returns(
					(ControllerContext cc, ModelBindingContext bc) =>
						{
							Assert.Equal(controllerContext, cc);
							Assert.Equal(typeof (int), bc.ModelType);
							Assert.Equal(bindingContext.ModelState, bc.ModelState);
							Assert.Equal(bindingContext.PropertyFilter, bc.PropertyFilter);
							Assert.Equal(bindingContext.ValueProvider, bc.ValueProvider);
							return Int32.Parse(bc.ModelName.Substring(4, 1), CultureInfo.InvariantCulture);
						});

			var binder = new TestableExtensibleDefaultModelBinder
			{
				Binders = new ModelBinderDictionary
				{
					{ typeof(int), mockInnerBinder.Object }
				}
			};

			// Act
			object newModel = binder.PublicBindComplexModel(controllerContext, bindingContext);

			// Assert
			var newIntArray = Assert.IsType<int[]>(newModel);
			Assert.Equal(new[] { 0, 1, 2 }, newIntArray);
		}

		[Fact]
		public void BindComplexModelCanBindCollections()
		{
			// Arrange
			ControllerContext controllerContext = new Mock<ControllerContext>().Object;

			var bindingContext = new ModelBindingContext
			{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(IList<int>)),
				ModelName = "foo",
				PropertyFilter = _ => false,
				ValueProvider = new SimpleValueProvider
				{
					{ "foo[0]", null },
					{ "foo[1]", null },
					{ "foo[2]", null }
				}
			};

			var mockInnerBinder = new Mock<IModelBinder>();
			mockInnerBinder
				.Setup(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
				.Returns(
					(ControllerContext cc, ModelBindingContext bc) =>
						{
							Assert.Equal(controllerContext, cc);
							Assert.Equal(typeof (int), bc.ModelType);
							Assert.Equal(bindingContext.ModelState, bc.ModelState);
							Assert.Equal(bindingContext.PropertyFilter, bc.PropertyFilter);
							Assert.Equal(bindingContext.ValueProvider, bc.ValueProvider);
							return Int32.Parse(bc.ModelName.Substring(4, 1), CultureInfo.InvariantCulture);
						});

			var binder = new TestableExtensibleDefaultModelBinder
				{
				Binders = new ModelBinderDictionary
					{
					{ typeof(int), mockInnerBinder.Object }
				}
			};

			// Act
			object newModel = binder.PublicBindComplexModel(controllerContext, bindingContext);

			// Assert
			var modelAsList = Assert.IsAssignableFrom<IList<int>>(newModel);
			Assert.Equal(new[] { 0, 1, 2 }, modelAsList.ToArray());
		}

		[Fact]
		public void BindComplexModelCanBindDictionariesWithDotsNotation()
		{
			// Arrange
			ControllerContext controllerContext = new Mock<ControllerContext>().Object;

			var bindingContext = new ModelBindingContext
			{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(IDictionary<string, CountryState>)),
				ModelName = "countries",
				PropertyFilter = _ => true,
				ValueProvider = new DictionaryValueProvider<object>(new Dictionary<string, object>
					{
					{ "countries.CA.Name", "Canada" },
					{ "countries.CA.States[0]", "Québec" },
					{ "countries.CA.States[1]", "British Columbia" },
					{ "countries.US.Name", "United States" },
					{ "countries.US.States[0]", "Washington" },
					{ "countries.US.States[1]", "Oregon" }
				}, CultureInfo.CurrentCulture)
			};

			var binder = new TestableExtensibleDefaultModelBinder();

			// Act
			object newModel = binder.PublicBindComplexModel(controllerContext, bindingContext);

			// Assert
			var modelAsDictionary = Assert.IsAssignableFrom<IDictionary<string, CountryState>>(newModel);
			Assert.Equal(2, modelAsDictionary.Count);
			Assert.Equal("Canada", modelAsDictionary["CA"].Name);
			Assert.Equal("United States", modelAsDictionary["US"].Name);
			Assert.Equal(2, modelAsDictionary["CA"].States.Count());
			Assert.Contains("Québec", modelAsDictionary["CA"].States);
			Assert.Contains("British Columbia", modelAsDictionary["CA"].States);
			Assert.Equal(2, modelAsDictionary["US"].States.Count());
			Assert.Contains("Washington", modelAsDictionary["US"].States);
			Assert.Contains("Oregon", modelAsDictionary["US"].States);
		}

		[Fact]
		public void BindComplexModelCanBindDictionariesWithBracketsNotation()
		{
			// Arrange
			ControllerContext controllerContext = new Mock<ControllerContext>().Object;

			var bindingContext = new ModelBindingContext
			{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(IDictionary<string, CountryState>)),
				ModelName = "countries",
				PropertyFilter = _ => true,
				ValueProvider = new DictionaryValueProvider<object>(new Dictionary<string, object>
					{
					{ "countries[CA].Name", "Canada" },
					{ "countries[CA].States[0]", "Québec" },
					{ "countries[CA].States[1]", "British Columbia" },
					{ "countries[US].Name", "United States" },
					{ "countries[US].States[0]", "Washington" },
					{ "countries[US].States[1]", "Oregon" }
				}, CultureInfo.CurrentCulture)
			};

			var binder = new TestableExtensibleDefaultModelBinder();

			// Act
			object newModel = binder.PublicBindComplexModel(controllerContext, bindingContext);

			// Assert
			var modelAsDictionary = Assert.IsAssignableFrom<IDictionary<string, CountryState>>(newModel);
			Assert.Equal(2, modelAsDictionary.Count);
			Assert.Equal("Canada", modelAsDictionary["CA"].Name);
			Assert.Equal("United States", modelAsDictionary["US"].Name);
			Assert.Equal(2, modelAsDictionary["CA"].States.Count());
			Assert.Contains("Québec", modelAsDictionary["CA"].States);
			Assert.Contains("British Columbia", modelAsDictionary["CA"].States);
			Assert.Equal(2, modelAsDictionary["US"].States.Count());
			Assert.Contains("Washington", modelAsDictionary["US"].States);
			Assert.Contains("Oregon", modelAsDictionary["US"].States);
		}

		[Fact]
		public void BindComplexModelCanBindDictionariesWithBracketsAndDotsNotation()
		{
			// Arrange
			ControllerContext controllerContext = new Mock<ControllerContext>().Object;

			var bindingContext = new ModelBindingContext
			{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(IDictionary<string, CountryState>)),
				ModelName = "countries",
				PropertyFilter = _ => true,
				ValueProvider = new DictionaryValueProvider<object>(new Dictionary<string, object>
					{
					{ "countries[CA].Name", "Canada" },
					{ "countries[CA].States[0]", "Québec" },
					{ "countries.CA.States[1]", "British Columbia" },
					{ "countries.US.Name", "United States" },
					{ "countries.US.States[0]", "Washington" },
					{ "countries.US.States[1]", "Oregon" }
				}, CultureInfo.CurrentCulture)
			};

			var binder = new TestableExtensibleDefaultModelBinder();

			// Act
			object newModel = binder.PublicBindComplexModel(controllerContext, bindingContext);

			// Assert
			var modelAsDictionary = Assert.IsAssignableFrom<IDictionary<string, CountryState>>(newModel);
			Assert.Equal(2, modelAsDictionary.Count);
			Assert.Equal("Canada", modelAsDictionary["CA"].Name);
			Assert.Equal("United States", modelAsDictionary["US"].Name);
			Assert.Single(modelAsDictionary["CA"].States);
			Assert.Contains("Québec", modelAsDictionary["CA"].States);

			// We do not accept double notation for a same entry, so we can't find that state.
			Assert.DoesNotContain("British Columbia", modelAsDictionary["CA"].States);
			Assert.Equal(2, modelAsDictionary["US"].States.Count());
			Assert.Contains("Washington", modelAsDictionary["US"].States);
			Assert.Contains("Oregon", modelAsDictionary["US"].States);
		}

		[Fact]
		public void BindComplexModelCanBindDictionaries()
		{
			// Arrange
			ControllerContext controllerContext = new Mock<ControllerContext>().Object;

			var bindingContext = new ModelBindingContext
			{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(IDictionary<int, string>)),
				ModelName = "foo",
				PropertyFilter = _ => false,
				ValueProvider = new SimpleValueProvider
					{
					{ "foo[0].key", null }, { "foo[0].value", null },
					{ "foo[1].key", null }, { "foo[1].value", null },
					{ "foo[2].key", null }, { "foo[2].value", null }
				}
			};

			var mockIntBinder = new Mock<IModelBinder>();
			mockIntBinder
				.Setup(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
				.Returns(
					delegate(ControllerContext cc, ModelBindingContext bc)
					{
						Assert.Equal(controllerContext, cc);
						Assert.Equal(typeof(int), bc.ModelType);
						Assert.Equal(bindingContext.ModelState, bc.ModelState);
						Assert.Equal(new ModelBindingContext().PropertyFilter, bc.PropertyFilter);
						Assert.Equal(bindingContext.ValueProvider, bc.ValueProvider);
						return Int32.Parse(bc.ModelName.Substring(4, 1), CultureInfo.InvariantCulture) + 10;
					});

			var mockStringBinder = new Mock<IModelBinder>();
			mockStringBinder
				.Setup(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
				.Returns(
					(ControllerContext cc, ModelBindingContext bc) =>
						{
							Assert.Equal(controllerContext, cc);
							Assert.Equal(typeof (string), bc.ModelType);
							Assert.Equal(bindingContext.ModelState, bc.ModelState);
							Assert.Equal(bindingContext.PropertyFilter, bc.PropertyFilter);
							Assert.Equal(bindingContext.ValueProvider, bc.ValueProvider);
							return (Int32.Parse(bc.ModelName.Substring(4, 1), CultureInfo.InvariantCulture) + 10) + "Value";
						});

			var binder = new TestableExtensibleDefaultModelBinder
			{
				Binders = new ModelBinderDictionary
				{
					{ typeof(int), mockIntBinder.Object },
					{ typeof(string), mockStringBinder.Object }
				}
			};

			// Act
			object newModel = binder.PublicBindComplexModel(controllerContext, bindingContext);

			// Assert
			var modelAsDictionary = Assert.IsAssignableFrom<IDictionary<int, string>>(newModel);
			Assert.Equal(3, modelAsDictionary.Count);
			Assert.Equal("10Value", modelAsDictionary[10]);
			Assert.Equal("11Value", modelAsDictionary[11]);
			Assert.Equal("12Value", modelAsDictionary[12]);
		}

		[Fact]
		public void BindComplexModelCanBindObjects()
		{
			// Arrange
			ControllerContext controllerContext = new Mock<ControllerContext>().Object;

			var model = new ModelWithoutBindAttribute
			{
				Foo = "FooPreValue",
				Bar = "BarPreValue",
				Baz = "BazPreValue",
			};
			var bindingContext = new ModelBindingContext
			{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
				ValueProvider = new SimpleValueProvider { { "Foo", null }, { "Bar", null } }
			};

			var mockInnerBinder = new Mock<IModelBinder>();
			mockInnerBinder
				.Setup(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
				.Returns(
					(ControllerContext cc, ModelBindingContext bc) =>
						{
							Assert.Equal(controllerContext, cc);
							Assert.Equal(bindingContext.ValueProvider, bc.ValueProvider);
							return bc.ModelName + "PostValue";
						});

			var binder = new TestableExtensibleDefaultModelBinder
			{
				Binders = new ModelBinderDictionary
				{
					{ typeof(string), mockInnerBinder.Object }
				}
			};

			// Act
			object updatedModel = binder.PublicBindComplexModel(controllerContext, bindingContext);

			// Assert
			Assert.Same(model, updatedModel);
			Assert.Equal("FooPostValue", model.Foo);
			Assert.Equal("BarPostValue", model.Bar);
			Assert.Equal("BazPreValue", model.Baz);
		}

		[Fact]
		public void BindComplexModelReturnsNullArrayIfNoValuesProvided()
		{
			// Arrange
			var bindingContext = new ModelBindingContext
			{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(int[])),
				ModelName = "foo",
				ValueProvider = new SimpleValueProvider { { "foo", null } }
			};

			var mockInnerBinder = new Mock<IModelBinder>();
			mockInnerBinder
				.Setup(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
				.Returns(
					(ControllerContext cc, ModelBindingContext bc) =>
					Int32.Parse(bc.ModelName.Substring(4, 1), CultureInfo.InvariantCulture));

			var binder = new TestableExtensibleDefaultModelBinder
			{
				Binders = new ModelBinderDictionary
					{
					{ typeof(int), mockInnerBinder.Object }
				}
			};

			// Act
			object newModel = binder.PublicBindComplexModel(null, bindingContext);

			// Assert
			Assert.Null(newModel);
		}

		[Fact]
		public void BindComplexModelWhereModelTypeContainsBindAttribute()
		{
			// Arrange
			ControllerContext controllerContext = new Mock<ControllerContext>().Object;

			var model = new ModelWithBindAttribute
			{
				Foo = "FooPreValue",
				Bar = "BarPreValue",
				Baz = "BazPreValue",
			};
			var bindingContext = new ModelBindingContext
			{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
				ValueProvider = new SimpleValueProvider { { "Foo", null }, { "Bar", null } }
			};

			var mockInnerBinder = new Mock<IModelBinder>();
			mockInnerBinder
				.Setup(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
				.Returns(
					(ControllerContext cc, ModelBindingContext bc) =>
						{
							Assert.Equal(controllerContext, cc);
							Assert.Equal(bindingContext.ValueProvider, bc.ValueProvider);
							return bc.ModelName + "PostValue";
						});

			var binder = new TestableExtensibleDefaultModelBinder
				{
				Binders = new ModelBinderDictionary
					{
					{ typeof(string), mockInnerBinder.Object }
				}
			};

			// Act
			binder.PublicBindComplexModel(controllerContext, bindingContext);

			// Assert
			Assert.Equal("FooPreValue", model.Foo);
			Assert.Equal("BarPostValue", model.Bar);
			Assert.Equal("BazPreValue", model.Baz);
		}

		[Fact]
		public void BindComplexModelWhereModelTypeDoesNotContainBindAttribute()
		{
			// Arrange
			ControllerContext controllerContext = new Mock<ControllerContext>().Object;

			var model = new ModelWithoutBindAttribute
			{
				Foo = "FooPreValue",
				Bar = "BarPreValue",
				Baz = "BazPreValue",
			};
			var bindingContext = new ModelBindingContext
			{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
				ValueProvider = new SimpleValueProvider { { "Foo", null }, { "Bar", null } }
			};

			var mockInnerBinder = new Mock<IModelBinder>();
			mockInnerBinder
				.Setup(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
				.Returns(
					(ControllerContext cc, ModelBindingContext bc) =>
						{
							Assert.Equal(controllerContext, cc);
							Assert.Equal(bindingContext.ValueProvider, bc.ValueProvider);
							return bc.ModelName + "PostValue";
						});

			var binder = new TestableExtensibleDefaultModelBinder
				{
				Binders = new ModelBinderDictionary
					{
					{ typeof(string), mockInnerBinder.Object }
				}
			};

			// Act
			binder.PublicBindComplexModel(controllerContext, bindingContext);

			// Assert
			Assert.Equal("FooPostValue", model.Foo);
			Assert.Equal("BarPostValue", model.Bar);
			Assert.Equal("BazPreValue", model.Baz);
		}

		//// BindModel tests

		[Fact]
		public void BindModelCanBindObjects()
		{
			// Arrange
			ControllerContext controllerContext = new Mock<ControllerContext>().Object;

			var model = new ModelWithoutBindAttribute
			{
				Foo = "FooPreValue",
				Bar = "BarPreValue",
				Baz = "BazPreValue",
			};
			var bindingContext = new ModelBindingContext
			{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
				ValueProvider = new SimpleValueProvider { { "Foo", null }, { "Bar", null } }
			};

			var mockInnerBinder = new Mock<IModelBinder>();
			mockInnerBinder
				.Setup(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
				.Returns(
					(ControllerContext cc, ModelBindingContext bc) =>
						{
							Assert.Equal(controllerContext, cc);
							Assert.Equal(bindingContext.ValueProvider, bc.ValueProvider);
							return bc.ModelName + "PostValue";
						});

			var binder = new TestableExtensibleDefaultModelBinder
				{
				Binders = new ModelBinderDictionary
					{
					{ typeof(string), mockInnerBinder.Object }
				}
			};

			// Act
			object updatedModel = binder.BindModel(controllerContext, bindingContext);

			// Assert
			Assert.Same(model, updatedModel);
			Assert.Equal("FooPostValue", model.Foo);
			Assert.Equal("BarPostValue", model.Bar);
			Assert.Equal("BazPreValue", model.Baz);
		}

		[Fact]
		public void BindModelCanBindSimpleTypes()
		{
			// Arrange
			var bindingContext = new ModelBindingContext
			{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(int)),
				ModelName = "foo",
				ValueProvider = new SimpleValueProvider
				{
					{ "foo", "42" }
				}
			};

			var binder = new ExtensibleDefaultModelBinder();

			// Act
			object updatedModel = binder.BindModel(new ControllerContext(), bindingContext);

			// Assert
			Assert.Equal(42, updatedModel);
		}

		[Fact]
		public void BindModel_PerformsValidationByDefault()
		{
			// Arrange
			ModelMetadata metadata = new DataAnnotationsModelMetadataProvider().GetMetadataForType(null, typeof(string));

			var controllerContext = new ControllerContext {Controller = new SimpleController()};

			var bindingContext = new ModelBindingContext
			{
				ModelMetadata = metadata,
				ModelName = "foo",
				ValueProvider = new CustomUnvalidatedValueProvider()
			};

			var binder = new ExtensibleDefaultModelBinder();

			// Act
			object updatedModel = binder.BindModel(controllerContext, bindingContext);

			// Assert
			Assert.Equal("fooValidated", updatedModel);
		}

		[Fact]
		public void BindModel_SkipsRequestValidationIfControllerOptsOut()
		{
			// Arrange
			ModelMetadata metadata = new DataAnnotationsModelMetadataProvider().GetMetadataForType(null, typeof(string));

			var controllerContext = new ControllerContext
				{
					Controller = new SimpleController { ValidateRequest = false } // [ValidateInput(false)] on controller/action
				};

			var bindingContext = new ModelBindingContext
			{
				ModelMetadata = metadata,
				ModelName = "foo",
				ValueProvider = new CustomUnvalidatedValueProvider()
			};

			var binder = new ExtensibleDefaultModelBinder();

			// Act
			object updatedModel = binder.BindModel(controllerContext, bindingContext);

			// Assert
			Assert.Equal("fooUnvalidated", updatedModel);
		}

		[Fact]
		public void BindModel_SkipsRequestValidationIfModelOptsOut()
		{
			// Arrange
			ModelMetadata metadata = new DataAnnotationsModelMetadataProvider().GetMetadataForType(null, typeof(string));
			metadata.RequestValidationEnabled = false; // [AllowHtml] on model

			var controllerContext = new ControllerContext {Controller = new SimpleController()};

			var bindingContext = new ModelBindingContext
			{
				ModelMetadata = metadata,
				ModelName = "foo",
				ValueProvider = new CustomUnvalidatedValueProvider()
			};

			var binder = new ExtensibleDefaultModelBinder();

			// Act
			object updatedModel = binder.BindModel(controllerContext, bindingContext);

			// Assert
			Assert.Equal("fooUnvalidated", updatedModel);
		}

		[Fact]
		public void BindModel_ForSimpleType_ReturnsNullIfKeyNotFound()
		{
			// Arrange
			var bindingContext = new ModelBindingContext
			{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(int)),
				ModelName = "foo",
				ValueProvider = new SimpleValueProvider()
			};

			var binder = new ExtensibleDefaultModelBinder();

			// Act
			object returnedModel = binder.BindModel(new ControllerContext(), bindingContext);

			// Assert
			Assert.Null(returnedModel);
		}

		[Fact]
		public void BindModel_ForComplexType_ReturnsNullIfKeyNotFound()
		{
			// Arrange
			var bindingContext = new ModelBindingContext
			{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(MyModel)),
				ModelName = "foo",
				ValueProvider = new SimpleValueProvider()
			};

			var binder = new ExtensibleDefaultModelBinder();

			// Act
			object returnedModel = binder.BindModel(new ControllerContext(), bindingContext);

			// Assert
			Assert.Null(returnedModel);
		}

		[Fact]
		public void BindModel_ForSimpleTypeAndFallbackToEmptyPrefix_ReturnsNullIfKeyNotFound()
		{
			// Arrange
			var bindingContext = new ModelBindingContext
			{
				FallbackToEmptyPrefix = true,
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(int)),
				ModelName = "foo",
				ValueProvider = new SimpleValueProvider()
			};

			var binder = new ExtensibleDefaultModelBinder();

			// Act
			object returnedModel = binder.BindModel(new ControllerContext(), bindingContext);

			// Assert
			Assert.Null(returnedModel);
		}

		[Fact]
		public void BindModel_ForComplexTypeAndFallbackToEmptyPrefix_ReturnsNewModelIfKeyNotFound()
		{
			// Arrange
			var bindingContext = new ModelBindingContext
			{
				FallbackToEmptyPrefix = true,
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(MyModel)),
				ModelName = "foo",
				ValueProvider = new SimpleValueProvider()
			};

			var binder = new ExtensibleDefaultModelBinder();

			// Act
			object returnedModel = binder.BindModel(new ControllerContext(), bindingContext);

			// Assert
			Assert.NotNull(returnedModel);
		}

		[Fact]
		public void BindModelThrowsIfBindingContextIsNull()
		{
			// Arrange
			var binder = new ExtensibleDefaultModelBinder();

			// Act & assert
			Assert.Throws<ArgumentNullException>(() => binder.BindModel(new ControllerContext(), null));
		}

		[Fact]
		public void BindModelValuesCanBeOverridden()
		{
			// Arrange
			var bindingContext = new ModelBindingContext
			{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => new ModelWithoutBindAttribute(), typeof(ModelWithoutBindAttribute)),
				ValueProvider = new SimpleValueProvider
					{
					{ "foo", "FooPostValue" },
					{ "bar", "BarPostValue" },
					{ "baz", "BazPostValue" }
				}
			};

			var binder = new Mock<TestableExtensibleDefaultModelBinder> { CallBase = true };
			binder.Setup(b => b.PublicGetPropertyValue(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>(),
											           It.IsAny<PropertyDescriptor>(), It.IsAny<IModelBinder>()))
				.Returns("Hello, world!");

			// Act
			var model = (ModelWithoutBindAttribute)binder.Object.BindModel(new ControllerContext(), bindingContext);

			// Assert
			Assert.Equal("Hello, world!", model.Bar);
			Assert.Equal("Hello, world!", model.Baz);
			Assert.Equal("Hello, world!", model.Foo);
		}

		[Fact]
		public void BindModelWithTypeConversionErrorUpdatesModelStateMessage()
		{
			// Arrange
			var bindingContext = new ModelBindingContext
			{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => new PropertyTestingModel(), typeof(PropertyTestingModel)),
				ValueProvider = new SimpleValueProvider
				{
					{ "IntReadWrite", "foo" }
				},
			};
			var binder = new ExtensibleDefaultModelBinder();

			// Act
			binder.BindModel(new ControllerContext(), bindingContext);

			// Assert
			ModelState modelState = bindingContext.ModelState["IntReadWrite"];
			Assert.NotNull(modelState);
			Assert.Single(modelState.Errors);
			Assert.Equal("The value 'foo' is not valid for IntReadWrite.", modelState.Errors[0].ErrorMessage);
		}

		[Fact]
		public void BindModelWithPrefix()
		{
			// Arrange
			var model = new ModelWithoutBindAttribute
			{
				Foo = "FooPreValue",
				Bar = "BarPreValue",
				Baz = "BazPreValue",
			};
			var bindingContext = new ModelBindingContext
			{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
				ModelName = "prefix",
				ValueProvider = new SimpleValueProvider
				{
					{ "prefix.foo", "FooPostValue" },
					{ "prefix.bar", "BarPostValue" }
				}
			};

			var binder = new ExtensibleDefaultModelBinder();

			// Act
			object updatedModel = binder.BindModel(new ControllerContext(), bindingContext);

			// Assert
			Assert.Same(model, updatedModel);
			Assert.Equal("FooPostValue", model.Foo);
			Assert.Equal("BarPostValue", model.Bar);
			Assert.Equal("BazPreValue", model.Baz);
		}

		[Fact]
		public void BindModelWithPrefixAndFallback()
		{
			// Arrange
			var model = new ModelWithoutBindAttribute
			{
				Foo = "FooPreValue",
				Bar = "BarPreValue",
				Baz = "BazPreValue",
			};
			var bindingContext = new ModelBindingContext
			{
				FallbackToEmptyPrefix = true,
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
				ModelName = "prefix",
				ValueProvider = new SimpleValueProvider
					{
					{ "foo", "FooPostValue" },
					{ "bar", "BarPostValue" }
				}
			};

			var binder = new ExtensibleDefaultModelBinder();

			// Act
			object updatedModel = binder.BindModel(new ControllerContext(), bindingContext);

			// Assert
			Assert.Same(model, updatedModel);
			Assert.Equal("FooPostValue", model.Foo);
			Assert.Equal("BarPostValue", model.Bar);
			Assert.Equal("BazPreValue", model.Baz);
		}

		[Fact]
		public void BindModelWithPrefixReturnsNullIfFallbackNotSpecifiedAndValueProviderContainsNoEntries()
		{
			// Arrange
			var model = new ModelWithoutBindAttribute
			{
				Foo = "FooPreValue",
				Bar = "BarPreValue",
				Baz = "BazPreValue",
			};
			var bindingContext = new ModelBindingContext
			{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
				ModelName = "prefix",
				ValueProvider = new SimpleValueProvider
					{
					{ "foo", "FooPostValue" },
					{ "bar", "BarPostValue" }
				}
			};

			var binder = new ExtensibleDefaultModelBinder();

			// Act
			object updatedModel = binder.BindModel(new ControllerContext(), bindingContext);

			// Assert
			Assert.Null(updatedModel);
		}

		[Fact]
		public void BindModelReturnsNullIfSimpleTypeNotFound()
		{
			// DevDiv 216165: ModelBinders should not try and instantiate simple types

			// Arrange
			var bindingContext = new ModelBindingContext
			{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(string)),
				ModelName = "prefix",
				ValueProvider = new SimpleValueProvider
				{
					{ "prefix.foo", "foo" },
					{ "prefix.bar", "bar" }
				}
			};

			var binder = new ExtensibleDefaultModelBinder();

			// Act
			object updatedModel = binder.BindModel(new ControllerContext(), bindingContext);

			// Assert
			Assert.Null(updatedModel);
		}

		//// BindProperty tests

		[Fact]
		public void BindPropertyCanUpdateComplexReadOnlyProperties()
		{
			// Arrange
			// the Customer type contains a single read-only Address property
			var model = new Customer();
			var bindingContext = new ModelBindingContext
			{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
				ValueProvider = new SimpleValueProvider { { "Address", null } }
			};

			// The 'AddressModelBinder' will do the updating of the Address object
			// Note: Simple properties need to be read-write!!!!
			// Note: Complecx properties can be read-only
			var mockInnerBinder = new Mock<IModelBinder>();
			mockInnerBinder
				.Setup(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
				.Returns(
					(ControllerContext cc, ModelBindingContext bc) =>
						{
							var address = (Address) bc.Model;
							address.Street = "1 Microsoft Way";
							address.Zip = "98052";
							return address;
						});

			PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(model)["Address"];
			var helper = new TestableExtensibleDefaultModelBinder
				{
				Binders = new ModelBinderDictionary
				{
					{ typeof(Address), mockInnerBinder.Object }
				}
			};

			// Act
			helper.PublicBindProperty(new ControllerContext(), bindingContext, propertyDescriptor);

			// Assert
			Assert.Equal("1 Microsoft Way", model.Address.Street);
			Assert.Equal("98052", model.Address.Zip);
		}

		[Fact]
		public void BindPropertyDoesNothingIfValueProviderContainsNoEntryForProperty()
		{
			// Arrange
			var model = new MyModel2 { IntReadWrite = 3 };
			var bindingContext = new ModelBindingContext
			{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
				ValueProvider = new SimpleValueProvider()
			};

			PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(model)["IntReadWrite"];
			var helper = new TestableExtensibleDefaultModelBinder();

			// Act
			helper.PublicBindProperty(new ControllerContext(), bindingContext, propertyDescriptor);

			// Assert
			Assert.Equal(3, model.IntReadWrite);
		}

		[Fact]
		public void BindProperty()
		{
			// Arrange
			ControllerContext controllerContext = new Mock<ControllerContext>().Object;
			var model = new MyModel2 { IntReadWrite = 3 };
			var bindingContext = new ModelBindingContext
			{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
				ValueProvider = new SimpleValueProvider
				{
					{ "IntReadWrite", "42" }
				}
			};

			PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(model)["IntReadWrite"];

			var mockHelper = new Mock<TestableExtensibleDefaultModelBinder> { CallBase = true };
			mockHelper.Setup(b => b.PublicOnPropertyValidating(controllerContext, bindingContext, propertyDescriptor, 42)).Returns(true).Verifiable();
			mockHelper.Setup(b => b.PublicSetProperty(controllerContext, bindingContext, propertyDescriptor, 42)).Verifiable();
			mockHelper.Setup(b => b.PublicOnPropertyValidated(controllerContext, bindingContext, propertyDescriptor, 42)).Verifiable();
			var helper = mockHelper.Object;

			// Act
			helper.PublicBindProperty(controllerContext, bindingContext, propertyDescriptor);

			// Assert
			mockHelper.Verify();
		}

		[Fact]
		public void BindPropertyReturnsIfOnPropertyValidatingReturnsFalse()
		{
			// Arrange
			ControllerContext controllerContext = new Mock<ControllerContext>().Object;
			var model = new MyModel2 { IntReadWrite = 3 };
			var bindingContext = new ModelBindingContext
				{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
				ValueProvider = new SimpleValueProvider
					{
					{ "IntReadWrite", "42" }
				}
			};

			PropertyDescriptor pd = TypeDescriptor.GetProperties(model)["IntReadWrite"];

			var mockHelper = new Mock<TestableExtensibleDefaultModelBinder> { CallBase = true };
			mockHelper.Setup(b => b.PublicOnPropertyValidating(controllerContext, bindingContext, pd, 42)).Returns(false);
			var helper = mockHelper.Object;

			// Act
			helper.PublicBindProperty(controllerContext, bindingContext, pd);

			// Assert
			Assert.Equal(3, model.IntReadWrite);
			mockHelper.Verify();
			mockHelper.Verify(b => b.PublicSetProperty(controllerContext, bindingContext, pd, 42), Times.Never());
			mockHelper.Verify(b => b.PublicOnPropertyValidated(controllerContext, bindingContext, pd, 42), Times.Never());
		}

		[Fact]
		public void BindPropertySetsPropertyToNullIfUserLeftTextEntryFieldBlankForOptionalValue()
		{
			// Arrange
			var model = new MyModel2 { NullableIntReadWrite = 8 };
			var bindingContext = new ModelBindingContext
			{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
				ValueProvider = new SimpleValueProvider { { "NullableIntReadWrite", null } }
			};

			var mockInnerBinder = new Mock<IModelBinder>();
			mockInnerBinder.Setup(b => b.BindModel(new ControllerContext(), It.IsAny<ModelBindingContext>())).Returns(null);

			PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(model)["NullableIntReadWrite"];
			var helper = new TestableExtensibleDefaultModelBinder
			{
				Binders = new ModelBinderDictionary
					{
					{ typeof(int?), mockInnerBinder.Object }
				}
			};

			// Act
			helper.PublicBindProperty(new ControllerContext(), bindingContext, propertyDescriptor);

			// Note: Every posted key is updated regardless if the value is null or non-null

			// Assert
			Assert.Empty(bindingContext.ModelState);
			Assert.Null(model.NullableIntReadWrite);
		}

		[Fact]
		public void BindPropertyUpdatesPropertyOnFailureIfInnerBinderReturnsNonNullObject()
		{
			// Arrange
			var model = new MyModel2 { IntReadWriteNonNegative = 8 };
			var bindingContext = new ModelBindingContext
				{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
				ValueProvider = new SimpleValueProvider { { "IntReadWriteNonNegative", null } }
			};

			var mockInnerBinder = new Mock<IModelBinder>();
			mockInnerBinder
				.Setup(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
				.Returns(
					(ControllerContext cc, ModelBindingContext bc) =>
						{
							bc.ModelState.AddModelError("IntReadWriteNonNegative", "Some error text.");
							return 4;
						});

			PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(model)["IntReadWriteNonNegative"];
			var helper = new TestableExtensibleDefaultModelBinder
			{
				Binders = new ModelBinderDictionary
					{
					{ typeof(int), mockInnerBinder.Object }
				}
			};

			// Act
			helper.PublicBindProperty(new ControllerContext(), bindingContext, propertyDescriptor);

			// Assert
			Assert.False(bindingContext.ModelState.IsValidField("IntReadWriteNonNegative"));
			var error = Assert.Single(bindingContext.ModelState["IntReadWriteNonNegative"].Errors);
			Assert.Equal("Some error text.", error?.ErrorMessage);
			Assert.Equal(4, model.IntReadWriteNonNegative);
		}

		[Fact]
		public void BindPropertyUpdatesPropertyOnSuccess()
		{
			// Arrange
			// Effectively, this is just testing updating a single property named "IntReadWrite"
			ControllerContext controllerContext = new Mock<ControllerContext>().Object;

			var model = new MyModel2 { IntReadWrite = 3 };
			var bindingContext = new ModelBindingContext
			{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
				ModelName = "foo",
				ModelState = new ModelStateDictionary { { "blah", new ModelState() } },
				ValueProvider = new SimpleValueProvider { { "foo.IntReadWrite", null } }
			};

			var mockInnerBinder = new Mock<IModelBinder>();
			mockInnerBinder
				.Setup(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
				.Returns(
					(ControllerContext cc, ModelBindingContext bc) =>
						{
							Assert.Equal(controllerContext, cc);
							Assert.Equal(3, bc.Model);
							Assert.Equal(typeof (int), bc.ModelType);
							Assert.Equal("foo.IntReadWrite", bc.ModelName);
							Assert.Equal(new ModelBindingContext().PropertyFilter, bc.PropertyFilter);
							Assert.Equal(bindingContext.ModelState, bc.ModelState);
							Assert.Equal(bindingContext.ValueProvider, bc.ValueProvider);
							return 4;
						});

			PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(model)["IntReadWrite"];
			var helper = new TestableExtensibleDefaultModelBinder
			{
				Binders = new ModelBinderDictionary
				{
					{ typeof(int), mockInnerBinder.Object }
				}
			};

			// Act
			helper.PublicBindProperty(controllerContext, bindingContext, propertyDescriptor);

			// Assert
			Assert.Equal(4, model.IntReadWrite);
		}

		//// BindSimpleModel tests

		[Fact]
		public void BindSimpleModelCanReturnArrayTypes()
		{
			// Arrange
			// Note: BindSimpleType delegates to ValueProviderResult.ConvertTo that only uses RawValue
			var valueProviderResult = new ValueProviderResult(42, null, null);
			var bindingContext = new ModelBindingContext
			{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(int[])),
				ModelName = "foo",
			};

			var binder = new TestableExtensibleDefaultModelBinder();

			// Act
			object returnedValue = binder.PublicBindSimpleModel(null, bindingContext, valueProviderResult);

			// Assert
			var returnedValueAsIntArray = Assert.IsType<int[]>(returnedValue);
			Assert.Single(returnedValueAsIntArray);
			Assert.Equal(42, returnedValueAsIntArray[0]);
		}

		[Fact]
		public void BindSimpleModelCanReturnCollectionTypes()
		{
			// Arrange
			var result = new ValueProviderResult(new [] { "42", "82" }, null, null);
			var bindingContext = new ModelBindingContext
			{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(IEnumerable<int>)),
				ModelName = "foo",
			};

			var binder = new TestableExtensibleDefaultModelBinder();

			// Act
			object returnedValue = binder.PublicBindSimpleModel(null, bindingContext, result);

			// Assert
			var returnedValueAsList = Assert.IsAssignableFrom<IEnumerable<int>>(returnedValue).ToList();
			Assert.Equal(2, returnedValueAsList.Count);
			Assert.Equal(42, returnedValueAsList[0]);
			Assert.Equal(82, returnedValueAsList[1]);
		}

		[Fact]
		public void BindSimpleModelCanReturnElementalTypes()
		{
			// Arrange
			// Note: Simple types (int) are treated as individual (simple) types
			var result = new ValueProviderResult("42", null, null);
			var bindingContext = new ModelBindingContext
			{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(int)),
				ModelName = "foo",
			};

			var binder = new TestableExtensibleDefaultModelBinder();

			// Act
			object returnedValue = binder.PublicBindSimpleModel(null, bindingContext, result);

			// Assert
			Assert.Equal(42, returnedValue);
		}

		[Fact]
		public void BindSimpleModelCanReturnStrings()
		{
			// Arrange
			// Note: Strings are treated as individual (simple) types
			var result = new ValueProviderResult(new object[] { "42" }, null, null);
			var bindingContext = new ModelBindingContext
			{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(string)),
				ModelName = "foo",
			};

			var binder = new TestableExtensibleDefaultModelBinder();

			// Act
			object returnedValue = binder.PublicBindSimpleModel(null, bindingContext, result);

			// Assert
			Assert.Equal("42", returnedValue);
		}

		[Fact]
		public void BindSimpleModelChecksValueProviderResultRawValueTypeIfItIsAssignmentCompatible()
		{
			// Arrange
			var result = new ValueProviderResult(new MemoryStream(), null, null);
			var bindingContext = new ModelBindingContext
			{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(Stream)),
				ModelName = "foo",
			};

			var binder = new TestableExtensibleDefaultModelBinder();

			// Act
			object returnedValue = binder.PublicBindSimpleModel(null, bindingContext, result);

			// Assert
			Assert.Equal(result, bindingContext.ModelState["foo"].Value);
			Assert.Same(result.RawValue, returnedValue);
		}

		[Fact]
		public void BindSimpleModelPropagatesErrorsOnFailure()
		{
			// Arrange
			var result = new ValueProviderResult("invalid", null, null);
			var bindingContext = new ModelBindingContext
			{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(int)),
				ModelName = "foo",
			};

			var binder = new TestableExtensibleDefaultModelBinder();

			// Act
			object returnedValue = binder.PublicBindSimpleModel(null, bindingContext, result);

			// Assert
			Assert.False(bindingContext.ModelState.IsValidField("foo"));
			Assert.IsType<InvalidOperationException>(bindingContext.ModelState["foo"].Errors[0].Exception);
			Assert.Equal("The parameter conversion from type 'System.String' to type 'System.Int32' failed. See the inner exception for more information.", bindingContext.ModelState["foo"].Errors[0].Exception.Message);
			Assert.Null(returnedValue);
		}

		//// CreateComplexElementalModelBindingContext tests

		[Fact]
		public void CreateComplexElementalModelBindingContext_ReadsBindAttributeFromModelClass()
		{
			// Arrange
			var bindingContext = new ModelBindingContext
			{
				ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(SomeModel)),
				ModelName = "someName",
				ValueProvider = new SimpleValueProvider(),
				PropertyFilter = _ => true // all properties are whitelistet per default
			};

			var binder = new TestableExtensibleDefaultModelBinder();

			// Act
			ModelBindingContext newBindingContext = binder.PublicCreateComplexElementalModelBindingContext(new ControllerContext(), bindingContext, null);

			// Assert
			Assert.False(newBindingContext.PropertyFilter("foo"));
			Assert.True(newBindingContext.PropertyFilter("bar"));
		}


		[Fact]
		public void CreateComplexElementalModelBindingContext_DoesNotReadBindAttributeFromBuddyClass()
		{
			// Note: I dislike the 'buddy class' concept, and therefore BetterDefaultModelBinder do not use
			// the AssociatedMetadataTypeTypeDescriptionProvider from System.ComponentModel.DataAnnotations.
			// Instead it uses the more conventional plain vanilla TypeDescriptor from System.ComponentModel.

			// Arrange
			var originalBindingContext = new ModelBindingContext
			{
				ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(FooModel)),
				ModelName = "someName",
				ValueProvider = new SimpleValueProvider(),
				PropertyFilter = _ => true // all properties are whitelistet per default
			};

			var binder = new TestableExtensibleDefaultModelBinder();

			// Act
			ModelBindingContext newBindingContext = binder.PublicCreateComplexElementalModelBindingContext(new ControllerContext(), originalBindingContext, null);

			// Assert
			Assert.True(newBindingContext.PropertyFilter("foo"));
			Assert.True(newBindingContext.PropertyFilter("bar"));
		}

		[MetadataType(typeof(FooModelBuddyClass))]
		class FooModel
		{
			[Bind(Exclude = "foo")]
			private class FooModelBuddyClass
			{
				// Note: Buddy class not supported by BetterDefaultModelBinder
			}
		}

		[Bind(Exclude = "foo")]
		class SomeModel
		{
		}

		//// CreateModel tests

		[Fact]
		public void CreateInstanceCreatesModelInstance()
		{
			// Arrange
			var helper = new TestableExtensibleDefaultModelBinder();

			// Act
			object modelObj = helper.PublicCreateModel(null, null, typeof(Guid));

			// Assert
			Assert.Equal(Guid.Empty, modelObj);
		}

		[Fact]
		public void CreateInstanceCreatesModelInstanceForGenericICollection()
		{
			// Arrange
			var helper = new TestableExtensibleDefaultModelBinder();

			// Act
			object modelObj = helper.PublicCreateModel(null, null, typeof(ICollection<Guid>));

			// Assert
			Assert.IsAssignableFrom<ICollection<Guid>>(modelObj);
		}

		[Fact]
		public void CreateInstanceCreatesModelInstanceForGenericIDictionary()
		{
			// Arrange
			var helper = new TestableExtensibleDefaultModelBinder();

			// Act
			object modelObj = helper.PublicCreateModel(null, null, typeof(IDictionary<string, Guid>));

			// Assert
			Assert.IsAssignableFrom<IDictionary<string, Guid>>(modelObj);
		}

		[Fact]
		public void CreateInstanceCreatesModelInstanceForGenericIEnumerable()
		{
			// Arrange
			var helper = new TestableExtensibleDefaultModelBinder();

			// Act
			object modelObj = helper.PublicCreateModel(null, null, typeof(IEnumerable<Guid>));

			// Assert
			Assert.IsAssignableFrom<ICollection<Guid>>(modelObj);
		}

		[Fact]
		public void CreateInstanceCreatesModelInstanceForGenericIList()
		{
			// Arrange
			var helper = new TestableExtensibleDefaultModelBinder();

			// Act
			object modelObj = helper.PublicCreateModel(null, null, typeof(IList<Guid>));

			// Assert
			Assert.IsAssignableFrom<IList<Guid>>(modelObj);
		}

		//// CreateSubIndexName, CreateSubPropertyName tests

		[Fact]
		public void CreateSubIndexNameReturnsPrefixPlusIndex()
		{
			// Arrange
			var helper = new TestableExtensibleDefaultModelBinder();

			// Act
			string newName = helper.PublicCreateSubIndexName("somePrefix", 2);

			// Assert
			Assert.Equal("somePrefix[2]", newName);
		}

		[Fact]
		public void CreateSubPropertyNameReturnsPrefixPlusPropertyName()
		{
			// Arrange
			var helper = new TestableExtensibleDefaultModelBinder();

			// Act
			string newName = helper.PublicCreateSubPropertyName("somePrefix", "someProperty");

			// Assert
			Assert.Equal("somePrefix.someProperty", newName);
		}

		[Fact]
		public void CreateSubPropertyNameReturnsPropertyNameIfPrefixIsEmpty()
		{
			// Arrange
			var helper = new TestableExtensibleDefaultModelBinder();

			// Act
			string newName = helper.PublicCreateSubPropertyName(string.Empty, "someProperty");

			// Assert
			Assert.Equal("someProperty", newName);
		}

		[Fact]
		public void CreateSubPropertyNameReturnsPropertyNameIfPrefixIsNull()
		{
			// Arrange
			var helper = new TestableExtensibleDefaultModelBinder();

			// Act
			string newName = helper.PublicCreateSubPropertyName(null, "someProperty");

			// Assert
			Assert.Equal("someProperty", newName);
		}

		[Fact]
		public void GetFilteredModelPropertiesFiltersNonUpdateableProperties()
		{
			// Arrange
			var bindingContext = new ModelBindingContext
			{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(PropertyTestingModel)),
				PropertyFilter = new BindAttribute { Exclude = "Blacklisted" }.IsPropertyAllowed
			};

			var helper = new TestableExtensibleDefaultModelBinder();

			// Act
			var properties = new PropertyDescriptorCollection(helper.PublicGetFilteredModelProperties(null, bindingContext).ToArray());

			// Assert
			Assert.NotNull(properties["StringReadWrite"]);
			Assert.Null(properties["StringReadOnly"]);
			Assert.NotNull(properties["IntReadWrite"]);
			Assert.Null(properties["IntReadOnly"]);
			Assert.NotNull(properties["ArrayReadWrite"]);
			Assert.Null(properties["ArrayReadOnly"]);
			Assert.NotNull(properties["AddressReadWrite"]);
			Assert.NotNull(properties["AddressReadOnly"]);
			Assert.NotNull(properties["Whitelisted"]);
			Assert.Null(properties["Blacklisted"]);
			Assert.Equal(6, properties.Count);
		}

		[Fact]
		public void GetModelPropertiesReturnsUnfilteredPropertyList()
		{
			// Arrange
			var bindingContext = new ModelBindingContext
				{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(PropertyTestingModel)),
				PropertyFilter = new BindAttribute { Exclude = "Blacklisted" }.IsPropertyAllowed
			};

			var helper = new TestableExtensibleDefaultModelBinder();

			// Act
			PropertyDescriptorCollection properties = helper.PublicGetModelProperties(null, bindingContext);

			// Assert
			Assert.NotNull(properties["StringReadWrite"]);
			Assert.NotNull(properties["StringReadOnly"]);
			Assert.NotNull(properties["IntReadWrite"]);
			Assert.NotNull(properties["IntReadOnly"]);
			Assert.NotNull(properties["ArrayReadWrite"]);
			Assert.NotNull(properties["ArrayReadOnly"]);
			Assert.NotNull(properties["AddressReadWrite"]);
			Assert.NotNull(properties["AddressReadOnly"]);
			Assert.NotNull(properties["Whitelisted"]);
			Assert.NotNull(properties["Blacklisted"]);
			Assert.Equal(10, properties.Count);
		}

		[Fact]
		public void OnModelUpdatingReturnsTrue()
		{
			// By default, this method does nothing, so we just want to make sure it returns true

			// Arrange
			var helper = new TestableExtensibleDefaultModelBinder();

			// Act
			bool returned = helper.PublicOnModelUpdating(null, null);

			// Arrange
			Assert.True(returned);
		}

		//// OnModelUpdated tests

		[Fact]
		public void OnModelUpdatedCalledWhenOnModelUpdatingReturnsTrue()
		{
			// Arrange
			var bindingContext = new ModelBindingContext
			{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => new ModelWithoutBindAttribute(), typeof(ModelWithoutBindAttribute)),
				ModelName = "",
				ValueProvider = new SimpleValueProvider()
			};
			var binder = new Mock<DefaultModelBinder> { CallBase = true };
			binder.Protected().Setup<bool>("OnModelUpdating",
										   ItExpr.IsAny<ControllerContext>(), ItExpr.IsAny<ModelBindingContext>())
				.Returns(true);
			binder.Protected().Setup("OnModelUpdated",
									 ItExpr.IsAny<ControllerContext>(), ItExpr.IsAny<ModelBindingContext>())
				.Verifiable();

			// Act
			binder.Object.BindModel(new ControllerContext(), bindingContext);

			// Assert
			binder.Verify();
		}

		[Fact]
		public void OnModelUpdatedNotCalledWhenOnModelUpdatingReturnsFalse()
		{
			// Arrange
			var bindingContext = new ModelBindingContext
			{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => new ModelWithoutBindAttribute(), typeof(ModelWithoutBindAttribute)),
				ModelName = "",
				ValueProvider = new SimpleValueProvider()
			};
			var binder = new Mock<DefaultModelBinder> { CallBase = true };
			binder.Protected().Setup<bool>("OnModelUpdating",
										   ItExpr.IsAny<ControllerContext>(), ItExpr.IsAny<ModelBindingContext>())
				.Returns(false);

			// Act
			binder.Object.BindModel(new ControllerContext(), bindingContext);

			// Assert
			binder.Verify();
			binder.Protected().Verify("OnModelUpdated", Times.Never(),
									  ItExpr.IsAny<ControllerContext>(), ItExpr.IsAny<ModelBindingContext>());
		}

		[Fact]
		public void OnModelUpdatedDoesntAddNewMessagesWhenMessagesAlreadyExist()
		{
			// Arrange
			var binder = new TestableExtensibleDefaultModelBinder<SetPropertyModel>();
			binder.Context.ModelState.AddModelError(BASE_MODEL_NAME + ".NonNullableStringWithAttribute", "Some pre-existing error");

			// Act
			binder.OnModelUpdated();

			// Assert
			var modelState = binder.Context.ModelState[BASE_MODEL_NAME + ".NonNullableStringWithAttribute"];
			var error = Assert.Single(modelState.Errors);
			Assert.Equal("Some pre-existing error", error?.ErrorMessage);
		}

		[Fact]
		public void OnPropertyValidatingNotCalledOnPropertiesWithErrors()
		{
			// Arrange
			var model = new ModelWithoutBindAttribute();
			var bindingContext = new ModelBindingContext
			{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
				ModelName = "",
				ValueProvider = new SimpleValueProvider
				{
					{ "foo", "foo" }
				},
			};
			bindingContext.ModelState.AddModelError("foo", "Pre-existing error");
			var binder = new Mock<DefaultModelBinder> { CallBase = true };

			// Act
			binder.Object.BindModel(new ControllerContext(), bindingContext);

			// Assert
			binder.Verify();
			binder.Protected().Verify("OnPropertyValidating", Times.Never(),
									  ItExpr.IsAny<ControllerContext>(), ItExpr.IsAny<ModelBindingContext>(),
									  ItExpr.IsAny<PropertyDescriptor>(), ItExpr.IsAny<object>());
		}

		[Fact]
		public void ExtraValueRequiredMessageNotAddedForAlreadyInvalidProperty()
		{
			// Arrange
			var binder = new ExtensibleDefaultModelBinder();
			var bindingContext = new ModelBindingContext
			{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(MyModel)),
				ModelName = "theModel",
				ValueProvider = new SimpleValueProvider()
			};
			bindingContext.ModelState.AddModelError("theModel.ReadWriteProperty", "Existing Error Message");

			// Act
			binder.BindModel(new ControllerContext(), bindingContext);

			// Assert
			ModelState modelState = bindingContext.ModelState["theModel.ReadWriteProperty"];
			var error = Assert.Single(modelState.Errors);
			Assert.Equal("Existing Error Message", error?.ErrorMessage);
		}

		[Fact]
		public void OnPropertyValidatingReturnsTrueOnSuccess()
		{
			// Arrange
			var bindingContext = new ModelBindingContext
			{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(MyModel)),
				ModelName = "theModel"
			};

			PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(MyModel))["ReadWriteProperty"];
			var helper = new TestableExtensibleDefaultModelBinder();
			bindingContext.PropertyMetadata["ReadWriteProperty"].Model = 42;

			// Act
			bool returned = helper.PublicOnPropertyValidating(new ControllerContext(), bindingContext, property, 42);

			// Assert
			Assert.True(returned);
			Assert.Empty(bindingContext.ModelState);
		}

		[Fact]
		public void UpdateCollectionCreatesDefaultEntriesForInvalidElements()
		{
			// Arrange
			var model = new List<int> { 4, 5, 6, 7, 8 };
			var bindingContext = new ModelBindingContext
			{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
				ModelName = "foo",
				ValueProvider = new SimpleValueProvider
				{
					{ "foo[0]", null },
					{ "foo[1]", null },
					{ "foo[2]", null }
				}
			};

			var mockInnerBinder = new Mock<IModelBinder>();
			mockInnerBinder
				.Setup(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
				.Returns(
					(ControllerContext cc, ModelBindingContext bc) =>
						{
							int fooIdx = Int32.Parse(bc.ModelName.Substring(4, 1), CultureInfo.InvariantCulture);
							return (fooIdx == 1) ? (object) null : fooIdx; // index == 1 => Invalid null value
						});

			var binder = new TestableExtensibleDefaultModelBinder
				{
				Binders = new ModelBinderDictionary
					{
					{ typeof(int), mockInnerBinder.Object }
				}
			};

			// Act
			binder.PublicUpdateCollection(null, bindingContext, typeof(int));

			// Assert
			Assert.Equal(3, model.Count);
			Assert.False(bindingContext.ModelState.IsValidField("foo[1]"));
			Assert.Equal("A value is required.", bindingContext.ModelState["foo[1]"].Errors[0].ErrorMessage);
			Assert.Equal(0, model[0]);
			Assert.Equal(0, model[1]);
			Assert.Equal(2, model[2]);
		}

		[Fact]
		public void UpdateCollectionReturnsModifiedCollectionOnSuccess_ExplicitIndex()
		{
			// Arrange
			ControllerContext controllerContext = new Mock<ControllerContext>().Object;

			var model = new List<int> { 4, 5, 6, 7, 8 };
			var bindingContext = new ModelBindingContext
			{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
				ModelName = "foo",
				ValueProvider = new SimpleValueProvider
					{
					{ "foo.index", new [] { "alpha", "bravo", "charlie" } }, // 'bravo' will be skipped
					{ "foo[alpha]", "10" },
					{ "foo[charlie]", "30" }
				}
			};

			var binder = new TestableExtensibleDefaultModelBinder();

			// Act
			object updatedModel = binder.PublicUpdateCollection(controllerContext, bindingContext, typeof(int));

			// Assert
			Assert.Same(model, updatedModel);
			Assert.Equal(2, model.Count);
			Assert.Equal(10, model[0]);
			Assert.Equal(30, model[1]);
		}

		[Fact]
		public void UpdateCollectionReturnsModifiedCollectionOnSuccess_ZeroBased()
		{
			// Arrange
			ControllerContext controllerContext = new Mock<ControllerContext>().Object;

			var model = new List<int> { 4, 5, 6, 7, 8 };
			var bindingContext = new ModelBindingContext
			{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
				ModelName = "foo",
				ValueProvider = new SimpleValueProvider
					{
					{ "foo[0]", null },
					{ "foo[1]", null },
					{ "foo[2]", null }
				}
			};

			var mockInnerBinder = new Mock<IModelBinder>();
			mockInnerBinder
				.Setup(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
				.Returns(
					(ControllerContext cc, ModelBindingContext bc) =>
						{
							Assert.Equal(controllerContext, cc);
							Assert.Equal(typeof (int), bc.ModelType);
							Assert.Equal(bindingContext.ModelState, bc.ModelState);
							Assert.Equal(bindingContext.PropertyFilter, bc.PropertyFilter);
							Assert.Equal(bindingContext.ValueProvider, bc.ValueProvider);
							return Int32.Parse(bc.ModelName.Substring(4, 1), CultureInfo.InvariantCulture);
						});

			var binder = new TestableExtensibleDefaultModelBinder
				{
				Binders = new ModelBinderDictionary
					{
					{ typeof(int), mockInnerBinder.Object }
				}
			};

			// Act
			object updatedModel = binder.PublicUpdateCollection(controllerContext, bindingContext, typeof(int));

			// Assert
			Assert.Same(model, updatedModel);
			Assert.Equal(3, model.Count);
			Assert.Equal(0, model[0]);
			Assert.Equal(1, model[1]);
			Assert.Equal(2, model[2]);
		}

		[Fact]
		public void UpdateCollectionReturnsNullIfZeroIndexNotFound()
		{
			// Arrange
			var bindingContext = new ModelBindingContext
			{
				ValueProvider = new SimpleValueProvider()
			};
			var binder = new TestableExtensibleDefaultModelBinder();

			// Act
			object updatedModel = binder.PublicUpdateCollection(null, bindingContext, typeof(object));

			// Assert
			Assert.Null(updatedModel);
		}

		[Fact]
		public void UpdateDictionaryCreatesDefaultEntriesForInvalidValues()
		{
			// Arrange
			var model = new Dictionary<string, int>
			{
				{ "one", 1 },
				{ "two", 2 }
			};
			var bindingContext = new ModelBindingContext
			{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
				ModelName = "foo",
				ValueProvider = new SimpleValueProvider
				{
					{ "foo[0].key", null }, { "foo[0].value", null },
					{ "foo[1].key", null }, { "foo[1].value", null },
					{ "foo[2].key", null }, { "foo[2].value", null }
				}
			};

			var mockStringBinder = new Mock<IModelBinder>();
			mockStringBinder
				.Setup(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
				.Returns(
					(ControllerContext cc, ModelBindingContext bc) =>
					(Int32.Parse(bc.ModelName.Substring(4, 1), CultureInfo.InvariantCulture) + 10) + "Value");

			var mockIntBinder = new Mock<IModelBinder>();
			mockIntBinder
				.Setup(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
				.Returns(
					delegate(ControllerContext cc, ModelBindingContext bc)
					{
						int fooIdx = Int32.Parse(bc.ModelName.Substring(4, 1), CultureInfo.InvariantCulture);
						return (fooIdx == 1) ? (object)null : fooIdx;
					});

			var binder = new TestableExtensibleDefaultModelBinder
				{
				Binders = new ModelBinderDictionary
				{
					{ typeof(string), mockStringBinder.Object },
					{ typeof(int), mockIntBinder.Object }
				}
			};

			// Act
			binder.PublicUpdateDictionary(null, bindingContext, typeof(string), typeof(int));

			// Assert
			Assert.Equal(3, model.Count);
			Assert.False(bindingContext.ModelState.IsValidField("foo[1].value"));
			Assert.Equal("A value is required.", bindingContext.ModelState["foo[1].value"].Errors[0].ErrorMessage);
			Assert.Equal(0, model["10Value"]);
			Assert.Equal(0, model["11Value"]);
			Assert.Equal(2, model["12Value"]);
		}

		[Fact]
		public void UpdateDictionaryReturnsModifiedDictionaryOnSuccess_ExplicitIndex()
		{
			// Arrange
			ControllerContext controllerContext = new Mock<ControllerContext>().Object;

			var model = new Dictionary<int, string>
			{
				{ 1, "one" },
				{ 2, "two" }
			};
			var bindingContext = new ModelBindingContext
			{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
				ModelName = "foo",
				PropertyFilter = _ => false,
				ValueProvider = new SimpleValueProvider
					{
					{ "foo.index", new [] { "alpha", "bravo", "charlie" } }, // 'bravo' will be skipped
					{ "foo[alpha].key", "10" }, { "foo[alpha].value", "ten" },
					{ "foo[charlie].key", "30" }, { "foo[charlie].value", "thirty" }
				}
			};

			var binder = new TestableExtensibleDefaultModelBinder();

			// Act
			object updatedModel = binder.PublicUpdateDictionary(controllerContext, bindingContext, typeof(int), typeof(string));

			// Assert
			Assert.Same(model, updatedModel);
			Assert.Equal(2, model.Count);
			Assert.Equal("ten", model[10]);
			Assert.Equal("thirty", model[30]);
		}

		[Fact]
		public void UpdateDictionaryReturnsModifiedDictionaryOnSuccess_ZeroBased()
		{
			// Arrange
			ControllerContext controllerContext = new Mock<ControllerContext>().Object;

			var model = new Dictionary<int, string>
			{
				{ 1, "one" },
				{ 2, "two" }
			};
			var bindingContext = new ModelBindingContext
			{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
				ModelName = "foo",
				PropertyFilter = _ => false,
				ValueProvider = new SimpleValueProvider
				{
					{ "foo[0].key", null }, { "foo[0].value", null },
					{ "foo[1].key", null }, { "foo[1].value", null },
					{ "foo[2].key", null }, { "foo[2].value", null }
				}
			};

			var mockIntBinder = new Mock<IModelBinder>();
			mockIntBinder
				.Setup(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
				.Returns(
					(ControllerContext cc, ModelBindingContext bc) =>
						{
							Assert.Equal(controllerContext, cc);
							Assert.Equal(typeof (int), bc.ModelType);
							Assert.Equal(bindingContext.ModelState, bc.ModelState);
							Assert.Equal(new ModelBindingContext().PropertyFilter, bc.PropertyFilter);
							Assert.Equal(bindingContext.ValueProvider, bc.ValueProvider);
							return Int32.Parse(bc.ModelName.Substring(4, 1), CultureInfo.InvariantCulture) + 10;
						});

			var mockStringBinder = new Mock<IModelBinder>();
			mockStringBinder
				.Setup(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
				.Returns(
					(ControllerContext cc, ModelBindingContext bc) =>
						{
							Assert.Equal(controllerContext, cc);
							Assert.Equal(typeof (string), bc.ModelType);
							Assert.Equal(bindingContext.ModelState, bc.ModelState);
							Assert.Equal(bindingContext.PropertyFilter, bc.PropertyFilter);
							Assert.Equal(bindingContext.ValueProvider, bc.ValueProvider);
							return (Int32.Parse(bc.ModelName.Substring(4, 1), CultureInfo.InvariantCulture) + 10) + "Value";
						});

			var binder = new TestableExtensibleDefaultModelBinder
			{
				Binders = new ModelBinderDictionary
					{
					{ typeof(int), mockIntBinder.Object },
					{ typeof(string), mockStringBinder.Object }
				}
			};

			// Act
			object updatedModel = binder.PublicUpdateDictionary(controllerContext, bindingContext, typeof(int), typeof(string));

			// Assert
			Assert.Same(model, updatedModel);
			Assert.Equal(3, model.Count);
			Assert.Equal("10Value", model[10]);
			Assert.Equal("11Value", model[11]);
			Assert.Equal("12Value", model[12]);
		}

		[Fact]
		public void UpdateDictionaryReturnsNullIfNoValidElementsFound()
		{
			// Arrange
			var bindingContext = new ModelBindingContext
				{
				ValueProvider = new SimpleValueProvider()
			};
			var binder = new TestableExtensibleDefaultModelBinder();

			// Act
			object updatedModel = binder.PublicUpdateDictionary(null, bindingContext, typeof(object), typeof(object));

			// Assert
			Assert.Null(updatedModel);
		}

		[Fact]
		public void UpdateDictionarySkipsInvalidKeys()
		{
			// Arrange
			var model = new Dictionary<int, string>
			{
				{ 1, "one" },
				{ 2, "two" }
			};
			var bindingContext = new ModelBindingContext
				{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
				ModelName = "foo",
				ValueProvider = new SimpleValueProvider
					{
					{ "foo[0].key", null }, { "foo[0].value", null },
					{ "foo[1].key", null }, { "foo[1].value", null },
					{ "foo[2].key", null }, { "foo[2].value", null }
				}
			};

			var mockIntBinder = new Mock<IModelBinder>();
			mockIntBinder
				.Setup(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
				.Returns(
					(ControllerContext cc, ModelBindingContext bc) =>
						{
							int fooIdx = Int32.Parse(bc.ModelName.Substring(4, 1), CultureInfo.InvariantCulture);
							return (fooIdx == 1) ? (object) null : fooIdx;
						});

			var mockStringBinder = new Mock<IModelBinder>();
			mockStringBinder
				.Setup(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
				.Returns(
					(ControllerContext cc, ModelBindingContext bc) =>
					(Int32.Parse(bc.ModelName.Substring(4, 1), CultureInfo.InvariantCulture) + 10) + "Value");

			var binder = new TestableExtensibleDefaultModelBinder
			{
				Binders = new ModelBinderDictionary
				{
					{ typeof(int), mockIntBinder.Object },
					{ typeof(string), mockStringBinder.Object }
				}
			};

			// Act
			binder.PublicUpdateDictionary(null, bindingContext, typeof(int), typeof(string));

			// Assert
			Assert.Equal(2, model.Count);
			Assert.False(bindingContext.ModelState.IsValidField("foo[1].key"));
			Assert.Equal("A value is required.", bindingContext.ModelState["foo[1].key"].Errors[0].ErrorMessage);
			Assert.Equal("10Value", model[0]);
			Assert.Equal("12Value", model[2]);
		}

		[ModelBinder(typeof(DefaultModelBinder))]
		private class MyModel
		{
			public int ReadOnlyProperty
			{
				get { return 4; }
			}

			public int ReadWriteProperty { get; set; }
			public int ReadWriteProperty2 { get; set; }
		}

		class MyModel2
		{
			private int _intReadWriteNonNegative;

			public int IntReadOnly
			{
				get { return 4; }
			}

			public int IntReadWrite { get; set; }

			public int IntReadWriteNonNegative
			{
				get { return _intReadWriteNonNegative; }
				set
				{
					if (value < 0)
					{
						throw new ArgumentOutOfRangeException(nameof(value), "Value must be non-negative.");
					}
					_intReadWriteNonNegative = value;
				}
			}

			public int? NullableIntReadWrite { get; set; }
		}

		class CountryState
		{
			public string Name { get; set; }

			public IEnumerable<string> States { get; set; }
		}

		//private class MyClassWithoutConverter
		//{
		//}

		//[Bind(Exclude = "Alpha,Echo")]
		//private class MyOtherModel
		//{
		//	public string Alpha { get; set; }
		//	public string Bravo { get; set; }
		//	public string Charlie { get; set; }
		//	public string Delta { get; set; }
		//	public string Echo { get; set; }
		//	public string Foxtrot { get; set; }
		//}

		class Customer
		{
			private readonly Address _address = new Address();
			public Address Address
			{
				get { return _address; }
			}
		}

		class Address
		{
			public string Street { get; set; }
			public string Zip { get; set; }
		}

		//public class IntegerContainer
		//{
		//	public int Integer { get; set; }
		//	public int? NullableInteger { get; set; }
		//}

		//[TypeConverter(typeof(CultureAwareConverter))]
		//public class StringContainer
		//{
		//	public StringContainer(string value)
		//	{
		//		Value = value;
		//	}

		//	public string Value { get; }
		//}

		//private class CultureAwareConverter : TypeConverter
		//{
		//	public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		//	{
		//		return (sourceType == typeof(string));
		//	}

		//	public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		//	{
		//		return (destinationType == typeof(string));
		//	}

		//	public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		//	{
		//		string stringValue = value as string;
		//		if (stringValue == null || stringValue.Length < 3)
		//		{
		//			throw new Exception("Value must have at least 3 characters.");
		//		}
		//		return new StringContainer(AppendCultureName(stringValue, culture));
		//	}

		//	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		//	{
		//		StringContainer container = value as StringContainer;
		//		if (container.Value == null || container.Value.Length < 3)
		//		{
		//			throw new Exception("Value must have at least 3 characters.");
		//		}

		//		return AppendCultureName(container.Value, culture);
		//	}

		//	private static string AppendCultureName(string value, CultureInfo culture)
		//	{
		//		string cultureName = (!string.IsNullOrEmpty(culture.Name)) ? culture.Name : culture.ThreeLetterWindowsLanguageName;
		//		return value + " (" + cultureName + ")";
		//	}
		//}

		//[ModelBinder(typeof(MyStringModelBinder))]
		//private class MyStringModel
		//{
		//	public string Value { get; set; }
		//}

		//private class MyStringModelBinder : IModelBinder
		//{
		//	public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		//	{
		//		MyStringModel castModel = bindingContext.Model as MyStringModel;
		//		if (castModel != null)
		//		{
		//			castModel.Value += "_Update";
		//		}
		//		else
		//		{
		//			castModel = new MyStringModel() { Value = bindingContext.ModelName + "_Create" };
		//		}
		//		return castModel;
		//	}
		//}

		class CustomUnvalidatedValueProvider : IUnvalidatedValueProvider
		{
			public ValueProviderResult GetValue(string key, bool skipValidation)
			{
				string newValue = key + (skipValidation ? "Unvalidated" : "Validated");
				return new ValueProviderResult(newValue, newValue, null);
			}

			public bool ContainsPrefix(string prefix)
			{
				return true;
			}

			public ValueProviderResult GetValue(string key)
			{
				return GetValue(key, skipValidation: false);
			}
		}

		class SimpleController : Controller
		{
		}

		[Bind(Exclude = "Foo")]
		class ModelWithBindAttribute : ModelWithoutBindAttribute
		{
		}

		class ModelWithoutBindAttribute
		{
			public string Foo { get; set; }
			public string Bar { get; set; }
			public string Baz { get; set; }
		}

		class PropertyTestingModel
		{
			public string StringReadWrite { get; set; }
			public string StringReadOnly { get; }
			public int IntReadWrite { get; set; }
			public int IntReadOnly { get; }
			public object[] ArrayReadWrite { get; set; }
			public object[] ArrayReadOnly { get; }
			public Address AddressReadWrite { get; set; }
			public Address AddressReadOnly { get; }
			public string Whitelisted { get; set; }
			public string Blacklisted { get; set; }
		}

		//// --------------------------------------------------------------------------------
		////  DataAnnotations tests

		const string BASE_MODEL_NAME = "BaseModelName";

		//// GetModelProperties tests

		[MetadataType(typeof(Metadata))]
		class GetModelPropertiesModel
		{
			[Required]
			public int LocalAttributes { get; set; }

			public int MetadataAttributes { get; set; }

			[Required]
			public int MixedAttributes { get; set; }

			class Metadata
			{
				[Range(10, 100)]
				public int MetadataAttributes { get; set; }

				[Range(10, 100)]
				public int MixedAttributes { get; set; }
			}
		}

		[Fact]
		public void GetModelPropertiesWithLocalAttributes()
		{
			// Arrange
			var modelBinder = new TestableExtensibleDefaultModelBinder<GetModelPropertiesModel>();

			// Act
			PropertyDescriptor property = modelBinder.GetModelProperties()
			                                         .Cast<PropertyDescriptor>().Single(pd => pd.Name == "LocalAttributes");

			// Assert
			Assert.Contains(property.Attributes.Cast<Attribute>(), a => a is RequiredAttribute);
		}

		[Fact]
		public void GetModelPropertiesWithMetadataAttributesDoesntWork()
		{
			// Arrange
			var modelBinder = new TestableExtensibleDefaultModelBinder<GetModelPropertiesModel>();

			// Act
			PropertyDescriptor property = modelBinder.GetModelProperties()
			                                         .Cast<PropertyDescriptor>().Single(pd => pd.Name == "MetadataAttributes");

			// Assert
			Assert.DoesNotContain(property.Attributes.Cast<Attribute>(), a => a is RangeAttribute);
		}

		[Fact]
		public void GetModelPropertiesWithMixedAttributes()
		{
			// Arrange
			var modelBinder = new TestableExtensibleDefaultModelBinder<GetModelPropertiesModel>();

			// Act
			PropertyDescriptor property = modelBinder.GetModelProperties()
			                                         .Cast<PropertyDescriptor>().Single(pd => pd.Name == "MixedAttributes");

			// Assert
			Assert.Contains(property.Attributes.Cast<Attribute>(), a => a is RequiredAttribute);
			Assert.DoesNotContain(property.Attributes.Cast<Attribute>(), a => a is RangeAttribute);
		}

		//// GetPropertyValue tests

		class GetPropertyValueModel
		{
			public string NoAttribute { get; set; }

			[DisplayFormat(ConvertEmptyStringToNull = false)]
			public string AttributeWithoutConversion { get; set; }

			[DisplayFormat(ConvertEmptyStringToNull = true)]
			public string AttributeWithConversion { get; set; }
		}

		[Fact]
		public void GetPropertyValueWithNoAttributeConvertsEmptyStringToNull()
		{
			// Arrange
			var binder = new TestableExtensibleDefaultModelBinder<GetPropertyValueModel>();
			binder.Context.ModelMetadata = binder.Context.PropertyMetadata["NoAttribute"];

			// Act
			object result = binder.GetPropertyValue("NoAttribute", string.Empty);

			// Assert
			Assert.Null(result);
		}

		[Fact]
		public void GetPropertyValueWithFalseAttributeDoesNotConvertEmptyStringToNull()
		{
			// Arrange
			var binder = new TestableExtensibleDefaultModelBinder<GetPropertyValueModel>();
			binder.Context.ModelMetadata = binder.Context.PropertyMetadata["AttributeWithoutConversion"];

			// Act
			object result = binder.GetPropertyValue("AttributeWithoutConversion", string.Empty);

			// Assert
			Assert.Equal(string.Empty, result);
		}

		[Fact]
		public void GetPropertyValueWithTrueAttributeConvertsEmptyStringToNull()
		{
			// Arrange
			var binder = new TestableExtensibleDefaultModelBinder<GetPropertyValueModel>();
			binder.Context.ModelMetadata = binder.Context.PropertyMetadata["AttributeWithConversion"];

			// Act
			object result = binder.GetPropertyValue("AttributeWithConversion", string.Empty);

			// Assert
			Assert.Null(result);
		}

		//// OnModelUpdated tests

		[Fact]
		public void OnModelUpdatedPassesNullContainerToValidate()
		{
			Mock<ModelValidatorProvider> provider = null;

			try
			{
				// Arrange
				ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(object));
				var controllerContext = new ControllerContext();
				var mockModelValidator = new Mock<ModelValidator>(metadata, controllerContext);
				provider = new Mock<ModelValidatorProvider>();
				provider
					.Setup(p => p.GetValidators(It.IsAny<ModelMetadata>(), It.IsAny<ControllerContext>()))
					.Returns(new [] { mockModelValidator.Object });
				ModelValidatorProviders.Providers.Add(provider.Object);
				var model = new object();
				var modelBinder = new TestableExtensibleDefaultModelBinder<object>(model);

				// Act
				modelBinder.OnModelUpdated();

				// Assert
				mockModelValidator.Verify(v => v.Validate(null));
			}
			finally
			{
				if (provider != null)
				{
					ModelValidatorProviders.Providers.Remove(provider.Object);
				}
			}
		}

		public class MinMaxValidationAttribute : ValidationAttribute
		{
			public MinMaxValidationAttribute()
				: base("Minimum must be less than or equal to Maximum")
			{
			}

			public override bool IsValid(object value)
			{
				var model = (OnModelUpdatedModelMultipleParameters)value;
				return model.Minimum <= model.Maximum;
			}
		}

		[MinMaxValidation]
		public class OnModelUpdatedModelMultipleParameters
		{
			public int Minimum { get; set; }
			public int Maximum { get; set; }
		}

		[Fact]
		public void OnModelUpdatedWithValidationAttributeMultipleParameters()
		{
			// Arrange
			var model = new OnModelUpdatedModelMultipleParameters { Minimum = 250, Maximum = 100 };
			var modelBinder = new TestableExtensibleDefaultModelBinder<OnModelUpdatedModelMultipleParameters>(model);

			// Act
			modelBinder.OnModelUpdated();

			// Assert
			Assert.Single(modelBinder.ModelState);
			ModelState stateModel = modelBinder.ModelState[BASE_MODEL_NAME];
			Assert.NotNull(stateModel);
			Assert.Equal("Minimum must be less than or equal to Maximum", stateModel.Errors.Single().ErrorMessage);
		}

		[Fact]
		public void OnModelUpdatedWithInvalidPropertyValidationWillNotRunEntityLevelValidation()
		{
			// Arrange
			var model = new OnModelUpdatedModelMultipleParameters { Minimum = 250, Maximum = 100 };
			var modelBinder = new TestableExtensibleDefaultModelBinder<OnModelUpdatedModelMultipleParameters>(model);
			modelBinder.ModelState.AddModelError(BASE_MODEL_NAME + ".Minimum", "The minimum value was invalid.");

			// Act
			modelBinder.OnModelUpdated();

			// Assert
			Assert.Null(modelBinder.ModelState[BASE_MODEL_NAME]);
		}

		public class AlwaysInvalidAttribute : ValidationAttribute
		{
			public AlwaysInvalidAttribute()
			{
			}

			public AlwaysInvalidAttribute(string message)
				: base(message)
			{
			}

			public override bool IsValid(object value)
			{
				return false;
			}
		}

		[AlwaysInvalid("The object just isn't right")]
		public class OnModelUpdatedModelNoParameters
		{
		}

		[Fact]
		public void OnModelUpdatedWithValidationAttributeNoParameters()
		{
			// Arrange
			var modelBinder = new TestableExtensibleDefaultModelBinder<OnModelUpdatedModelNoParameters>();

			// Act
			modelBinder.OnModelUpdated();

			// Assert
			Assert.Single(modelBinder.ModelState);
			ModelState stateModel = modelBinder.ModelState[BASE_MODEL_NAME];
			Assert.NotNull(stateModel);
			Assert.Equal("The object just isn't right", stateModel.Errors.Single().ErrorMessage);
		}

		[AlwaysInvalid]
		public class OnModelUpdatedModelNoValidationResult
		{
		}

		[Fact]
		public void OnModelUpdatedWithValidationAttributeNoValidationMessage()
		{
            using (new CurrentCultureScope("en-US"))
            {
                // Arrange
                var modelBinder = new TestableExtensibleDefaultModelBinder<OnModelUpdatedModelNoValidationResult>();

                // Act
                modelBinder.OnModelUpdated();

                // Assert
                Assert.Single(modelBinder.ModelState);
                ModelState stateModel = modelBinder.ModelState[BASE_MODEL_NAME];
                Assert.NotNull(stateModel);
                Assert.Equal("The field OnModelUpdatedModelNoValidationResult is invalid.",
                    stateModel.Errors.Single().ErrorMessage);
            }
        }

		[Fact]
		public void OnModelUpdatedDoesNotPlaceErrorMessagesInModelStateWhenSubPropertiesHaveErrors()
		{
			// Arrange
			var modelBinder = new TestableExtensibleDefaultModelBinder<OnModelUpdatedModelNoValidationResult>();
			modelBinder.ModelState.AddModelError("Foo.Bar", "Foo.Bar is invalid");
			modelBinder.Context.ModelName = "Foo";

			// Act
			modelBinder.OnModelUpdated();

			// Assert
			Assert.Null(modelBinder.ModelState["Foo"]);
		}

		//// OnPropertyValidating tests

		class OnPropertyValidatingModel
		{
			public string NotValidated { get; set; }

			[Range(10, 65)]
			public int RangedInteger { get; set; }

			[Required(ErrorMessage = "Custom Required Message")]
			public int RequiredInteger { get; set; }
		}

		[Fact]
		public void OnPropertyValidatingWithoutValidationAttribute()
		{
			// Arrange
			var modelBinder = new TestableExtensibleDefaultModelBinder<OnPropertyValidatingModel>();

			// Act
			modelBinder.OnPropertyValidating("NotValidated", 42);

			// Assert
			Assert.Empty(modelBinder.ModelState);
		}

		[Fact]
		public void OnPropertyValidatingWithValidationAttributePassing()
		{
			// Arrange
			var modelBinder = new TestableExtensibleDefaultModelBinder<OnPropertyValidatingModel>();
			modelBinder.Context.PropertyMetadata["RangedInteger"].Model = 42;

			// Act
			bool result = modelBinder.OnPropertyValidating("RangedInteger", 42);

			// Assert
			Assert.True(result);
			Assert.Empty(modelBinder.ModelState);
		}

		//// SetProperty tests

		[Fact]
		public void SetPropertyWithRequiredOnValueTypeOnlyResultsInSingleMessage()
		{
			// Arrange
			var modelBinder = new TestableExtensibleDefaultModelBinder<OnPropertyValidatingModel>();
			modelBinder.Context.ModelMetadata.Model = new OnPropertyValidatingModel();

			// Act
			modelBinder.SetProperty("RequiredInteger", null);

			// Assert
			ModelState modelState = modelBinder.ModelState[BASE_MODEL_NAME + ".RequiredInteger"];
			ModelError modelStateError = modelState.Errors.Single();
			Assert.Equal("Custom Required Message", modelStateError.ErrorMessage);
		}

		[Fact]
		public void SetPropertyAddsDefaultMessageForNonBindableNonNullableValueTypes()
		{
			DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;

			try
			{
				// Arrange
				var modelBinder = new TestableExtensibleDefaultModelBinder<List<string>>();
				modelBinder.Context.ModelMetadata.Model = null;

				// Act
				modelBinder.SetProperty("Count", null);

				// Assert
				ModelState modelState = modelBinder.ModelState[BASE_MODEL_NAME + ".Count"];
				ModelError modelStateError = modelState.Errors.Single();
				Assert.Equal("A value is required.", modelStateError.ErrorMessage);
			}
			finally
			{
				DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = true;
			}
		}

		[Fact]
		public void SetPropertyCreatesValueRequiredErrorIfNecessary()
		{
            using (new CurrentCultureScope("en-US"))
            {
                // Arrange
                ModelBindingContext bindingContext = new ModelBindingContext()
                {
                    ModelMetadata =
                        ModelMetadataProviders.Current.GetMetadataForType(() => new MyModel(), typeof(MyModel)),
                    ModelName = "theModel",
                };

                PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(MyModel))["ReadWriteProperty"];
                var helper = new TestableExtensibleDefaultModelBinder();

                // Act
                helper.PublicSetProperty(new ControllerContext(), bindingContext, property, null);

                // Assert
                Assert.Equal("The ReadWriteProperty field is required.",
                    bindingContext.ModelState["theModel.ReadWriteProperty"].Errors[0].ErrorMessage);
            }
        }

		[Fact]
		public void SetPropertyWithThrowingSetter()
		{
			// Arrange
			var binder = new TestableExtensibleDefaultModelBinder<SetPropertyModel>();

			// Act
			binder.SetProperty("NonNullableString", null);

			// Assert
			ModelState modelState = binder.Context.ModelState[BASE_MODEL_NAME + ".NonNullableString"];
			Assert.Single(modelState.Errors);
			Assert.IsType<ArgumentNullException>(modelState.Errors[0].Exception);
		}

		[Fact]
		public void SetPropertyWithNullValueAndThrowingSetterWithRequiredAttribute()
		{
			// Arrange
			var binder = new TestableExtensibleDefaultModelBinder<SetPropertyModel>();

			// Act
			binder.SetProperty("NonNullableStringWithAttribute", null);

			// Assert
			ModelState modelState = binder.Context.ModelState[BASE_MODEL_NAME + ".NonNullableStringWithAttribute"];
			var error = Assert.Single(modelState.Errors);
			Assert.Equal("My custom required message", error?.ErrorMessage);
		}

		[Fact]
		public void SetPropertyDoesNothingIfPropertyIsReadOnly()
		{
			// Arrange
			var model = new MyModel();
			var bindingContext = new ModelBindingContext
			{
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
				ModelName = "theModel"
			};

			PropertyDescriptor property = TypeDescriptor.GetProperties(model)["ReadOnlyProperty"];
			var helper = new TestableExtensibleDefaultModelBinder();

			// Act
			helper.PublicSetProperty(new ControllerContext(), bindingContext, property, 42);

			// Assert
			Assert.Empty(bindingContext.ModelState);
		}

		[Fact]
		public void SetPropertySuccess()
		{
			// Arrange
			var binder = new TestableExtensibleDefaultModelBinder<SetPropertyModel>();

			// Act
			binder.SetProperty("NullableString", "The new value");

			// Assert
			Assert.Equal("The new value", ((SetPropertyModel)binder.Context.Model).NullableString);
			Assert.Empty(binder.Context.ModelState);
		}

		class SetPropertyModel
		{
			public string NullableString { get; set; }

			public string NonNullableString
			{
				get { return null; }
				set
				{
					if (value == null)
					{
						throw new ArgumentNullException(nameof(value));
					}
				}
			}

			[Required(ErrorMessage = "My custom required message")]
			public string NonNullableStringWithAttribute
			{
				get { return null; }
				set
				{
					if (value == null)
					{
						throw new ArgumentNullException(nameof(value));
					}
				}
			}
		}

		//// Helper classes

		class TestableExtensibleDefaultModelBinder<TModel> : ExtensibleDefaultModelBinder
			where TModel : new()
		{
			public TestableExtensibleDefaultModelBinder()
				: this(new TModel())
			{
			}

			public TestableExtensibleDefaultModelBinder(TModel model)
			{
				ModelState = new ModelStateDictionary();

				Context = new ModelBindingContext
					{
						ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, typeof (TModel)),
						ModelName = BASE_MODEL_NAME,
						ModelState = ModelState
					};
			}

			public ModelBindingContext Context { get; }

			public ModelStateDictionary ModelState { get; set; }

			public void BindProperty(string propertyName)
			{
				base.BindProperty(new ControllerContext(), Context, GetProperty<TModel>(propertyName));
			}

			public PropertyDescriptorCollection GetModelProperties()
			{
				return base.GetModelProperties(new ControllerContext(), Context);
			}

			public object GetPropertyValue(string propertyName, object existingValue)
			{
				var mockPropertyBinder = new Mock<IModelBinder>();
				mockPropertyBinder.Setup(b => b.BindModel(It.IsAny<ControllerContext>(), Context)).Returns(existingValue);
				return base.GetPropertyValue(new ControllerContext(), Context, GetProperty<TModel>(propertyName), mockPropertyBinder.Object);
			}

			public bool OnPropertyValidating(string propertyName, object value)
			{
				return base.OnPropertyValidating(new ControllerContext(), Context, GetProperty<TModel>(propertyName), value);
			}

			public void OnPropertyValidated(string propertyName, object value)
			{
				base.OnPropertyValidated(new ControllerContext(), Context, GetProperty<TModel>(propertyName), value);
			}

			public void OnModelUpdated()
			{
				base.OnModelUpdated(new ControllerContext(), Context);
			}

			public void SetProperty(string propertyName, object value)
			{
				base.SetProperty(new ControllerContext(), Context, GetProperty<TModel>(propertyName), value);
			}

			static PropertyDescriptor GetProperty<T>(string propertyName)
			{
				return TypeDescriptor.GetProperties(typeof(T))
				                     .Cast<PropertyDescriptor>().Single(p => p.Name == propertyName);
			}
		}
	}
}
