using Maxfire.Core;

namespace Maxfire.AutoMapper.Web.Mvc.Formatters
{
	public class EnumerationToNameFormatter : AbstractEnumerationFormatter
	{
		protected override string FormatValueCore(Enumeration enumeration)
		{
			return enumeration.Name;
		}
	}
}