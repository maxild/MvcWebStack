using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Web.Mvc;
using Castle.Components.Validator;
using Maxfire.Core.Reflection;
using Rhino.Mocks;

namespace Maxfire.Web.Mvc.UnitTests
{
	public class TestableValidationModelBinder<TModel> : ValidationModelBinder where TModel : class
	{
		public TestableValidationModelBinder() 
			: this(null)
		{
		}

		public TestableValidationModelBinder(TModel model)
		{
			RequestParams = new NameValueCollection();
			ModelState = new ModelStateDictionary();
			_context = new ModelBindingContext
						{
							ModelMetadata = TestableMetadataProvider.Instance.GetMetadataForType(() => model, typeof(TModel)),
							ModelState = ModelState,
							FallbackToEmptyPrefix = true
						};
			// Register this as the default binder without any side-effects (i.e. without using the Binders registry)
			Binders = new ModelBinderDictionary { DefaultBinder = this };
		}

		public NameValueCollection RequestParams { get; set; }

		private ModelBindingContext _context;
		public ModelBindingContext Context 
		{
			get
			{
				if (_context != null)
				{
					// Note: Because the ValueProvider property can never be null (see the getter in ModelBindingContext) we have to initialize to an empty params collection when RequestParams is null.
					_context.ValueProvider = new FormCollection(RequestParams ?? new NameValueCollection()).ToValueProvider();
				}
				return _context;
			}
			set
			{
				_context = value;
			}
		} 

		public ModelStateDictionary ModelState { get; set; }

		public bool IsInputValid
		{
			get { return ModelState.IsValid; }
		}

		public string Prefix
		{
			get 
			{ 
				if (_context == null)
				{
					return null;
				}
				return _context.ModelName;
			}
			set
			{
				if (_context != null)
				{
					_context.ModelName = value;
				}
			}
		}

		public TModel BindModel()
		{
			return (TModel)base.BindModel(new ControllerContext(), Context);
		}

		public void BindProperty(Expression<Func<TModel, object>> expression)
		{
			base.BindProperty(new ControllerContext(), Context, ReflectionHelper.GetProperty(expression));
		}

		public void BindProperty(string propertyName)
		{
			base.BindProperty(new ControllerContext(), Context, ReflectionHelper.GetProperty<TModel>(propertyName));
		}

		public PropertyDescriptorCollection GetModelProperties()
		{
			return base.GetModelProperties(new ControllerContext(), Context);
		}

		public object GetPropertyValue(string propertyName, object existingValue)
		{
			var mockModelBinder = MockRepository.GenerateStub<IModelBinder>();
			mockModelBinder.Stub(b => b.BindModel(Arg<ControllerContext>.Is.Anything, Context)).Return(existingValue);
			return base.GetPropertyValue(new ControllerContext(), Context, ReflectionHelper.GetProperty<TModel>(propertyName), mockModelBinder);
		}

		public void OnModelUpdated()
		{
			base.OnModelUpdated(new ControllerContext(), Context);
		}

		public bool OnPropertyValidating(Expression<Func<TModel, object>> expression, object value)
		{
			return OnPropertyValidating(new ControllerContext(), Context, ReflectionHelper.GetProperty(expression), value);
		}

		public bool OnPropertyValidating(string propertyName, object value)
		{
			return base.OnPropertyValidating(new ControllerContext(), Context, ReflectionHelper.GetProperty<TModel>(propertyName), value);
		}

		class TestableModelMetadata : ModelMetadata
		{
			public TestableModelMetadata(ModelMetadataProvider provider, Type containerType, Func<object> modelAccessor, Type modelType, string propertyName)
				: base(provider, containerType, modelAccessor, modelType, propertyName)
			{
			}

			public override IEnumerable<ModelValidator> GetValidators(ControllerContext context)
			{
				return TestableModelValidatorProvider.Instance.GetValidators(this, context);
			}
		}

		class TestableMetadataProvider : OpinionatedMetadataProvider
		{
			private static TestableMetadataProvider _singleton;

			private TestableMetadataProvider()
			{
			}

			protected override ModelMetadata CreateMetadata(IEnumerable<Attribute> attributes, Type containerType, Func<object> modelAccessor, Type modelType, string propertyName)
			{
				return new TestableModelMetadata(this, containerType, modelAccessor, modelType, propertyName);
			}

			public static TestableMetadataProvider Instance
			{
				get
				{
					if (_singleton == null)
					{
						_singleton = new TestableMetadataProvider();
					}
					return _singleton;
				}
			}
		}

		class TestableModelValidatorProvider : ModelValidatorProvider
		{
			private static TestableModelValidatorProvider _singleton;

			private TestableModelValidatorProvider()
			{
			}

			public override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, ControllerContext context)
			{
				yield return new CastleModelValidator(metadata, context, new ValidatorRunner(new CachedValidationRegistry()));
			}

			public static ModelValidatorProvider Instance
			{
				get
				{
					if (_singleton == null)
					{
						_singleton = new TestableModelValidatorProvider();
					}
					return _singleton;
				}
			}
		}

		public bool IsInputValidFor(string propertyName)
		{
			return ModelState.IsValidField(propertyName);
		}

		public bool IsInputValidFor(Expression<Func<TModel, object>> expression)
		{
			string propertyName = expression.GetNameFor();
			return IsInputValidFor(propertyName);
		}

		public ModelState GetModelStateFor(string propertyName)
		{
			return ModelState[propertyName];
		}

		public ModelState GetModelStateFor(Expression<Func<TModel, object>> expression)
		{
			string propertyName = expression.GetNameFor();
			return GetModelStateFor(propertyName);
		}

		public ModelErrorCollection GetModelErrorsFor(string propertyName)
		{
			return GetModelStateFor(propertyName).Errors;
		}

		public ModelErrorCollection GetModelErrorsFor(Expression<Func<TModel, object>> expression)
		{
			return GetModelStateFor(expression).Errors;
		}
	}
}