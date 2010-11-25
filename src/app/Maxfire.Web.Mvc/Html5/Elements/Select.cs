using System.Linq;
using System.Text;
using System.Web.Mvc;
using Maxfire.Core.Extensions;
using Maxfire.Web.Mvc.Html5.HtmlTokens;

namespace Maxfire.Web.Mvc.Html5.Elements
{
	public class Select : OptionsFormElement<Select>
	{
		public Select(string name, IModelMetadataAccessor accessor) 
			: base(HtmlElement.Select, name, accessor)
		{
			SelectedValue(accessor.GetModelMetadata(name).Model);
		}

		// TODO: Can be made private??
		public object SelectedValue()
		{
			return Selected().FirstOrDefault();
		}

		// TODO: Can be made private??
		public Select SelectedValue(object value)
		{
			Selected(value != null ? new[] {value} : null);
			return self;
		}

		protected override string RenderOptions()
		{
			return Options()
				.Map(item => new Option().Value(item.Value).InnerText(item.Text))
				.Aggregate(new StringBuilder(), (sb, option) => sb.Append(option))
				.ToString();
		}

		protected override void ApplyModelStateAttemptedValue(ValueProviderResult attemptedValue)
		{
			SelectedValue(attemptedValue.ConvertTo<string>());
		}
	}
}