using System;
using System.Web.Mvc;
using Maxfire.Web.Mvc.FluentHtml.Extensions;

namespace Maxfire.Web.Mvc.Html5
{
	public class FormElement<T> : Element<T> where T : FormElement<T>
	{
		private readonly Func<ModelMetadata> _modelMetadataAccessor;

		public FormElement(string tagName, string name, Func<ModelMetadata> modelMetadataAccessor) : base(tagName)
		{
			_modelMetadataAccessor = modelMetadataAccessor;
			Name(name);
		}

		private ModelMetadata _modelMetadata;
		public ModelMetadata ModelMetadata
		{
			get { return _modelMetadata ?? (_modelMetadata = _modelMetadataAccessor()); }
		}

		public object Model
		{
			get { return ModelMetadata.Model; }
		}

		protected virtual void InferIdFromName()
		{
			if (!HasAttr(HtmlAttribute.Id))
			{
				SetId(Attr(HtmlAttribute.Name).FormatAsHtmlId());
			}
		}

		public T Name(string name)
		{
			Attr(HtmlAttribute.Name, name);
			return self;
		}

		public override string ToString()
		{
			InferIdFromName();
			return base.ToString();
		}
	}
}