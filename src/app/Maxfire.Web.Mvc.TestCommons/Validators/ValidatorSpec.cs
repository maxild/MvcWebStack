using System;
using System.Linq.Expressions;
using Castle.Components.Validator;
using Maxfire.Core.Reflection;
using Maxfire.TestCommons;
using Maxfire.Web.Mvc.Validators;
using Xunit;

namespace Maxfire.Web.Mvc.TestCommons.Validators
{
	// Todo: registry through fixture
	public abstract class ValidatorConstructor : XunitSpec
	{
		protected IValidatorRunner _runner;
		protected IValidatorRegistry _registry;
		
		protected override void Establish_context()
		{
			_registry = new CachedValidationRegistry();
			_runner = new ValidatorRunner(_registry);
		}

		/// <summary>
		/// This method replicated what is done inside the CachedValidationRegistry.GetValidators() method.
		/// </summary>
		protected BaseValidator CreateValidatorFor<TModel>(Expression<Func<TModel, object>> propertyExpression)
		{
			var propertyInfo = ExpressionHelper.GetProperty(propertyExpression);

			if (propertyInfo.HasSingleCustomAttribute<BaseValidationAttribute>(true) == false)
			{
				throw new InvalidOperationException("The property should have exactly one validation attribute.");
			}

			var validationAttribute = propertyInfo.GetCustomAttribute<BaseValidationAttribute>();
			validationAttribute.Initialize(_registry, propertyInfo);

			var validator = validationAttribute.Build(_runner, typeof (TModel));
			validator.Initialize(_registry, propertyInfo);
			
			return validator as BaseValidator;
		}
	}

	// Todo: Use fixture for registry
	public abstract class ValidatorSpec : ValidatorConstructor
	{
		protected PropertyValidator<TModel> Validating<TModel>(TModel model)
			where TModel : class
		{
			
			return new PropertyValidator<TModel>(this, model);
		}

		public class PropertyValidator<TModel>
			where TModel : class
		{
			private readonly ValidatorSpec _spec;
			private readonly TModel _model;
			
			public PropertyValidator(ValidatorSpec spec, TModel model)
			{
				_spec = spec;
				_model = model;
			}

			public PropertyValidatorResult OnProperty(Expression<Func<TModel, object>> propertyExpression)
			{
				var validator = _spec.CreateValidatorFor(propertyExpression);

				var validationResult = validator.IsValid(_model, propertyExpression.GetValueFrom(_model));
				
				var validationErrorMessage = "";
				if (validationResult == false)
				{
					validationErrorMessage = validator.FormatErrorMessage(propertyExpression.GetDisplayName());
				}
				
				return new PropertyValidatorResult(validationResult, validationErrorMessage);
			}
		}

		public class PropertyValidatorResult
		{
			private readonly bool _result;
			private readonly string _errorMessage;

			public PropertyValidatorResult(bool validationResult, string errorMessage)
			{
				_result = validationResult;
				_errorMessage = errorMessage;
			}

			public void ShouldReturnTrue()
			{
				Assert.True(_result);
			}

			public PropertyValidatorErrorMessage ShouldReturnFalse()
			{
				Assert.False(_result);
				return new PropertyValidatorErrorMessage(_errorMessage);
			}
		}

		public class PropertyValidatorErrorMessage
		{
			private readonly string _errorMessage;

			public PropertyValidatorErrorMessage(string errorMessage)
			{
				_errorMessage = errorMessage;
			}

			public void WithErrorMessage(string expectedErrorMessage)
			{
				Assert.Equal(expectedErrorMessage, _errorMessage);
			}
		}
	}
}