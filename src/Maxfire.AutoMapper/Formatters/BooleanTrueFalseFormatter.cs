namespace Maxfire.AutoMapper.Web.Mvc.Formatters
{
	public class BooleanTrueFalseFormatter : ConventionsValueFormatter<bool>
	{
		protected override string FormatValueCore(bool value)
		{
			return Conventions.FormatBoolean(value);
		}
	}
}