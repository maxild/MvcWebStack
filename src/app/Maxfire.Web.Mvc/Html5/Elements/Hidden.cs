using Maxfire.Web.Mvc.Html5.HtmlTokens;

namespace Maxfire.Web.Mvc.Html5.Elements
{
	public class Hidden : InputElement<Hidden>
	{
		public Hidden(string name, IModelMetadataAccessor accessor) 
			: base(HtmlInputType.Hidden, name, accessor)
		{
		}
	}
}