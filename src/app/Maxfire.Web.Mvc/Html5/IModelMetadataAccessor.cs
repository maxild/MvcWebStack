using System.Collections.Generic;
using System.Web.Mvc;
using Maxfire.Core;

namespace Maxfire.Web.Mvc.Html5
{
	public interface IModelMetadataAccessor : IModelStateAccessor
	{
		/// <summary>
		/// Get the string-based value that the model binding system would resolve (or bind)
		/// to the current model value.
		/// </summary>
		/// <param name="modelName">The model name.</param>
		/// <returns>Normally the string-based value that the model binding system would resolve (or bind)
		/// to the current model value. If the model is null, it returns null.</returns>
		object GetAttemptedModelValue(string modelName);

		/// <summary>
		/// Get the metadata of the root Model or descendant properties in the model object graph.
		/// </summary>
		/// <param name="modelName">The model name.</param>
		/// <returns>The model metadata of the requested model.</returns>
		ModelMetadata GetModelMetadata(string modelName);

		/// <summary>
		/// Get any options associated with this model.
		/// </summary>
		/// <param name="modelName">The model name.</param>
		IEnumerable<TextValuePair> GetOptions(string modelName);
		
		/// <summary>
		/// Get any label text associated with this model.
		/// </summary>
		/// <param name="modelName">The model name.</param>
		string GetLabelText(string modelName);
	}

	public interface IModelMetadataAccessor<TModel> : IModelNameResolver<TModel>, IModelMetadataAccessor
	{
	}
}