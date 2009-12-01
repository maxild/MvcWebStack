namespace Maxfire.Core.Specifications
{
	public class NotSpecification<T> : Specification<T>
	{
		private readonly ISpecification<T> _specification;

		public NotSpecification(ISpecification<T> specification)
		{
			_specification = specification;
		}

		public override bool IsSatisfiedBy(T objectToTestSatisfaction)
		{
			return !_specification.IsSatisfiedBy(objectToTestSatisfaction);
		}
	}
}