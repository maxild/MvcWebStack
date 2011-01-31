using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;
using Maxfire.Core;
using Maxfire.Core.Extensions;
using Maxfire.Web.Mvc.Html.Extensions;
using Maxfire.Web.Mvc.Html5;

namespace Maxfire.Web.Mvc
{
	public class OpinionatedHtmlHelper : HtmlHelper, IUrlHelper
	{
		private readonly INameValueSerializer _nameValueSerializer;

		public OpinionatedHtmlHelper(ViewContext viewContext, IViewDataContainer viewDataContainer, INameValueSerializer nameValueSerializer)
			: base(viewContext, viewDataContainer)
		{
			_nameValueSerializer = nameValueSerializer;
		}

		public string GetVirtualPath(RouteValueDictionary routeValues)
		{
			return UrlHelperUtil.GetVirtualPath(RouteTable.Routes, ViewContext.RequestContext, routeValues);
		}

		public virtual string SiteRoot
		{
			get { return UrlHelperUtil.GetSiteRoot(ViewContext.RequestContext); }
		}

		public virtual string SiteResource(string path)
		{
			return UrlHelper.GenerateContentUrl(path, ViewContext.HttpContext);
		}

		public INameValueSerializer NameValueSerializer
		{
			get { return _nameValueSerializer; }
		}
	}

	// Note : OpinionatedHtmlHelper<TModel> has to derive from HtmlHelper<T> (This is Liskov's substitution 
	// principle and enables us to use Html.ValidationMessage and other System-defined view helpers).
	public class OpinionatedHtmlHelper<TModel> : HtmlHelper<TModel>, IModelMetadataAccessor<TModel>, IUrlHelper
		where TModel: class
	{
		private readonly INameValueSerializer _nameValueSerializer;
		
		public OpinionatedHtmlHelper(ViewContext viewContext, IViewDataContainer viewDataContainer, INameValueSerializer nameValueSerializer) 
			: base(viewContext, viewDataContainer)
		{
			_nameValueSerializer = nameValueSerializer;
		}

		public string GetVirtualPath(RouteValueDictionary routeValues)
		{
			return UrlHelperUtil.GetVirtualPath(RouteTable.Routes, ViewContext.RequestContext, routeValues);
		}

		public virtual string SiteRoot
		{
			get { return UrlHelperUtil.GetSiteRoot(ViewContext.RequestContext); }
		}

		public virtual string SiteResource(string path)
		{
			return UrlHelper.GenerateContentUrl(path, ViewContext.HttpContext);
		}

		public INameValueSerializer NameValueSerializer
		{
			get { return _nameValueSerializer; }
		}

		public virtual IEnumerable<TextValuePair> GetOptions(string key)
		{
			return OptionsWrapper.GetData(key);
		}

		public virtual IEnumerable<TextValuePair> GetOptionsFor<TValue>(Expression<Func<TModel, TValue>> expression)
		{
			return OptionsWrapper.GetDataFor(expression) ?? GetOptionsOfType<TValue>();
		}

		private ViewDataWrapper<IEnumerable<TextValuePair>> _optionsWrapper;
		private ViewDataWrapper<IEnumerable<TextValuePair>> OptionsWrapper
		{
			get { return _optionsWrapper ?? (_optionsWrapper = new ViewDataWrapper<IEnumerable<TextValuePair>>(ViewData)); }
		}

		private static IEnumerable<TextValuePair> GetOptionsOfType<T>()
		{
			Type type = typeof (T);

			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				type = type.GetGenericArguments()[0];
			}

			if (type == typeof(bool))
			{
				return OptionsAdapter.Boolean();
			}
			if (type.IsEnum)
			{
				return OptionsAdapter.FromEnumTexts(type);
			}

			return null;
		}

		public virtual string GetLabelTextFor<TValue>(Expression<Func<TModel, TValue>> expression)
		{
			return LabelTextWrapper.GetDataFor(expression) ?? GetDisplayNameFor(expression);
		}

