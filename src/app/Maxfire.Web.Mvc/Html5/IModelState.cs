using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Maxfire.Web.Mvc.Html5
{
	public interface IModelNameResolver<TModel>
	{
		/// <summary>
		/// Get the model name of the root Model or descendant properties in the model object graph.
		/// </summary>
		/// <param name="expression">A property expression</param>
		/// <returns>The model name of the requested model. It returns the empty 
		/// string for root Model (i.e. GetModelNameFor(m=&gt; m))</returns>
		string GetModelNameFor<TValue>(Expression<Func<TModel, TValue>> expression);
	}

	public interface IModelMetadataAccessor : IModelStateAccessor
	{
		/// <summary>
		/// Get the string-based value that the model binding system would resolve (or bind)
		/// to the current model value.
		/// </summary>
		/// <param name="modelName">The model name.</param>
		/// <returns>Normally the string-based value that the model binding system would resolve (or bind)
		/// to the current model value. If the model is null, it returns null.</returns>
		string GetAttemptedModelValue(string modelName);

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
		IEnumerable<SelectListItem> GetOptions(string modelName);
		
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