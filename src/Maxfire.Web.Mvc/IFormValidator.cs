using System.Collections.Generic;

namespace Maxfire.Web.Mvc
{
	public interface IFormValidator
	{
		IDictionary<string, string[]> GetValidationErrorsFor<TInputModel>(TInputModel input);
	}
}