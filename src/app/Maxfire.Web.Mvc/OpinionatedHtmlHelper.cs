using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using Maxfire.Core.Extensions;
using Maxfire.Web.Mvc.Html5;

namespace Maxfire.Web.Mvc
{
	public class OpinionatedHtmlHelper<TModel> : HtmlHelper<TModel>, IModelNameValueAccessor<TModel> where TModel: class
	{
		private readonly INameValueSerializer _nameValueSerializer;

		public OpinionatedHtmlHelper(ViewContext viewContext, IViewDataContainer viewDataContainer, INameValueSerializer nameValueSerializer) : base(viewContext, viewDataContainer)
		{
			_nameValueSerializer = nameValueSerializer;
		}

		public virtual IEnumerable<SelectListItem> GetOptionsFor<TValue>(Expression<Func<TModel, TValue>> expression)
		{
			return OptionsWrapper.GetOptionsFor(expression) ?? GetOptionsOfType<TValue>();
		}

		private OptionsViewDataWrapper _optionsWrapper;
		protected OptionsViewDataWrapper OptionsWrapper
		{
			get { return _optionsWrapper ?? (_optionsWrapper = new OptionsViewDataWrapper(ViewData)); }
		}

		private static IEnumerable<SelectListItem> GetOptionsOfType<T>()
		{
			if (typeof(T) == typeof(bool) || typeof(T) == typeof(bool?))
			{
				return OptionsAdapter2.Boolean();
			}
			if (typeof(T).IsEnum)
			{
				return OptionsAdapter2.FromEnumTexts<T>();
			}

			return null;
		}

		public INameValueSerializer NameValueSerializer
		{
			get { return _nameValueSerializer; }
		}

		ModelStateDictionary IModelStateContainer.ModelState
		{
			get { return ViewData.ModelState; }
		}

		TModel IModelNameValueAccessor<TModel>.Model
		{
			get { return ViewData.Model; }
		}

		string IModelNameValueAccessor<TModel>.GetModelNameFor<TValue>(Expression<Func<TModel, TValue>> expression)
		{
			string key = ExpressionHelper.GetExpressionText(expression);
			return GetOrAdd(key, () => new ModelInfo(key, ModelMetadata.FromLambdaExpression(expression, ViewData))).Name;
		}

		public string GetAttemptedModelValue(string modelName)
		{
			object value = _cachedModelValues[modelName].Value;
			if (NameValueSerializer != null)
			{
				value = NameValueSerializer.GetValues(value).Map(x => x.Value).FirstOrDefault();
			}
			return Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
		}

		public ModelMetadata GetModelMetadata(string modelName)
		{
			return _cachedModelValues[modelName].Metadata;
		}

		private readonly Dictionary<string, ModelInfo> _cachedModelValues = new Dictionary<string, ModelInfo>(StringComparer.Ordinal);
		private ModelInfo GetOrAdd(string key, Func<ModelInfo> factory)
		{
			ModelInfo item;
			if (!_cachedModelValues.TryGetValue(key, out item))
			{
				item = factory();
				_cachedModelValues.Add(key, item);
			}
			return item;
		}

		class ModelInfo
		{
			public ModelInfo(string name, ModelMetadata metadata)
			{
				Name = name;
				Metadata = metadata;
			}

			public string Name { get; private set; }

			public object Value { get { return Metadata.Model; } }

			public ModelMetadata Metadata { get; private set; }
		}
	}
}