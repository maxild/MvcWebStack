namespace Maxfire.Web.Mvc.AutoMapper
{
	public class BooleanTrueFalseFormatter : ConventionsValueFormatter<bool>
	{
		protected override string FormatValueCore(bool value)
		{
			return Conventions.FormatBoolean(value);
		}
	}
}