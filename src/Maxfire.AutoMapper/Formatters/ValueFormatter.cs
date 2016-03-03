using AutoMapper;
using Maxfire.Core.Extensions;

namespace Maxfire.AutoMapper.Web.Mvc.Formatters
{
	public abstract class ValueFormatter<T> : IValueFormatter
	{
		public string FormatValue(ResolutionContext context)
		{
			// Formatters are excecuted in a chain by the StringMapper (ValueFormatter)
			// in AutoMapper. Therefore they have to be idempotent (that is accept
			// a String even though SourceType is not of type String. Also we have
			// to map null values to the empty String. And because we may cast to a
			// non-nullable type we cannot handle null any other way. One has to use
			// the FormatNullValueAs API to handle null values differently.
			if (!(context.SourceValue is T))
				return context.SourceValue.ToNullSafeString();

			string formattedValue = FormatValueCore((T)context.SourceValue);
			return formattedValue;
		}

		protected abstract string FormatValueCore(T value);
	}
}