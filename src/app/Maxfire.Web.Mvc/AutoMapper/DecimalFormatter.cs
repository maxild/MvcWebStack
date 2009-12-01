namespace Maxfire.Web.Mvc.AutoMapper
{
	public class DecimalFormatter : ConventionsValueFormatter<decimal>
	{
		protected override string FormatValueCore(decimal value)
		{
			return Conventions.FormatDecimal(value);
		}
	}
}