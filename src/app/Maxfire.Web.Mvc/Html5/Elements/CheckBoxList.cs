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
			if (accessor != null)
			{
				var selectedValues = accessor.GetModelMetadata(name).Model as IEnumerable;
				if (selectedValues != null)
				{
					SetSelectedValues(selectedValues);
				}
			}
		}

		public IEnumerable<object> SelectedValues()
		{
			return Selected();
		}

		public CheckBoxList SelectedValues(IEnumerable selectedValues)
		{
			BindExplicitValue(selectedValues);
			return self;
		}

		protected override object GetAttemptedValue(ValueProviderResult attemptedValue)
		{
			return attemptedValue.ConvertTo<string[]>();
		}

		protected override void BindValue(object value)
		{
			SetSelectedValues(value as IEnumerable);
		}

		private void SetSelectedValues(IEnumerable selectedValues)
		{
			Selected(selectedValues);
		}
	}
}