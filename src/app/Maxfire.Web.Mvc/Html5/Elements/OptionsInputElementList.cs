using System.Linq;
using System.Text;
using Maxfire.Core.Extensions;
using Maxfire.Web.Mvc.Html5.HtmlTokens;

namespace Maxfire.Web.Mvc.Html5.Elements
{
	public abstract class OptionsInputElementList<T> : OptionsFormFragment<T> where T : OptionsInputElementList<T>
	{
		protected OptionsInputElementList(string type, string name, IModelMetadataAccessor accessor) 
			: base(HtmlElement.Input, name, accessor)
		{
			Attr(HtmlAttribute.Type, type);
		}

		public override string ToHtmlString()
		{
			// TODO: Skal radio og checkboc have InnerText? Nej, kun value...
			// TODO: AutoLabel med item.Text og særskilte attributer
			// TODO: Unique id på hver radio/checkbox
			return RemoveClass().AddClass(Attr(HtmlAttribute.Class)).GetOptions()
				.Map(item => Value(item.Value).ToggleAttr(HtmlAttribute.Checked, item.Selected).InnerText(item.Text))
				.Aggregate(new StringBuilder(), (sb, me) => sb.Append(me.ToTagString()))
				.ToString();
		}
	}
}