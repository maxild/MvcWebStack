using Maxfire.Core;

namespace Maxfire.Web.Mvc.AutoMapper
{
	public class EnumerationToValueFormatter : AbstractEnumerationFormatter
	{
		protected override string FormatValueCore(Enumeration enumeration)
		{
			return enumeration.Value.ToString();
		}
	}
}