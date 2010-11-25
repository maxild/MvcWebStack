using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Maxfire.Web.Mvc.Html5.Elements
{
	public class RadioButtonList : OptionElementList<RadioButtonList>
	{
		public RadioButtonList(string tagName, string name, IModelMetadataAccessor accessor) 
			: base(tagName, name, accessor)
		{
		}

		// TODO: Can be made private??
		public object SelectedValue()
		{
			return Selected().FirstOrDefault();
		}

		// TODO: Can be made private??
		public RadioButtonList SelectedValue(object value)
		{
			Selected(value != null ? new[] { value } : null);
			return self;
		}

		protected override bool PreRender()
		{
			return false;
		}

		protected override void ApplyModelStateAttemptedValue(ValueProviderResult attemptedValue)
		{
		}
	}

	public class CheckBoxList : OptionElementList<CheckBoxList>
	{
		public CheckBoxList(string tagName, string name, IModelMetadataAccessor accessor) : base(tagName, name, accessor)
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

		protected override bool PreRender()
		{
			return false;
		}

		protected override void ApplyModelStateAttemptedValue(ValueProviderResult attemptedValue)
		{
		}
	}
}