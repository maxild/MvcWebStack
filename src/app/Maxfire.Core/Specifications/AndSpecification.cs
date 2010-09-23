using System;
using System.Linq;

namespace Maxfire.Core.Specifications
{
	public class AndSpecification<T> : Specification<T>
	{
		private readonly ISpecification<T>[] _specs;
		
		public AndSpecification(ISpecification<T> leftSideSpec, ISpecification<T> rightSideSpec)
		{
			_specs = new[] { leftSideSpec, rightSideSpec };
		}

		public AndSpecification(params ISpecification<T>[] specs)
		{
			if (specs == null || specs.Length == 0) throw new ArgumentException("At least one specification argument must be given.");
			_specs = specs;
		}

		public override bool IsSatisfiedBy(T objectToTestSatisfaction)
		{
			// Empty list of specs will make specification satisfy anything because of Enumerable.All
			return _specs.All(t => t.IsSatisfiedBy(objectToTestSatisfaction));
		}
	}
}