using System;
using System.Text;

namespace Maxfire.Web.Mvc
{
	public class ValidationException : Exception
	{
		public ValidationException(ValidationResult validationResult)
		{
			ValidationResult = validationResult;
		}

		public ValidationResult ValidationResult { get; }

		public override string Message
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				foreach (var kvp in ValidationResult.GetAllErrors())
				{
					foreach (var message in kvp.Value)
					{
						sb.AppendFormat("{0}: {1}", kvp.Key, message);
					}
				}
				return sb.ToString();
			}
		}
	}
}
