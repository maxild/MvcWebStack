using System.Collections.Generic;

namespace Maxfire.Web.Mvc
{
	public interface IModelUnflattener<TInputModel, TModel>
	{
		IDictionary<string, string[]> ValidateInput(TInputModel input);
		TModel MapInputToModel(TInputModel input);
	}
}