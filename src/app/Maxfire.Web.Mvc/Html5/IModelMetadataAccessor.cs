using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using Maxfire.Core;
using Maxfire.Core.Extensions;

namespace Maxfire.Web.Mvc.Html5
{
	public interface IModelMetadataAccessor : IModelStateAccessor
	{
		/// <summary>
		/// Get the metadata of the root Model or descendant properties in the model object graph.
		/// </summary>
		/// <param name="modelName">The fully qualified model name (see HtmlFieldPrefix).</param>
		/// <returns>The model metadata of the requested model.</returns>
		ModelMetadata GetModelMetadata(string modelName);

		/// <summary>
		/// Get any options associated with this model.
		/// </summary>
		/// <param name="modelName">The fully qualified model name (see HtmlFieldPrefix).</param>
		IEnumerable<TextValuePair> GetOptions(string modelName);
		
		/// <summary>
		/// Get any label text associated with this model.
		/// </summary>
		/// <param name="modelName">The fully qualified model name (see HtmlFieldPrefix).</param>
		string GetLabelText(string modelName);
	}

	public static class ModelMetadataAccessorExtensions
	{
		public static object GetModel(this IModelMetadataAccessor modelMetadataAccessor, string modelName)
		{
			return modelMetadataAccessor.GetModelMetadata(modelName).Model;
		}

		public static string GetModelValueAsString(this IModelMetadataAccessor modelMetadataAccessor, string modelName)
		{
			return TypeExtensions.ConvertSimpleType<string>(CultureInfo.CurrentCulture, modelMetadataAccessor.GetModel(modelName));
		}
	}

	public interface IModelMetadataAccessor<TModel> : IModelNameResolver<TModel>, IModelMetadataAccessor
	{
	}
}