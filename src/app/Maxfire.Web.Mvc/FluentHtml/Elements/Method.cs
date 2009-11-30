using Maxfire.Web.Mvc.FluentHtml.Html;

namespace Maxfire.Web.Mvc.FluentHtml.Elements
{
	public class Method : TextInput<Method>
	{
		public Method() 
			: base(HtmlInputType.Hidden, "_method", null)
		{
			_value = "PUT";
		}
	}
}