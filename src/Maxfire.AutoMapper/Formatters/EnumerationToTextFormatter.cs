using Maxfire.Core;

namespace Maxfire.AutoMapper.Web.Mvc.Formatters
{
	public class EnumerationToTextFormatter : AbstractEnumerationFormatter
	{
		protected override string FormatValueCore(Enumeration enumeration)
		{
			return enumeration.Text;
		}
	}
}