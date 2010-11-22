using System.Web.Mvc;
using Maxfire.Core.Extensions;
using Maxfire.Web.Mvc.Html5.HtmlTokens;

namespace Maxfire.Web.Mvc.Html5.Elements
{
	public abstract class FormElementWithValue<T> : FormElement<T> where T : FormElementWithValue<T>
	{
		protected FormElementWithValue(string tagName, string name, ModelMetadata modelMetadata) 
			: base(tagName, name, modelMetadata)
		{
		}

		// TODO: WithValue via mixin to be DRY

		/// <summary>
		/// Set the value of the "value" attribute.
		/// </summary>
		/// <param name="value">The new value of the "value" attribute. 
		/// If null the "value" attribute is removed.</param>
		public virtual T Value(object value)
		{
			if (value == null)
			{
				RemoveAttr(HtmlAttribute.Value);
			}
			else
			{
				Attr(HtmlAttribute.Value, value);
			}
			return self;
		}

		/// <summary>
		/// Get the value of the "value" attribute.
		/// </summary>
		public virtual object Value()
		{
			return Attr(HtmlAttribute.Value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TDestionationType"></typeparam>
		/// <returns>The value of the "value</returns>
		public TDestionationType ValueAs<TDestionationType>()
		{
			return (TDestionationType)TypeExtensions.ConvertSimpleType(null, Value(), typeof(TDestionationType));
		}
	}
}