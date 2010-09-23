namespace Maxfire.Core.Specifications
{
	public abstract class Specification<T> : ISpecification<T>
	{
		public abstract bool IsSatisfiedBy(T candidate);

		public ISpecification<T> And(ISpecification<T> specification)
		{
			return new AndSpecification<T>(this, specification);
		}

		public ISpecification<T> Or(ISpecification<T> specification)
		{
			return new OrSpecification<T>(this, specification);
		}

		public ISpecification<T> Not()
		{
			return new NotSpecification<T>(this);
		}

		public static Specification<T> operator *(Specification<T> leftSide, Specification<T> rightSide)
		{
			return new AndSpecification<T>(leftSide, rightSide);
		}

		public static Specification<T> operator +(Specification<T> leftSide, Specification<T> rightSide)
		{
			return new OrSpecification<T>(leftSide, rightSide);
		}

		public static Specification<T> operator !(Specification<T> other)
		{
			return new NotSpecification<T>(other);
		}
	}

	public static class Specification
	{
		public static ISpecification<T> True<T>()
		{
			return new TrueSpecification<T>();
		}

		public static ISpecification<T> False<T>()
		{
			return new FalseSpecification<T>();
		}

		class TrueSpecification<T> : Specification<T>
		{
			public override bool IsSatisfiedBy(T candidate)
			{
				return true;
			}
		}

		class FalseSpecification<T> : Specification<T>
		{
			public override bool IsSatisfiedBy(T candidate)
			{
				return false;
			}
		}
	}
}