using Maxfire.Core;

namespace Maxfire.Web.Mvc.AutoMapper
{
	public class EnumerationToTextFormatter : AbstractEnumerationFormatter
	{
		protected override string FormatValueCore(Enumeration enumeration)
		{
			return enumeration.Text;
		}
	}
}