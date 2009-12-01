namespace Maxfire.Core
{
	public class Criterion
	{
		public Criterion()
		{
		}

		public Criterion(string attribute, object value, ComparisonOperator comparisonOperator)
		{
			Attribute = attribute;
			Value = value;
			Operator = comparisonOperator;
		}

		public Criterion(string attribute, object value)
			: this(attribute, value, ComparisonOperator.Equal)
		{
		}

		public string Attribute { get; set; }

		public object Value { get; set; }

		public ComparisonOperator Operator { get; set; }

		public override bool Equals(object obj)
		{
			var other = (Criterion) obj;

			bool isEqual = other.Attribute == Attribute && other.Value == Value;
			return isEqual;
		}

		public override int GetHashCode()
		{
			string combinedKey = Attribute + Value;
			return combinedKey.GetHashCode();
		}
	}
}