using System;
using System.Linq.Expressions;
using Castle.Components.Validator;
using Maxfire.Castle.Validation.Validators;
using Maxfire.Core.Reflection;
using Maxfire.TestCommons;

namespace Maxfire.Castle.Validation.UnitTests
{
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
}