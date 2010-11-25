using System.Web.Mvc;
using Maxfire.Web.Mvc.Html5.HtmlTokens;

namespace Maxfire.Web.Mvc.Html5.Elements
{
	public abstract class InputElement<T> : FormElementWithValue<T> where T : InputElement<T>
	{
		protected InputElement(string type, string name, IModelMetadataAccessor accessor) 
			: base(HtmlElement.Input, name, accessor)
		{
			SetType(type);
		}

		protected void SetType(string type)
		{
			Attr(HtmlAttribute.Type, type);
		}

		protected override void ApplyModelStateAttemptedValue(ValueProviderResult attemptedValue)
		{
			Value(attemptedValue.ConvertTo<string>());
		}
	}
}