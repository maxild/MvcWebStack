using System.Web.Mvc;
using Maxfire.Web.Mvc.Html5.HtmlTokens;

namespace Maxfire.Web.Mvc.Html5.Elements
{
	public abstract class InputElement<T> : FormElementWithValue<T> where T : InputElement<T>
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
	}
}