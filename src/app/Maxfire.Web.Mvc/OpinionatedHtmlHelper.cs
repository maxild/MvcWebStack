using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using Maxfire.Core.Extensions;
using Maxfire.Web.Mvc.Html.Extensions;
using Maxfire.Web.Mvc.Html5;

namespace Maxfire.Web.Mvc
{
	public class OpinionatedHtmlHelper<TModel> : HtmlHelper<TModel>, IModelMetadataAccessor<TModel> where TModel: class
	{
		private readonly INameValueSerializer _nameValueSerializer;

		public OpinionatedHtmlHelper(ViewContext viewContext, IViewDataContainer viewDataContainer, INameValueSerializer nameValueSerializer) : base(viewContext, viewDataContainer)
		{
			_nameValueSerializer = nameValueSerializer;
		}

		public virtual IEnumerable<SelectListItem> GetOptionsFor<TValue>(Expression<Func<TModel, TValue>> expression)
		{
			return OptionsWrapper.GetDataFor(expression) ?? GetOptionsOfType<TValue>();
		}

		private ViewDataWrapper<IEnumerable<SelectListItem>> _optionsWrapper;
		private ViewDataWrapper<IEnumerable<SelectListItem>> OptionsWrapper
		{
			get { return _optionsWrapper ?? (_optionsWrapper = new ViewDataWrapper<IEnumerable<SelectListItem>>(ViewData)); }
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

		public INameValueSerializer NameValueSerializer
		{
			get { return _nameValueSerializer; }
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

		string IModelMetadataAccessor.GetAttemptedModelValue(string modelName)
		{
			return GetAttemptedModelValue(modelName);
		}

		protected virtual string GetAttemptedModelValue(string modelName)
		{
			object value = GetModelMetadata(modelName).Model;
			if (value == null)
			{
				return null;
			}
			if (NameValueSerializer != null)
			{
				value = NameValueSerializer.GetValues(value).Map(x => x.Value).FirstOrDefault();
			}
			return Convert.ToString(value, CultureInfo.InvariantCulture);
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

		IEnumerable<SelectListItem> IModelMetadataAccessor.GetOptions(string modelName)
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