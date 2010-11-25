using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Maxfire.Web.Mvc.Html5.HtmlTokens;

namespace Maxfire.Web.Mvc.Html5.Elements
{
	/// <summary>
	/// A Fragment is either an element og a list of elements with identical tag names.
	/// </summary>
	public abstract class Fragment<T> : IHtmlString where T : Fragment<T>
	{
		// TODO: Behaviours
		private readonly TagBuilder _tagBuilder;
		private readonly ISet<string> _classNames = new SortedSet<string>(StringComparer.Ordinal);
		private TagRenderMode _tagRenderMode;

		protected Fragment(string tagName)
		{
			_tagRenderMode = TagRenderMode.SelfClosing;
			_tagBuilder = new TagBuilder(tagName);
		}

		protected T self { get { return (T) this; }}

		/// <summary>
		/// Get the tag name of this/each element.
		/// </summary>
		public string TagName
		{
			get { return _tagBuilder.TagName; }
		}

		/// <summary>
		/// How should the/each element be rendered.
		/// </summary>
		/// <param name="tagRenderMode">The tag render mode of the/each element.</param>
		public T RenderAs(TagRenderMode tagRenderMode)
		{
			_tagRenderMode = tagRenderMode;
			return self;
		}

		/// <summary>
		/// Get the tag render mode of the/each element.
		/// </summary>
		/// <returns>The tag render mode of the/each element</returns>
		public TagRenderMode RenderAs()
		{
			return _tagRenderMode;
		}

		/// <summary>
		/// Get the value of an attribute for the/each element.
		/// </summary>
		/// <param name="attributeName">The name of the attribute to get.</param>
		/// <returns>The value of the attribute, if found, otherwise null.</returns>
		public string Attr(string attributeName)
		{
			if (attributeName == HtmlAttribute.Class)
			{
				return _classNames.Count > 0 ? GetClass() : null;
			}
			string value;
			return _tagBuilder.Attributes.TryGetValue(attributeName, out value) ? value : null;
		}

		/// <summary>
		/// Set a single attribute for the/each element.
		/// </summary>
		/// <param name="attributeName">The name of the attribute to set.</param>
		/// <param name="value">A value to set for the attribute.</param>
		public T Attr(string attributeName, object value)
		{
			string valueAsString = Convert.ToString(value, CultureInfo.InvariantCulture);
			if (attributeName == HtmlAttribute.Class)
			{
				AddClass(valueAsString);
			}
			else
			{
				_tagBuilder.MergeAttribute(attributeName, valueAsString, true);
			}
			return self;
		}

		/// <summary>
		/// Set one or more attributes for the/each element.
		/// </summary>
		/// <param name="attributes">The collection of attributes to set.</param>
		public T Attr(IEnumerable<KeyValuePair<string, object>> attributes)
		{
			if (attributes != null)
			{
				foreach (var entry in attributes)
				{
					Attr(entry.Key, entry.Value);
				}
			}
			return self;
		}

		/// <summary>
		/// Set one or more attributes for the/each element.
		/// </summary>
		/// <param name="attributes">An object literal of attribute key-value pairs.</param>
		public T Attr(object attributes)
		{
			return Attr(HtmlHelper.AnonymousObjectToHtmlAttributes(attributes));
		}

		/// <summary>
		/// Remove an attribute from the/each element.
		/// </summary>
		/// <param name="attributeName">The attribute name to remove.</param>
		public T RemoveAttr(string attributeName)
		{
			if (attributeName == HtmlAttribute.Class)
			{
				_classNames.Clear();
			}
			else
			{
				_tagBuilder.Attributes.Remove(attributeName);
			}
			return self;
		}

		/// <summary>
		/// Determine whether the/each element contains the given attribute.
		/// </summary>
		/// <param name="attributeName">The name of the attribute to search for.</param>
		public bool HasAttr(string attributeName)
		{
			return _tagBuilder.Attributes.ContainsKey(attributeName);
		}

		/// <summary>
		/// Add inner text that is HTML encoded to the/each element.
		/// </summary>
		/// <param name="innerText">The text to be added inside the opening and closing tags of the/each element.</param>
		public T InnerText(string innerText)
		{
			if (string.IsNullOrWhiteSpace(innerText))
			{
				_tagBuilder.InnerHtml = null;
			}
			else
			{
				_tagBuilder.SetInnerText(innerText);
			}
			return self;
		}


		/// <summary>
		/// Add inner HTML content to the/each element 
		/// </summary>
		/// <param name="innerHtml">The HTML text fragment to add inside the opening and closing tags of the/each element.</param>
		public T InnerHtml(string innerHtml)
		{
			_tagBuilder.InnerHtml = innerHtml;
			return self;
		}

		/// <summary>
		/// Adds the specified class(es) to the/each element.
		/// </summary>
		/// <param name="className">One or more class names to be added to the class attribute of the/each element.</param>
		public T AddClass(params string[] className)
		{
			foreach (string part in className.SelectMany(splitClassName))
			{
				_classNames.Add(part);
			}
			return self;
		}

		/// <summary>
		/// Remove a single class, multiple classes, or all classes from the/each element.
		/// </summary>
		/// <param name="className">A class name to be removed from the class attribute of the/each element.</param>
		/// <returns>The element</returns>
		/// <remarks>
		/// If a class name is included as a parameter, then only that class will be removed from this element.
		/// If no class names are specified in the parameter, all classes will be removed.
		/// </remarks>
		public T RemoveClass(params string[] className)
		{
			if (className == null || className.Length == 0)
			{
				_classNames.Clear();
			}
			else
			{
				foreach (string part in className.SelectMany(splitClassName))
				{
					_classNames.Remove(part);
				}
			}
			return self;
		}

		/// <summary>
		/// Add or remove one or more classes from the/each element, depending on either the class's presence 
		/// or the value of the switch argument.
		/// </summary>
		/// <param name="className">One or more class names (separated by spaces) to be toggled on this element.</param>
		/// <param name="switch">A boolean value to determine whether the class should be added or removed.</param>
		public T ToggleClass(string className, bool? @switch = null)
		{
			bool addOrRemove = @switch.HasValue ? @switch.Value : !HasClass(className);
			if (addOrRemove)
			{
				AddClass(className);
			}
			else
			{
				RemoveClass(className);
			}
			return self;
		}

		/// <summary>
		/// Determine whether the/each element is assigned the given class.
		/// </summary>
		/// <param name="className">The class name to search for.</param>
		/// <returns>True, if the/each element is/are assigned the given class, otherwise false.</returns>
		public bool HasClass(string className = null)
		{
			return className == null ? _classNames.Count > 0 : _classNames.Contains(className);
		}

		// TODO: Data, RemoveData (a la jQuery setData, HTML5 data-prefix attributes)
		
		public override sealed string ToString()
		{
			return ToHtmlString();
		}

		public virtual string ToHtmlString()
		{
			return _tagBuilder.ToString(_tagRenderMode);
		}

		private static IEnumerable<string> splitClassName(string className)
		{
			if (string.IsNullOrEmpty(className))
			{
				yield break;
			}
			foreach (string part in className.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
			{
				yield return part;
			}
		}

		private string GetClass()
		{
			return _classNames.Aggregate((className, sum) => sum + " " + className).TrimEnd();
		}
	}
}