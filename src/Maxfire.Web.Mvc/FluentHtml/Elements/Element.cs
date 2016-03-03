using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using Maxfire.Web.Mvc.FluentHtml.Behaviors;
using Maxfire.Web.Mvc.FluentHtml.Extensions;
using Maxfire.Web.Mvc.FluentHtml.Html;

namespace Maxfire.Web.Mvc.FluentHtml.Elements
{
	/// <summary>
	/// Base class for HTML elements.
	/// </summary>
	/// <typeparam name="T">The derived class type.</typeparam>
	public abstract class Element<T> : IMemberElement, ISupportsBehaviors where T : Element<T>, IElement
	{
		private readonly TagBuilder _builder;
		private IEnumerable<IBehaviorMarker> _behaviorsApplied;
		private readonly MemberExpression _forMember;
		
		protected Element(string tag, MemberExpression forMember) : this(tag)
		{
			_forMember = forMember;
		}

		protected Element(string tag)
		{
			_builder = new TagBuilder(tag);
		}

		protected void SetId(string id)
		{
			SetAttr(HtmlAttribute.Id, id);
		}

		protected string GetId()
		{
			return GetAttr(HtmlAttribute.Id);
		}

		protected bool HasId()
		{
			return HasAttribute(HtmlAttribute.Id);
		}

		/// <summary>
		/// Set the 'id' attribute.
		/// </summary>
		/// <param name="value">The value of the 'id' attribute.</param>
		public T Id(string value)
		{
			SetId(value);
			return (T)this;
		}

		/// <summary>
		/// Add a value to the 'class' attribute.
		/// </summary>
		/// <param name="classToAdd">The value of the class to add.</param>
		public T Class(string classToAdd)
		{
			_builder.AddCssClassOnlyOnce(classToAdd);
			return (T)this;
		}

		/// <summary>
		/// Set the 'title' attribute.
		/// </summary>
		/// <param name="value">The value of the 'title' attribute.</param>
		public T Title(string value)
		{
			_builder.MergeAttribute(HtmlAttribute.Title, value, true);
			return (T)this;
		}

		/// <summary>
		/// Set the value of a specified attribute.
		/// </summary>
		/// <param name="name">The name of the attribute.</param>
		/// <param name="value">The value of the attribute.</param>
		public T Attr(string name, object value)
		{
			SetAttr(name, value);
			return (T)this;
		}

		public override string ToString()
		{
			PreRender();
			string html = _builder.ToString(TagRenderMode);
			return html;
		}

		void ISupportsBehaviors.ApplyBehaviors(IEnumerable<IBehaviorMarker> behaviors)
		{
			if (behaviors == null)
			{
				return;
			}
			_behaviorsApplied = behaviors;
			foreach (var behavior in behaviors)
			{
				if (behavior is IBehavior)
				{
					((IBehavior)behavior).Execute(this);
				}
				if (behavior is IMemberBehavior && ForMember != null)
				{
					((IMemberBehavior)behavior).Execute(this);
				}
			}
		}

		string IElement.TagName
		{
			get { return TagName; }
		}

		void IElement.AddCssClass(string @class)
		{
			AddCssClass(@class);
		}

		bool IElement.HasAttribute(string name)
		{
			return HasAttribute(name);
		}

		void IElement.RemoveAttr(string name)
		{
			RemoveAttr(name);
		}

		void IElement.SetAttr(string name, object value)
		{
			SetAttr(name, value);
		}

		string IElement.GetAttr(string name)
		{
			return GetAttr(name);
		}

		MemberExpression IMemberElement.ForMember
		{
			get { return ForMember; }
		}

		protected IEnumerable<IBehaviorMarker> AppliedBehaviors
		{
			get { return _behaviorsApplied; }
		}

		protected MemberExpression ForMember
		{
			get { return _forMember; }
		}

		protected string TagName
		{
			get { return _builder.TagName; }
		}

		protected void SetInnerText(string text)
		{
			_builder.SetInnerText(text);
		}

		protected void SetInnerHtml(string html)
		{
			_builder.InnerHtml = html;
		}

		protected void AddCssClass(string @class)
		{
			_builder.AddCssClassOnlyOnce(@class);
		}

		protected bool HasAttribute(string name)
		{
			return _builder.HasAttribute(name);
		}

		protected void RemoveAttr(string name)
		{
			_builder.Attributes.Remove(name);
		}

		protected void SetAttr(string name, object value)
		{
			if (value == null)
			{
				return;
			}
			var valueAsString = value.ToString();
			_builder.MergeAttribute(name, valueAsString, true);
		}

		protected string GetAttr(string name)
		{
			string result;
			_builder.Attributes.TryGetValue(name, out result);
			return result;
		}

		protected virtual TagRenderMode TagRenderMode
		{
			get { return TagRenderMode.Normal; }
		}

		protected virtual void PreRender() { }
	}
}