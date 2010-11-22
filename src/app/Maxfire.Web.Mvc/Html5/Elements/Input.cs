using System.Web.Mvc;

namespace Maxfire.Web.Mvc.Html5.Elements
{
	public class Input : InputElement<Input> 
	{
		public Input(string type, string name, ModelMetadata modelMetadata) 
			: base(type, name, modelMetadata)
		{
		}
	}
}