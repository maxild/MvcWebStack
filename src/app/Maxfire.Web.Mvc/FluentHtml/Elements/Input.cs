using System.Linq.Expressions;
using System.Web.Mvc;
using Maxfire.Core.Extensions;
using Maxfire.Web.Mvc.FluentHtml.Behaviors;
using Maxfire.Web.Mvc.FluentHtml.Html;

namespace Maxfire.Web.Mvc.FluentHtml.Elements
{
	/// <summary>
	/// Base class for input elements.
	/// </summary>
	/// <typeparam name="T">Derived class type.</typeparam>
	public abstract class Input<T> : FormElement<T>, ISupportsModelState where T : Input<T>, IElement
	{
		protected object _value;

		protected Input(string type, string name, MemberExpression forMember)
			: base(HtmlTag.Input, name, forMember)
		{
			if (type.IsNotEmpty())
			{
				SetAttr(HtmlAttribute.Type, type);
			}
		}

		/// <summary>
		/// Set the 'value' attribute.
		/// </summary>
		/// <param name="value">The value for the attribute.</param>
		public T Value(object value)
		{
			_value = value;
			return (T)this;
		}

		/// <summary>
		/// Set the 'size' attribute.
		/// </summary>
		/// <param name="value">The value for the attribute.</param>
		public T Size(int value)
		{
			Attr(HtmlAttribute.Size, value);
			return (T)this;
		}

		protected override void PreRender()
		{
			Attr(HtmlAttribute.Value, _value);
			base.PreRender();
		}

		void ISupportsModelState.ApplyModelState(ModelState state)
		{
			ApplyModelState(state);
		}

		protected virtual void ApplyModelState(ModelState state)
		{
			var value = state.Value.ConvertTo(typeof(string));
			Value(value);
		}
	}
}