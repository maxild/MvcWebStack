using System;
using System.Linq;

namespace Maxfire.Core.Specifications
{
	public class OrSpecification<T> : Specification<T>
	{
		private readonly ISpecification<T>[] _specs;
		
		public OrSpecification(ISpecification<T> leftSideSpec, ISpecification<T> rightSideSpec)
		{
			_specs = new[] { leftSideSpec, rightSideSpec };
		}

		public OrSpecification(params ISpecification<T>[] specs)
		{
			if (specs == null || specs.Length == 0) throw new ArgumentException("At least one specification argument must be given.");
			_specs = specs;
		}

		public override bool IsSatisfiedBy(T objectToTestSatisfaction)
		{
			// Empty list of specs will make specification satisfy nothing because of Enumerable.Any
			return _specs.Any(t => t.IsSatisfiedBy(objectToTestSatisfaction));
		}
	}
}