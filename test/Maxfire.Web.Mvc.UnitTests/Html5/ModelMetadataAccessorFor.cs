using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Web.Mvc;
using Maxfire.Core;
using Maxfire.Web.Mvc.Html.Extensions;
using Maxfire.Web.Mvc.Html5;

namespace Maxfire.Web.Mvc.UnitTests.Html5
{
	public class ModelMetadataAccessorFor<TModel> : IModelMetadataAccessor<TModel>
	{
		private readonly ModelState _modelState;
		private readonly ModelMetadata _modelMetadata;

		public ModelMetadataAccessorFor(string attemptedValue, Func<TModel> modelAccesor)
			: this(new ModelState { Value = attemptedValue != null ? new ValueProviderResult(attemptedValue, attemptedValue, CultureInfo.InvariantCulture) : null}, modelAccesor)
		{
		}

		private ModelMetadataAccessorFor(ModelState modelState, Func<TModel> modelAccesor)
		{
			_modelState = modelState;
			_modelMetadata = new FakeModelMetadata(() => modelAccesor());
		}

		public string GetModelNameFor<TValue>(Expression<Func<TModel, TValue>> expression)
		{
			string name = expression.GetHtmlFieldNameFor();
			return name;
		}

		public ModelState GetModelState(string modelName)
		{
			return _modelState;
		}

		public ModelMetadata GetModelMetadata(string modelName)
		{
			return _modelMetadata;
		}

		public IEnumerable<TextValuePair> GetOptions(string modelName)
		{
			yield break;
		}

		public string GetLabelText(string modelName)
		{
			return null;
		}

		private class FakeModelMetadata : ModelMetadata
		{
			public FakeModelMetadata(Func<object> modelAccessor)
				: base(new FakeMetadataProvider(), null, modelAccessor, typeof(TModel), null)
			{
			}

			public override IEnumerable<ModelMetadata> Properties
			{
				get { throw new NotSupportedException(); }
			}
		}

		private class FakeMetadataProvider : ModelMetadataProvider
		{
			public override IEnumerable<ModelMetadata> GetMetadataForProperties(object container, Type containerType)
			{
				yield break;
			}

			public override ModelMetadata GetMetadataForProperty(Func<object> modelAccessor, Type containerType,
			                                                     string propertyName)
			{
				return null;
			}

			public override ModelMetadata GetMetadataForType(Func<object> modelAccessor, Type modelType)
			{
				return null;
			}
		}
	}
}