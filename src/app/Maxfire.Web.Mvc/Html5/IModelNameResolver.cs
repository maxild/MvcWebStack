using System;
using System.Linq.Expressions;

namespace Maxfire.Web.Mvc.Html5
{
	public interface IModelNameResolver<TModel>
	{
		/// <summary>
		/// Get the model name of the root Model or descendant properties in the model object graph.
		/// </summary>
		/// <param name="expression">A property expression.</param>
		/// <returns>
		/// The model name of the requested model. It returns the empty 
		/// string for the root Model (i.e. GetModelNameFor(m=&gt; m))
		/// </returns>
		string GetModelNameFor<TValue>(Expression<Func<TModel, TValue>> expression);
	}
}