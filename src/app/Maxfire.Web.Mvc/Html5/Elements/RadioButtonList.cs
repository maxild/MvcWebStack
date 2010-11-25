using System.Web.Mvc;

namespace Maxfire.Web.Mvc.Html5.Elements
{
	public class RadioButtonList : OptionsContainerElement<RadioButtonList>
	{
		// TODO: Problem at vi ikke ønsker nogen container (div)!!!!!
		public RadioButtonList(string name, IModelMetadataAccessor accessor) : base("div", name, accessor)
		{
		}

		protected override void ApplyModelStateAttemptedValue(ValueProviderResult attemptedValue)
		{
		}

		protected override string RenderOptions()
		{
			return null;
		}
	}
}