using AutoMapper;
using Maxfire.Core;
using Maxfire.Core.Extensions;

namespace Maxfire.AutoMapper.Web.Mvc.Formatters
{
	public abstract class AbstractEnumerationFormatter : IValueFormatter
	{
		public string FormatValue(ResolutionContext context)
		{
			if (context.SourceValue == null || !(context.SourceValue is Enumeration))
				return context.SourceValue.ToNullSafeString();

			return FormatValueCore((Enumeration)context.SourceValue);
		}

		protected abstract string FormatValueCore(Enumeration enumeration);
	}
}