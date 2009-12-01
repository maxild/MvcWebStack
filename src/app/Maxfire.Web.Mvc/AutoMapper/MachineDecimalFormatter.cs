namespace Maxfire.Web.Mvc.AutoMapper
{
	public class MachineDecimalFormatter : ConventionsValueFormatter<decimal>
	{
		protected override string FormatValueCore(decimal value)
		{
			return Conventions.FormatMachineDecimal(value);
		}
	}
}