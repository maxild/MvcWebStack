using Maxfire.Web.Mvc.FluentHtml.Html;

namespace Maxfire.Web.Mvc.FluentHtml.Behaviors
{
	/// <summary>
	/// This behavior will add the type of the input element as a CSS class 
	/// attribute to the input element. This can be necessary to support IE6
	/// (and below) because this browser does not support an attribute selector 
	/// CSS rule such as input[type="text"]. Instead use a class selector such 
	/// as 'input.text'.
	/// </summary>
	public class InputTypeBehavior : IBehavior
	{
		public void Execute(IElement element)
		{
			var tagName = element.TagName;
			if (tagName == HtmlTag.Input)
			{
				var type = element.GetAttr(HtmlAttribute.Type);
				if (type != HtmlInputType.Hidden)
				{
					element.AddCssClass(type);
				}
			}
		}
	}
}