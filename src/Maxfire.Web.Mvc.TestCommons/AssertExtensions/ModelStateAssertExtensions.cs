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
				throw new XunitException(
				    $"The actual number of invalid fields is {actualNumberOfInvalidFields}, but we expected {expectedNumberOfInvalidFields}.");
			}
		}

		public static ModelStateInvestigator ShouldContain(this ModelStateDictionary modelStateDictionary, string modelName)
		{
			ModelState modelState;
			if (!modelStateDictionary.TryGetValue(modelName, out modelState))
			{
				throw new XunitException($"The property '{modelName}' is not contained by the ModelStateDictionary object");
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
			ModelError modelError = GetModelError();
			if (modelError.Exception == null)
			{
				throw new XunitException($"The property '{_modelName}' has no exception model error.");
			}
			if (modelError.Exception.GetType() != typeof(T))
			{
				throw new XunitException(
				    $"The exception model error is not of the expected type '{typeof (T)}'. The actual type is '{modelError.Exception.GetType()}'.");
			}
			if (expectedExceptionMessage != modelError.Exception.Message)
			{
				throw new XunitException(
				    $"The actual Exception.Message '{modelError.ErrorMessage}' of property '{_modelName}' is not equal to the expected value: '{expectedExceptionMessage}'");
			}
		}

		public void AndErrorMessage(string expectedErrorMessage)
		{
			ModelError modelError = GetModelError();
			if (expectedErrorMessage != modelError.ErrorMessage)
			{
				throw new XunitException(
				    $"The actual ErrorMessage '{modelError.ErrorMessage}' of property '{_modelName}' is not equal to the expected value: '{expectedErrorMessage}'");
			}
		}

		private ModelError GetModelError()
		{
			if (_checkAttemptedValue)
			{
				if (_modelState.Value == null)
				{
					throw new XunitException(
					    $"No ValueProviderResult for the ModelState of '{_modelName}'.");
				}
				if (_attemptedValue != _modelState.Value.AttemptedValue)
				{
					throw new XunitException(
					    $"The actual AttemptedValue '{_modelState.Value.AttemptedValue}' of property '{_modelName}' is not as expected '{_attemptedValue}'");
				}
			}
			ModelError modelError = _modelState.Errors.FirstOrDefault();
			if (modelError == null)
			{
				throw new XunitException($"The property '{_modelName}' have no errors.");
			}
			return modelError;
		}
	}
}
