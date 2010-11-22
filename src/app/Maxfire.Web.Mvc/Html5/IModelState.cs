using System;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Maxfire.Web.Mvc.Html5
{
	public interface IModelNameValueAccessor<TModel> : IModelStateContainer
	{
		TModel Model { get; } // TODO: Do we need to expose root object
		string GetModelNameFor<TValue>(Expression<Func<TModel, TValue>> expression);
		// Note: Empty string is root object => GetModelStateForModel, GetModelValueForModel, GetModelMetadataForModel
		string GetAttemptedModelValue(string modelName);
		ModelMetadata GetModelMetadata(string modelName);
	}
}