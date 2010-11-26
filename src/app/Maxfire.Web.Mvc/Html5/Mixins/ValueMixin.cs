using Maxfire.Core.Extensions;
using Maxfire.Web.Mvc.Html5.Elements;
using Maxfire.Web.Mvc.Html5.HtmlTokens;

namespace Maxfire.Web.Mvc.Html5.Mixins
{
	public interface ISupportsValue<out T>
	{
		T Value(object value);
		object Value();
		TDestinationType ValueAs<TDestinationType>();
	}

	public class ValueMixin<T> : FragmentMixin<T>, ISupportsValue<T> where T : class, IHtmlFragment<T>
	{
		public ValueMixin(IHtmlFragment<T> fragment) : base(fragment)
		{
		}

		/// <summary>
		/// Set the value of the "value" attribute.
		/// </summary>
		/// <param name="value">The new value of the "value" attribute. 
		/// If null the "value" attribute is removed.</param>
		public T Value(object value)
		{
			if (value == null)
			{
				elem.RemoveAttr(HtmlAttribute.Value);
			}
			else
			{
				elem.Attr(HtmlAttribute.Value, value);
			}
			return self;
		}

		/// <summary>
		/// Get the value of the "value" attribute.
		/// </summary>
		public object Value()
		{
			return elem.Attr(HtmlAttribute.Value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TDestinationType"></typeparam>
		/// <returns>The value of the "value</returns>
		public TDestinationType ValueAs<TDestinationType>()
		{
			return (TDestinationType)TypeExtensions.ConvertSimpleType(null, Value(), typeof(TDestinationType));
		}
	}
}