		private string GetDisplayNameFor<TValue>(Expression<Func<TModel, TValue>> expression)
		{
			var self = ((IModelMetadataAccessor<TModel>) this);
			string modelName = self.GetModelNameFor(expression);
			ModelMetadata modelMetadata = self.GetModelMetadata(modelName);
			return modelMetadata.DisplayName ?? modelMetadata.PropertyName ?? expression.GetHtmlFieldNameFor().Split('.').Last();
		}

		private ViewDataWrapper<string> _labelTextWrapper;
		private ViewDataWrapper<string> LabelTextWrapper
		{
			get { return _labelTextWrapper ?? (_labelTextWrapper = new ViewDataWrapper<string>(ViewData)); }
		}

		#region IModelMetadataAccessor Explicit Members

		ModelState IModelStateAccessor.GetModelState(string modelName)
		{
			ModelState modelState;
			return (ViewData.ModelState.TryGetValue(modelName, out modelState)) ? modelState : null;
		}

		string IModelNameResolver<TModel>.GetModelNameFor<TValue>(Expression<Func<TModel, TValue>> expression)
		{
			return GetModelNameFor(expression);
		}

		protected virtual string GetModelNameFor<TValue>(Expression<Func<TModel, TValue>> expression)
		{
			string key = ExpressionHelper.GetExpressionText(expression);
			TryAdd(key, () => ModelMetadata.FromLambdaExpression(expression, ViewData));
			return key;
		}

		string IModelMetadataAccessor.GetFullHtmlFieldName(string modelName)
		{
			return GetFullHtmlFieldName(modelName);
		}

		protected virtual string GetFullHtmlFieldName(string modelName)
		{
			return ViewData.TemplateInfo.GetFullHtmlFieldName(modelName);
		}

		object IModelMetadataAccessor.GetAttemptedModelValue(string modelName)
		{
			return GetAttemptedModelValue(modelName);
		}

		protected virtual object GetAttemptedModelValue(string modelName)
		{
			object value = GetModelMetadata(modelName).Model;
			if (value != null && NameValueSerializer != null)
			{
				value = NameValueSerializer.GetValues(value).Map(x => x.Value).FirstOrDefault();
			}
			return value;
		}

		ModelMetadata IModelMetadataAccessor.GetModelMetadata(string modelName)
		{
			return GetModelMetadata(modelName);
		}

		protected virtual ModelMetadata GetModelMetadata(string modelName)
		{
			if (modelName == null)
			{
				throw new ArgumentNullException("modelName");
			}
			if (!_cachedModelValues.ContainsKey(modelName))
			{
				if (string.IsNullOrEmpty(modelName))
				{
					throw new ArgumentException(
						"The empty modelName does not exist in the cache. Did you remember to call GetModelNameFor(m => m) to populate the cache.");
				}

				throw new ArgumentException(
					"The modelName does not exist in the cache. Did you remember to call GetModelNameFor(m => m.{0}) to populate the cache."
						.FormatWith(modelName));
			}
			return _cachedModelValues[modelName];
		}

		IEnumerable<TextValuePair> IModelMetadataAccessor.GetOptions(string modelName)
		{
			return OptionsWrapper.GetData(modelName);
		}

		string IModelMetadataAccessor.GetLabelText(string modelName)
		{
			return LabelTextWrapper.GetData(modelName);
		}

		private readonly Dictionary<string, ModelMetadata> _cachedModelValues = new Dictionary<string, ModelMetadata>(StringComparer.Ordinal);
		private void TryAdd(string key, Func<ModelMetadata> factory)
		{
			ModelMetadata item;
			if (!_cachedModelValues.TryGetValue(key, out item))
			{
				item = factory();
				_cachedModelValues.Add(key, item);
			}
			return;
		}

		#endregion

	}
}