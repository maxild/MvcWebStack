using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Maxfire.Web.Mvc.Html5.Elements
{
	public abstract class Element<T> : IHtmlString
		where T : Element<T>
	{
		// TODO: Behaviours
		private readonly TagBuilder _tagBuilder;
		readonly HashSet<string> _classNames = new HashSet<string>(StringComparer.Ordinal);

		protected Element(string tagName)
		{
			_tagBuilder = new TagBuilder(tagName);
		}

		protected T self { get { return (T) this; }}

		/// <summary>
		/// Get the tag name of this element.
		/// </summary>
		public string TagName
		{
			get { return _tagBuilder.TagName; }
		}

		/// <summary>
		/// Get the value of an attribute for this element.
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
		/// Set a single attribute for this element.
		/// </summary>
		/// <param name="attributeName">The name of the attribute to set.</param>
		/// <param name="value">A value to set for the attribute.</param>
		/// <returns>The element.</returns>
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
		/// Set one or more attributes for this element.
		/// </summary>
		/// <param name="attributes">The hash of attributes to set.</param>
		/// <returns>The element.</returns>
		public T Attr(IDictionary<string, object> attributes)
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
		/// Determine whether this element contains the given attribute.
		/// </summary>
		/// <param name="attributeName">The name of the attribute to search for.</param>
		/// <returns></returns>
		public bool HasAttr(string attributeName)
		{
			return _tagBuilder.Attributes.ContainsKey(attributeName);
		}

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
		/// Adds the specified class(es) to this element.
		/// </summary>
		/// <param name="className">One or more class names to be added to the class attribute of this element.</param>
		/// <returns>The element.</returns>
		public T AddClass(params string[] className)
		{
			foreach (string part in className.SelectMany(splitClassName))
			{
				_classNames.Add(part);
			}
			return self;
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
		/// Determine whether this element is assigned the given class.
		/// </summary>
		/// <param name="className">The class name to search for.</param>
		/// <returns>True, if this element is assigned the given class, otherwise false.</returns>
		public bool HasClass(string className)
		{
			return _classNames.Contains(className);
		}

		public override string ToString()
		{
			return ToHtmlString();
		}

		public string ToHtmlString()
		{
			if (_classNames.Count > 0)
			{
				_tagBuilder.MergeAttribute(HtmlAttribute.Class, GetClass());
			}
			return _tagBuilder.ToString(TagRenderMode.SelfClosing);
		}

		private string GetClass()
		{
			return _classNames.Aggregate((className, sum) => sum + " " + className).TrimEnd();
		}

		protected void SetId(string value)
		{
			Attr(HtmlAttribute.Id, value);
		}
	}
}