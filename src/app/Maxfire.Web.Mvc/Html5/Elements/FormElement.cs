using System;
using System.Web.Mvc;
using Maxfire.Web.Mvc.FluentHtml.Extensions;

namespace Maxfire.Web.Mvc.Html5.Elements
{
	/// <summary>
	/// Form element bound to a model/member that is a control in the sense 
	/// that it takes part of form submission if it is a succesful control.
	/// </summary>
	public abstract class FormElement<T> : Element<T> where T : FormElement<T>
	{
		// TODO: Inherent label support via attribute (label from bindings that can take values none|before|after)
		private readonly ModelMetadata _modelMetadata;

		protected FormElement(string tagName, string name, ModelMetadata modelMetadata) : base(tagName)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("The argument cannot be empty.", "name");
			}
			_modelMetadata = modelMetadata;
			Name(name);
		}

		public ModelMetadata ModelMetadata
		{
			get { return _modelMetadata; }
		}

		protected virtual void InferIdFromName()
		{
			if (!HasAttr(HtmlAttribute.Id))
			{
				string name = Attr(HtmlAttribute.Name);
				SetId(name.FormatAsHtmlId());
			}
		}

		/// <summary>
		/// Set the value of the name attribute for this form element.
		/// </summary>
		/// <param name="name">The value of the name attribute for this form element.</param>
		/// <returns>The element.</returns>
		public T Name(string name)
		{
			Attr(HtmlAttribute.Name, name);
			return self;
		}

		/// <summary>
		/// Get the value of the name attribute for this form elememt.
		/// </summary>
		/// <returns></returns>
		public string Name()
		{
			return Attr(HtmlAttribute.Name);
		}

		/// <summary>
		/// Set the 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public abstract T Value(object value);

		/// <summary>
		/// Get the current value of this form element.
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// In the case of &lt;select multiple="multiple"&gt; elements or a radio button set,
		/// this  method returns an array containing the value of each selected option.
		/// </remarks>
		public abstract object Value();

		public override string ToString()
		{
			InferIdFromName();
			return base.ToString();
		}
	}
}