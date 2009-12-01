namespace Maxfire.Web.Mvc.AutoMapper
{
	public class BooleanJaNejFormatter : ConventionsValueFormatter<bool>
	{
		protected override string FormatValueCore(bool value)
		{
			return Conventions.FormatBooleanToJaNej(value);
		}
	}
}