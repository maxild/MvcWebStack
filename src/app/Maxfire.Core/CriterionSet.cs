using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Maxfire.Core
{
	public class CriterionSet : IEnumerable<Criterion>
	{
		private readonly List<Criterion> _criteria = new List<Criterion>();

		public string OrderBy { get; set; }

		public SortOrder SortOrder { get; set; }

		public IEnumerator<Criterion> GetEnumerator()
		{
			return _criteria.GetEnumerator();
		}

		public override bool Equals(object obj)
		{
			var other = (CriterionSet) obj;
			IEnumerable<Criterion> criteria = GetCriteria();
			IEnumerable<Criterion> otherCriteria = other.GetCriteria();

			if (criteria.Count() != otherCriteria.Count())
			{
				return false;
			}

			if (SortOrder != other.SortOrder || OrderBy != other.OrderBy)
			{
				return false;
			}

			bool criterionSetsEqual = true;

			foreach (Criterion criterion in criteria)
			{
				bool matchingCriterionFound = false;

				foreach (Criterion otherCriterion in otherCriteria)
				{
					object value1 = otherCriterion.Value;
					object value2 = criterion.Value;

					bool keyMatches = otherCriterion.Attribute == criterion.Attribute;
					bool valueMatches = (value1 == null && value2 == null) || (value1 != null && value1.Equals(value2));
					bool operatorMatches = otherCriterion.Operator == criterion.Operator;

					if (keyMatches && valueMatches && operatorMatches)
					{
						matchingCriterionFound = true;
						break;
					}
				}

				if (!matchingCriterionFound)
				{
					criterionSetsEqual = false;
					break;
				}
			}

			return criterionSetsEqual;
		}

		public override int GetHashCode()
		{
			int hashCode = 0;

			foreach (Criterion criterion in _criteria)
			{
				hashCode += criterion.GetHashCode();
			}

			return hashCode;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public Criterion Add(Criterion criterion)
		{
			_criteria.Add(criterion);
			return criterion;
		}

		public IEnumerable<Criterion> GetCriteria()
		{
			return _criteria;
		}
	}
}