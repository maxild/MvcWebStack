using System.Collections.Generic;
using System.Web.Mvc;

namespace Maxfire.Web.Mvc.Html5.Elements
{
	// TODO: Finish this class and RadioButtonList

	// Common base class for element and elementlist
	public abstract class Fragment
	{
		
	}

	/// <summary>
	/// Base class for a list of elements with the same tagName where only text/value attributes can vary.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class ElementList<T> where T : ElementList<T>
	{
		private readonly string _tagName;
		private TagRenderMode _tagRenderMode;

		protected ElementList(string tagName)
		{
			_tagName = tagName;
		}

		protected T self { get { return (T)this; } }

		/// <summary>
		/// Get the tag name of this element.
		/// </summary>
		public string TagName
		{
			get { return _tagName; }
		}

		public T RenderAs(TagRenderMode tagRenderMode)
		{
			_tagRenderMode = tagRenderMode;
			return self;
		}

		public TagRenderMode RenderAs()
		{
			return _tagRenderMode;
		}

		/// <summary>
		/// Get the value of an attribute for this element.
		/// </summary>
		/// <param name="attributeName">The name of the attribute to get.</param>
		/// <returns>The value of the attribute, if found, otherwise null.</returns>
		public string Attr(string attributeName)
		{
			return null;
		}

		/// <summary>
		/// Set a single attribute for this element.
		/// </summary>
		/// <param name="attributeName">The name of the attribute to set.</param>
		/// <param name="value">A value to set for the attribute.</param>
		/// <returns>The element.</returns>
		public T Attr(string attributeName, object value)
		{
			return self;
		}

		/// <summary>
		/// Set one or more attributes for this element.
		/// </summary>
		/// <param name="attributes">The hash of attributes to set.</param>
		/// <returns>The element.</returns>
		public T Attr(IDictionary<string, object> attributes)
		{
			return self;
		}

		/// <summary>
		/// Set one or more attributes for this element.
		/// </summary>
		/// <param name="htmlAttributes">An object literal</param>
		/// <returns>The element.</returns>
		public T Attr(object htmlAttributes)
		{
			return Attr(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
		}

		/// <summary>
		/// Remove an attribute from this element.
		/// </summary>
		/// <param name="attributeName">An attribute to remove.</param>
		/// <returns>The element.</returns>
		public T RemoveAttr(string attributeName)
		{
			return self;
		}

		/// <summary>
		/// Determine whether this element contains the given attribute.
		/// </summary>
		/// <param name="attributeName">The name of the attribute to search for.</param>
		/// <returns></returns>
		public bool HasAttr(string attributeName)
		{
			return false;
		}

		/// <summary>
		/// Add inner text that is HTML encoded.
		/// </summary>
		/// <param name="innerText">Text to add inside the opening and closing tags of this element.</param>
		public T InnerText(string innerText)
		{
			return self;
		}

		/// <summary>
		/// Add inner HTML (that is off course not HTML encoded)
		/// </summary>
		/// <param name="innerHtml">HTML text fragment to add inside the opening and closing tags of this element.</param>
		public T InnerHtml(string innerHtml)
		{
			return self;
		}

		/// <summary>
		/// Adds the specified class(es) to this element.
		/// </summary>
		/// <param name="className">One or more class names to be added to the class attribute of this element.</param>
		/// <returns>The element.</returns>
		public T AddClass(params string[] className)
		{
			return self;
		}

		/// <summary>
		/// Remove a single class, multiple classes, or all classes from this element.
		/// </summary>
		/// <param name="className">A class name to be removed from the class attribute of this element.</param>
		/// <returns>The element</returns>
		/// <remarks>
		/// If a class name is included as a parameter, then only that class will be removed from this element.
		/// If no class names are specified in the parameter, all classes will be removed.
		/// </remarks>
		public T RemoveClass(params string[] className)
		{
			return self;
		}

		// TODO: Data, RemoveData

		/// <summary>
		/// Add or remove one or more classes from this element, depending on either the class's presence 
		/// or the value of the switch argument.
		/// </summary>
		/// <param name="className">One or more class names (separated by spaces) to be toggled on this element.</param>
		/// <param name="switch">A boolean value to determine whether the class should be added or removed.</param>
		/// <returns></returns>
		public T ToggleClass(string className, bool? @switch = null)
		{
			return self;
		}

		/// <summary>
		/// Determine whether this element is assigned the given class.
		/// </summary>
		/// <param name="className">The class name to search for.</param>
		/// <returns>True, if this element is assigned the given class, otherwise false.</returns>
		public bool HasClass(string className)
		{
			return false;
		}

		public override string ToString()
		{
			return null;
		}

		public string ToHtmlString()
		{
			return ToString();
		}
	}
}