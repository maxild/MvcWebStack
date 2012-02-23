namespace Maxfire.AutoMapper.Web.Mvc.Formatters
{
	public class MachineDecimalFormatter : ConventionsValueFormatter<decimal>
	{
		protected override string FormatValueCore(decimal value)
		{
			return Conventions.FormatMachineDecimal(value);
		}
	}
}