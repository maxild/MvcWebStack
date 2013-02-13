using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using Maxfire.Core;
using Maxfire.Core.Extensions;
using Maxfire.Web.Mvc.Html.Extensions;

namespace Maxfire.Web.Mvc
{
	public static class ViewDataWrapper
	{
		const string OPTIONS_KEY = "Options";
		const string LABEL_TEXT_KEY = "LabelText";

		public static ViewDataWrapper<IEnumerable<TextValuePair>> NewOptionsWrapper(ViewDataDictionary viewData)
		{
			return new ViewDataWrapper<IEnumerable<TextValuePair>>(viewData, OPTIONS_KEY);
		}

		public static ViewDataWrapper<string> NewLabelTextWrapper(ViewDataDictionary viewData)
		{
			return new ViewDataWrapper<string>(viewData, LABEL_TEXT_KEY);
		}
	}

	public class ViewDataWrapper<T> : IDataProviderAndMutator<T>
	{
		private readonly ViewDataDictionary _viewData;
		private readonly string _key;

		public ViewDataWrapper(ViewDataDictionary viewData, string keyPrefix)
		{
			_viewData = viewData;
			_key = "__viewDataWrapper_" + keyPrefix;
		}

		public string Key
		{
			get { return _key; }
		}

		public T GetData(string key)
		{
			return Hash.GetValueOrDefault(key);
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
			SetDataFor(expression, null, data);
		}

		public void SetData(string key, T data)
		{
			Hash[key] = data;
		}

		public void SetDataFor<TModel>(Expression<Func<TModel, object>> expression, T data)
			where TModel : class
		{
			SetDataFor(expression, null, data);
		}

		public void SetDataFor<TModel, TValue>(Expression<Func<TModel, TValue>> expression, string prefix, T data)
			where TModel : class
		{
			string key = expression.GetHtmlFieldNameFor(_viewData);
			if (!string.IsNullOrEmpty(prefix))
			{
				key += prefix;
			}
			SetData(key, data);
		}

		public void SetDataFor<TModel>(Expression<Func<TModel, object>> expression, string prefix, T data)
			where TModel : class
		{
			string key = expression.GetHtmlFieldNameFor(_viewData);
			if (!string.IsNullOrEmpty(prefix))
			{
				key += prefix;
			}
			SetData(key, data);
		}

		private IDictionary<string, T> Hash
		{
			get { return _viewData.GetOrCreate(_key, () => new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase)) as IDictionary<string, T>; }
		}
	}
}