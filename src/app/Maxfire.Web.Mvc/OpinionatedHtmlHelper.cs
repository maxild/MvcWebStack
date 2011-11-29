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
			return OptionsWrapper.GetData(key) ?? GetDefaultOptions(key);
		}

		public virtual IEnumerable<TextValuePair> GetOptionsFor<TValue>(Expression<Func<TModel, TValue>> expression)
		{
			return OptionsWrapper.GetDataFor(expression) ?? GetDefaultOptions<TValue>();
		}

		private ViewDataWrapper<IEnumerable<TextValuePair>> _optionsWrapper;
		private ViewDataWrapper<IEnumerable<TextValuePair>> OptionsWrapper
		{
			get { return _optionsWrapper ?? (_optionsWrapper = new ViewDataWrapper<IEnumerable<TextValuePair>>(ViewData)); }
		}

		private IEnumerable<TextValuePair> GetDefaultOptions(string key)
		{
			ModelMetadata modelMetadata = _cachedModelMetadataHash.GetValueOrDefault(key);
			Type type = modelMetadata != null ? modelMetadata.ModelType : null;
			return GetDefaultOptions(type);
		}

		private static IEnumerable<TextValuePair> GetDefaultOptions(Type type)
		{
			if (type == null)
			{
				return null;
			}

			if (type.IsGenericType && !type.IsGenericTypeDefinition && type.GetGenericTypeDefinition() == typeof(Nullable<>))
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

		private static IEnumerable<TextValuePair> GetDefaultOptions<T>()
		{
			return GetDefaultOptions(typeof (T));
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
			string modelName = ExpressionHelper.GetExpressionText(expression);
			string fullHtmlFieldName = ViewData.TemplateInfo.GetFullHtmlFieldName(modelName);
			TryAdd(fullHtmlFieldName, () => ModelMetadata.FromLambdaExpression(expression, ViewData));
			return fullHtmlFieldName;
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
			if (!_cachedModelMetadataHash.ContainsKey(modelName))
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
			return _cachedModelMetadataHash[modelName];
		}

		IEnumerable<TextValuePair> IModelMetadataAccessor.GetOptions(string modelName)
		{
			return GetOptions(modelName);
		}

		string IModelMetadataAccessor.GetLabelText(string modelName)
		{
			return LabelTextWrapper.GetData(modelName);
		}

		private readonly Dictionary<string, ModelMetadata> _cachedModelMetadataHash = new Dictionary<string, ModelMetadata>(StringComparer.Ordinal);
		private void TryAdd(string key, Func<ModelMetadata> factory)
		{
			ModelMetadata item;
			if (!_cachedModelMetadataHash.TryGetValue(key, out item))
			{
				item = factory();
				_cachedModelMetadataHash.Add(key, item);
			}
			return;
		}

		#endregion

	}
}