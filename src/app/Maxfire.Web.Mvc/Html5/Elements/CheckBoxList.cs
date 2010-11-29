using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;
using Maxfire.Web.Mvc.Html5.HtmlTokens;

namespace Maxfire.Web.Mvc.Html5.Elements
{
	public class CheckBoxList : OptionsInputElementList<CheckBoxList>
	{
		public CheckBoxList(string name, IModelMetadataAccessor accessor) 
			: base(HtmlInputType.Checkbox, name, accessor)
		{
		}

		public IEnumerable<object> SelectedValues()
		{
			return Selected();
		}

		public CheckBoxList SelectedValues(IEnumerable selectedValues)
		{
			Selected(selectedValues);
			return self;
		}

		protected override void ApplyModelStateAttemptedValue(ValueProviderResult attemptedValue)
		{
			SelectedValues(attemptedValue.ConvertTo<string[]>());
		}
	}
}