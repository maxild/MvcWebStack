using System.Web.Mvc;
using Maxfire.Web.Mvc.Html5.HtmlTokens;

namespace Maxfire.Web.Mvc.Html5.Elements
{
	public class Hidden : InputElement<Hidden>
	{
		public Hidden(string name, ModelMetadata modelMetadata) 
			: base(HtmlInputType.Hidden, name, modelMetadata)
		{
		}
	}
}