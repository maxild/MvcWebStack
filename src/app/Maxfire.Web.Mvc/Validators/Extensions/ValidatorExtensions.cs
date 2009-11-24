namespace Maxfire.Web.Mvc.Validators.Extensions
{
	public static class ValidatorExtensions
	{
		public static bool IsTrimmedEmpty(this object fieldValue)
		{
			return fieldValue.ToTrimmedNullSafeString().Length == 0;
		}

		public static bool IsTrimmedNotEmpty(this object fieldValue)
		{
			return fieldValue.ToTrimmedNullSafeString().Length != 0;
		}

		public static string ToTrimmedNullSafeString(this object fieldValue)
		{
			return fieldValue == null ? string.Empty : fieldValue.ToString().Trim();
		}
	}
}