using System;
using System.Web.Mvc;

namespace Maxfire.Web.Mvc.Html5.Elements
{
	public abstract class InputElement<T> : FormElement<T> where T : InputElement<T>
	{
		protected InputElement(string type, string name, ModelMetadata modelMetadata) 
			: base(HtmlElement.Input, name, modelMetadata)
		{
			SetType(type);
		}

		protected void SetType(string type)
		{
			Attr(HtmlAttribute.Type, type);
		}

		public override T Value(object value)
		{
			Attr(HtmlAttribute.Value, value);
			return self;
		}

		public override object Value()
		{
			return Attr(HtmlAttribute.Value);
		}
	}
}