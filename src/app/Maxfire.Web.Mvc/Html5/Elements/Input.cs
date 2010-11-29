using System.Web.Mvc;
using Maxfire.Web.Mvc.Html5.HtmlTokens;

namespace Maxfire.Web.Mvc.Html5.Elements
{
	public abstract class InputElement<T> : FormFragment<T> where T : InputElement<T>
	{
		protected InputElement(string type, string name, IModelMetadataAccessor accessor)
			: base(HtmlElement.Input, name, accessor)
		{
			Attr(HtmlAttribute.Type, type);
		}

		protected override void ApplyModelStateAttemptedValue(ValueProviderResult attemptedValue)
		{
			Value(attemptedValue.ConvertTo<string>());
		}

		protected override string ToTagString()
		{
			RemoveClass().AddClass(Attr(HtmlAttribute.Class));
			return base.ToTagString();
		}
	}

	public class Input : InputElement<Input> 
	{
		public Input(string type, string name, IModelMetadataAccessor accessor) 
			: base(type, name, accessor)
		{
		}
	}
}