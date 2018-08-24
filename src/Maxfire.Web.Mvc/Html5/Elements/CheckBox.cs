using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Web.Mvc;
using Maxfire.Core.Extensions;
using Maxfire.Web.Mvc.Html5.HtmlTokens;

namespace Maxfire.Web.Mvc.Html5.Elements
{
	/// <summary>
	/// Boolean single-valued checkbox (with hidden control of false)
	/// </summary>
	[SuppressMessage("ReSharper", "MustUseReturnValue")]
    public class CheckBox : InputElement<CheckBox>
	{
		public CheckBox(string name, IModelMetadataAccessor accessor)
			: base(HtmlInputType.Checkbox, name, accessor)
		{
			if (accessor != null)
			{
				SetChecked(accessor.GetModelValueAs<bool?>(name));
			}
		}

		/// <summary>
		/// Set the checked attribute.
		/// </summary>
		/// <param name="value">Whether the checkbox should be checked.</param>
		public CheckBox Checked(bool value)
		{
			BindExplicitValue(value); // we cannot call SetChecked, because we need explicit value flag to be set
			return self;
		}

		/// <summary>
		/// Determine if checkbox is checked.
		/// </summary>
		/// <remarks>Equivalent to ValueAs&lt;bool&gt;</remarks>
		public bool Checked()
		{
			return HasAttr(HtmlAttribute.Checked);
		}

		public override object Value()
		{
			return GetValue(Checked());
		}

		private string GetValue(bool state)
		{
			return state ? "true,false" : "false";
		}

		public override CheckBox Value(object explicitValue)
		{
			// Setting value other than "true,false"/"false" is not supported...we fail silently using a noop.
			return self;
		}

		protected override object GetAttemptedValue(ValueProviderResult attemptedValue)
		{
			return attemptedValue.ConvertTo<bool?>();
		}

		protected override void BindValue(object value)
		{
			var isChecked = TypeExtensions.ConvertSimpleType<bool?>(CultureInfo.CurrentCulture, value);
			SetChecked(isChecked);
		}

		private void SetChecked(bool? isChecked)
		{
			if (isChecked.HasValue)
			{
				SetChecked(isChecked.Value);
			}
		}

		private void SetChecked(bool isChecked)
		{
			if (isChecked)
			{
				Attr(HtmlAttribute.Checked, HtmlAttribute.Checked);
			}
			else
			{
				RemoveAttr(HtmlAttribute.Checked);
			}
		}

		/// <summary>
		/// Set the disabled attribute, and configure the submitted (succesful, hidden)
		/// value to be defioned by the Checked property.
		/// </summary>
		/// <param name="value">Whether the checkbox should be disabled (but succesful, as in readonly).</param>
		public CheckBox ReadOnly(bool value)
		{
			if (value)
			{
				Attr(HtmlAttribute.Disabled, HtmlAttribute.Disabled);
			}
			else
			{
				RemoveAttr(HtmlAttribute.Disabled);
			}
			return self;
		}

		public bool ReadOnly()
		{
			return HasAttr(HtmlAttribute.Disabled);
		}

		public override string ToHtmlString()
		{
			string checkbox = base.Value("true").ToTagString();
			// If not disabled, the hidden contral is "false", if disabled the checked status defines the value
			// This way Disabled is more like
			string hidden = new Hidden(Attr(HtmlAttribute.Name), ModelMetadataAccessor, GetValue(ReadOnly() && Checked())).ToHtmlString();
			return string.Concat(checkbox, hidden);
		}

		class Hidden : InputElement<Hidden>
		{
			public Hidden(string name, IModelMetadataAccessor accessor, string value)
				: base(HtmlInputType.Hidden, name, accessor)
			{
				SetValueAttr(value);
			}

			protected override void BindValue(object value)
			{
				// not used
			}
		}
	}
}
