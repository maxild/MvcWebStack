namespace Maxfire.TestCommons
{
	/// <summary>
	/// Abstract Spec base class that has no dependency on any (xunit.net, nunit, mbunit) library.
	/// </summary>
	public abstract class Spec : SpecBase
	{
		protected void Setup()
		{
			Establish_context();
			Because_of();
		}

		protected virtual void Establish_context()
		{
		}
	}

	/// <summary>
	/// Abstract Spec base class that has no dependency on any (xunit.net, nunit, mbunit) library.
	/// </summary>
	public abstract class Spec<TContext> : SpecBase
	{
		protected TContext Sut { get; private set; }

		protected void Setup()
		{
			Sut = Establish_context();
			Because_of();
		}

		protected abstract TContext Establish_context();
	}
}