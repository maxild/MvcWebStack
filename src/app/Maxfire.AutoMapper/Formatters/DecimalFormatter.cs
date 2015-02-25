namespace Maxfire.AutoMapper.Web.Mvc.Formatters
{
	public class DecimalFormatter : ConventionsValueFormatter<decimal>
	{
		protected override string FormatValueCore(decimal value)
		{
			return Conventions.FormatDecimal(value);
		}
	}
}