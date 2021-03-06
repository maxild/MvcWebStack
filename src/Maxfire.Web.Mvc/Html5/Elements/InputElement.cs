using System.Diagnostics.CodeAnalysis;
using Maxfire.Core.Extensions;
using Maxfire.Web.Mvc.Html5.HtmlTokens;

namespace Maxfire.Web.Mvc.Html5.Elements
{
	[SuppressMessage("ReSharper", "MustUseReturnValue")]
	public abstract class InputElement<T> : FormFragment<T> where T : InputElement<T>
	{
		protected InputElement(string type, string name, IModelMetadataAccessor accessor)
			: base(HtmlElement.Input, name, accessor)
		{
			if (type.IsNotEmpty())
			{
				Attr(HtmlAttribute.Type, type);
			}
		}

		public override string Attr(string attributeName)
		{
			string value = base.Attr(attributeName);
			if (attributeName == HtmlAttribute.Type)
			{
				// Even though type attribute is not rendered we fallback to 'text' as this is the default.
				return value ?? "text";
			}
			return value;
		}
	}
}
