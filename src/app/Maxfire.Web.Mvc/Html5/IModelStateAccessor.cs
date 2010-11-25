using System.Web.Mvc;

namespace Maxfire.Web.Mvc.Html5
{
	public interface IModelStateAccessor
	{
		/// <summary>
		/// Try get the model state associated with this model.
		/// </summary>
		/// <param name="modelName">The named part of the model graph</param>
		/// <returns>The model state associated with the named part of the model graph, 
		/// or null if no model state have been set for this named part of the model graph.</returns>
		/// <remarks>
		/// The model state associated with the model graph has been 
		/// defined during model binding by any 'good citizen' model binder
		/// associated with this model graph.
		/// </remarks>
		ModelState GetModelState(string modelName);
	}
}