using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace Maxfire.Web.Mvc.Html5.Elements
{
	public interface IHtmlFragment<out T> : IHtmlString where T : IHtmlFragment<T>
	{
		/// <summary>
		/// Get the tag name of this/each element.
		/// </summary>
		string TagName { get; }

		/// <summary>
		/// How should the/each element be rendered.
		/// </summary>
		/// <param name="tagRenderMode">The tag render mode of the/each element.</param>
		T RenderAs(TagRenderMode tagRenderMode);


		/// <summary>
		/// Get the tag render mode of the/each element.
		/// </summary>
		/// <returns>The tag render mode of the/each element</returns>
		TagRenderMode RenderAs();

		/// <summary>
		/// Get the value of an attribute for the/each element.
		/// </summary>
		/// <param name="attributeName">The name of the attribute to get.</param>
		/// <returns>The value of the attribute, if found, otherwise null.</returns>
		string Attr(string attributeName);


		/// <summary>
		/// Set a single attribute for the/each element.
		/// </summary>
		/// <param name="attributeName">The name of the attribute to set.</param>
		/// <param name="value">A value to set for the attribute.</param>
		T Attr(string attributeName, object value);


		/// <summary>
		/// Set one or more attributes for the/each element.
		/// </summary>
		/// <param name="attributes">The collection of attributes to set.</param>
		T Attr(IEnumerable<KeyValuePair<string, object>> attributes);


		/// <summary>
		/// Set one or more attributes for the/each element.
		/// </summary>
		/// <param name="attributes">An object literal of attribute key-value pairs.</param>
		T Attr(object attributes);

		/// <summary>
		/// Remove an attribute from the/each element.
		/// </summary>
		/// <param name="attributeName">The attribute name to remove.</param>
		T RemoveAttr(string attributeName);

		/// <summary>
		/// Determine whether the/each element contains the given attribute.
		/// </summary>
		/// <param name="attributeName">The name of the attribute to search for.</param>
		bool HasAttr(string attributeName);

		/// <summary>
		/// Add inner text that is HTML encoded to the/each element.
		/// </summary>
		/// <param name="innerText">The text to be added inside the opening and closing tags of the/each element.</param>
		T InnerText(string innerText);

		/// <summary>
		/// Add inner HTML content to the/each element 
		/// </summary>
		/// <param name="innerHtml">The HTML text fragment to add inside the opening and closing tags of the/each element.</param>
		T InnerHtml(string innerHtml);

		/// <summary>
		/// Adds the specified class(es) to the/each element.
		/// </summary>
		/// <param name="className">One or more class names to be added to the class attribute of the/each element.</param>
		T AddClass(params string[] className);

		/// <summary>
		/// Remove a single class, multiple classes, or all classes from the/each element.
		/// </summary>
		/// <param name="className">A class name to be removed from the class attribute of the/each element.</param>
		/// <returns>The element</returns>
		/// <remarks>
		/// If a class name is included as a parameter, then only that class will be removed from this element.
		/// If no class names are specified in the parameter, all classes will be removed.
		/// </remarks>
		T RemoveClass(params string[] className);

		/// <summary>
		/// Add or remove one or more classes from the/each element, depending on either the class's presence 
		/// or the value of the switch argument.
		/// </summary>
		/// <param name="className">One or more class names (separated by spaces) to be toggled on this element.</param>
		/// <param name="switch">A boolean value to determine whether the class should be added or removed.</param>
		T ToggleClass(string className, bool? @switch = null);

		/// <summary>
		/// Determine whether the/each element is assigned the given class.
		/// </summary>
		/// <param name="className">The class name to search for.</param>
		/// <returns>True, if the/each element is/are assigned the given class, otherwise false.</returns>
		bool HasClass(string className = null);
		
		// TODO: Data, RemoveData (a la jQuery setData, HTML5 data-prefix attributes)
	}
}