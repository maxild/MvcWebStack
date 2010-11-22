using System.Collections;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Maxfire.Core.Extensions;
using Maxfire.Web.Mvc.Html5.HtmlTokens;

namespace Maxfire.Web.Mvc.Html5.Elements
{
	public class Select : OptionsElement<Select>
	{
		public Select(string name, ModelMetadata modelMetadata) 
			: base(HtmlElement.Select, name, modelMetadata)
		{
		}

		protected override string RenderOptions()
		{
			return Options()
				.Map(item => new Option().Value(item.Value).InnerText(item.Text))
				.Aggregate(new StringBuilder(), (sb, option) => sb.Append(option))
				.ToString();
		}
	}
}