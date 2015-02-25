using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using Maxfire.Core.Reflection;
using Xunit.Sdk;

namespace Maxfire.Web.Mvc.TestCommons.AssertExtensions
{
	public static class ModelStateAssertExtensions
	{
		public static void ShouldContainNumberOfInvalidFields(this ModelStateDictionary modelStateDictionary, int expectedNumberOfInvalidFields)
		{
			int actualNumberOfInvalidFields = 0;
			foreach (var modelState in modelStateDictionary.Values)
			{
				if (modelState.Errors != null && modelState.Errors.Count > 0)
				{
					actualNumberOfInvalidFields++;
				}
			}

			if (actualNumberOfInvalidFields != expectedNumberOfInvalidFields)
			{
				throw new AssertException(string.Format("The actual number of invalid fields is {0}, but we expected {1}.", 
				                                        actualNumberOfInvalidFields, expectedNumberOfInvalidFields));
			}
		}

		public static ModelStateInvestigator ShouldContain(this ModelStateDictionary modelStateDictionary, string modelName)
		{
			ModelState modelState;
			if (!modelStateDictionary.TryGetValue(modelName, out modelState))
			{
				throw new AssertException(String.Format("The property '{0}' is not contained by the ModelStateDictionary object",
				                                        modelName));
			}
			return new ModelStateInvestigator(modelName, modelState);
		}

		public static ModelStateInvestigator ShouldContain<T>(this ModelStateDictionary modelStateDictionary, Expression<Func<T, string>> expression)
			where T : class
		{
			string name = expression.GetNameFor();
			return modelStateDictionary.ShouldContain(name);
		}
	}

	public class ModelStateInvestigator
	{
		private readonly string _modelName;
		private readonly ModelState _modelState;
		private string _attemptedValue;
		private bool _checkAttemptedValue;

		public ModelStateInvestigator(string modelName, ModelState modelState)
		{
			_modelName = modelName;
			_modelState = modelState;
		}

		public ModelStateInvestigator WithAttemptedValue(string expectedAttemptedValue)
		{
			_attemptedValue = expectedAttemptedValue;
			_checkAttemptedValue = true;
			return this;
		}

		public void AndException<T>(string expectedExceptionMessage)
		{
			ModelError modelError = getModelError();
			if (modelError.Exception == null)
			{
				throw new AssertException(String.Format("The property '{0}' has no exception model error.", _modelName));
			}
			if (modelError.Exception.GetType() != typeof(T))
			{
				throw new AssertException(String.Format("The exception model error is not of the expected type '{0}'. The actual type is '{1}'.", typeof(T), modelError.Exception.GetType()));
			}
			if (expectedExceptionMessage != modelError.Exception.Message)
			{
				throw new AssertException(String.Format("The actual Exception.Message '{0}' of property '{1}' is not equal to the expected value: '{2}'",
				                                        modelError.ErrorMessage, _modelName, expectedExceptionMessage));
			}
		}

		public void AndErrorMessage(string expectedErrorMessage)
		{
			ModelError modelError = getModelError();
			if (expectedErrorMessage != modelError.ErrorMessage)
			{
				throw new AssertException(String.Format("The actual ErrorMessage '{0}' of property '{1}' is not equal to the expected value: '{2}'",
				                                        modelError.ErrorMessage, _modelName, expectedErrorMessage));
			}
		}

		private ModelError getModelError()
		{
			if (_checkAttemptedValue)
			{
				if (_modelState.Value == null)
				{
					throw new AssertException(
						String.Format("No ValueProviderResult for the ModelState of '{0}'.", _modelName));
				}
				if (_attemptedValue != _modelState.Value.AttemptedValue)
				{
					throw new AssertException(
						String.Format("The actual AttemptedValue '{0}' of property '{1}' is not as expected '{2}'",
						              _modelState.Value.AttemptedValue, _modelName, _attemptedValue));
				}
			}
			ModelError modelError = _modelState.Errors.FirstOrDefault();
			if (modelError == null)
			{
				throw new AssertException(String.Format("The property '{0}' have no errors.", _modelName));
			}
			return modelError;
		}
	}
}