using Maxfire.Core;

namespace Maxfire.Web.Mvc.AutoMapper
{
	public class EnumerationToNameFormatter : AbstractEnumerationFormatter
	{
		protected override string FormatValueCore(Enumeration enumeration)
		{
			return enumeration.Name;
		}
	}
}