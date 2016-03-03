namespace Maxfire.AutoMapper.Web.Mvc.Formatters
{
	public class BooleanJaNejFormatter : ConventionsValueFormatter<bool>
	{
		protected override string FormatValueCore(bool value)
		{
			return Conventions.FormatBooleanToJaNej(value);
		}
	}
}