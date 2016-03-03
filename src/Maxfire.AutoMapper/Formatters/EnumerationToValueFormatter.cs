using Maxfire.Core;

namespace Maxfire.AutoMapper.Web.Mvc.Formatters
{
	public class EnumerationToValueFormatter : AbstractEnumerationFormatter
	{
		protected override string FormatValueCore(Enumeration enumeration)
		{
			return enumeration.Value.ToString();
		}
	}
}