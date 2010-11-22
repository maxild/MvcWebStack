using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using Maxfire.Core.Extensions;
using Maxfire.Web.Mvc.Html.Extensions;

namespace Maxfire.Web.Mvc
{
	public class ViewDataWrapper<T>
	{
		private readonly ViewDataDictionary _viewData;
		private readonly string _key;

		public ViewDataWrapper(ViewDataDictionary viewData)
		{
			_viewData = viewData;
			_key = "ViewDataWrapper<{0}>".FormatWith(typeof(T).FullName);
		}

		public T GetDataFor<TModel, TValue>(Expression<Func<TModel, TValue>> expression)
			where TModel : class
		{
			string key = expression.GetHtmlFieldNameFor(_viewData);
			return Hash.GetValueOrDefault(key);
		}

		public T GetDataFor<TModel>(Expression<Func<TModel, object>> expression)
			where TModel : class
		{
			string key = expression.GetHtmlFieldNameFor(_viewData);
			return Hash.GetValueOrDefault(key);
		}

		public void SetDataFor<TModel, TValue>(Expression<Func<TModel, TValue>> expression, T data)
			where TModel : class
		{
			string key = expression.GetHtmlFieldNameFor(_viewData);
			Hash[key] = data;
		}

		public void SetDataFor<TModel>(Expression<Func<TModel, object>> expression, T data)
			where TModel : class
		{
			string key = expression.GetHtmlFieldNameFor(_viewData);
			Hash[key] = data;
		}

		private IDictionary<string, T> Hash
		{
			get { return _viewData.GetOrCreate(_key, () => new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase)) as IDictionary<string, T>; }
		}
	}
}