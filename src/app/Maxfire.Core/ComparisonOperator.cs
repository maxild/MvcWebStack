namespace Maxfire.Core
{
	public class ComparisonOperator : ConvertibleEnumeration<ComparisonOperator>
	{
		public static readonly ComparisonOperator Equal = new ComparisonOperator(1, "Equal", "Equal comparison operator");
		public static readonly ComparisonOperator NotEqual = new ComparisonOperator(2, "NotEqual", "Not equal comparison operator");
		public static readonly ComparisonOperator GreaterThan = new ComparisonOperator(3, "GreaterThan", "Greater than comparison operator");
		public static readonly ComparisonOperator LessThan = new ComparisonOperator(4, "LessThan", "Less than comparison operator");
		public static readonly ComparisonOperator GreaterThanOrEqual = new ComparisonOperator(5, "GreaterThanOrEqual", "Greater than or equal comparison operator");
		public static readonly ComparisonOperator LessThanOrEqual = new ComparisonOperator(6, "LessThanOrEqual", "Less than or equal comparison operator");

		private ComparisonOperator(int value, string name, string text)
			: base(value, name, text)
		{
		}
	}
}