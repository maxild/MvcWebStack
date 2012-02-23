using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Maxfire.Core.Extensions;
using Maxfire.Core.Reflection;

namespace Maxfire.Web.Mvc
{
	public class ValidationResult
	{
		private const string UNBOUND_VALIDATION_ERROR_KEY = "no-member";

		public bool IsValid { get; private set; }
		
		private readonly IDictionary<string, string[]> _errors = new Dictionary<string, string[]>();

		public ValidationResult()
		{
			IsValid = true;
		}

		public ValidationResult AddUnboundError(string message)
		{
			return AddError(UNBOUND_VALIDATION_ERROR_KEY, message);
		}

		public ValidationResult AddError(string key, string message)
		{
			if (_errors.ContainsKey(key))
			{
				var strings = new List<string>(_errors[key]) { message };
				_errors[key] = strings.ToArray();
			}
			else
			{
				_errors.Add(key, new[] { message });
			}

			IsValid = false;

			return this;
		}

		public ValidationResult AddErrorFor<TViewModel>(Expression<Func<TViewModel, object>> expression, string message)
			where TViewModel : class
		{
			string key = expression.GetNameFor();
			return AddError(key, message);
		}

		public string[] GetUnboundErrors()
		{
			return GetErrors(UNBOUND_VALIDATION_ERROR_KEY);
		}

		public string[] GetErrors(string key)
		{
			return _errors.GetValueOrDefault(key, new string[] {});
		}

		public string[] GetErrorsFor<TViewModel>(Expression<Func<TViewModel, object>> expression)
			where TViewModel : class
		{
			return GetErrors(expression.GetNameFor());
		}

		public IDictionary<string, string[]> GetAllErrors()
		{
			return _errors;
		}
	}
}