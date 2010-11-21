using System;
using System.Web.Mvc;

namespace Maxfire.Web.Mvc.Html5
{
	public abstract class InputElement<T> : FormElement<T> where T : InputElement<T>
	{
		protected InputElement(string type, string name, Func<ModelMetadata> modelMetadataAccessor) 
			: base(HtmlElement.Input, name, modelMetadataAccessor)
		{
			SetType(type);
		}

		protected void SetType(string type)
		{
			Attr(HtmlAttribute.Type, type);
		}
	}

	public class Input : InputElement<Input> 
	{
		public Input(string type, string name, Func<ModelMetadata> modelMetadataAccessor) 
			: base(type, name, modelMetadataAccessor)
		{
		}
	}
}