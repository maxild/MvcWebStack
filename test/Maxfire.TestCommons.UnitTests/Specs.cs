using Maxfire.TestCommons.AssertExtensions;
using Rhino.Mocks;
using Context = System.Diagnostics.CodeAnalysis.UsedImplicitlyAttribute;
using Specification = Xunit.FactAttribute;

namespace Maxfire.TestCommons.UnitTests
{
	[Context]
	public class When_initializing_the_spec : XunitSpec<StopWatch>
	{
		protected override StopWatch Establish_context()
		{
			return new StopWatch();
		}

		protected override void Because_of()
		{
		}

		[Specification]
		public void Should_populate_the_SUT_before_starting_the_specification()
		{
			Sut.ShouldNotBeNull();
		}
	}

	[Context]
	public class When_initializing_the_spec_with_mocks : XunitSpec<StopWatch>
	{
		private ITimer _timer;

		protected override StopWatch Establish_context()
		{
		    _timer = MockRepository.GenerateMock<ITimer>();

			_timer.Stub(x => x.Start(null)).IgnoreArguments().Return(true);

			return new StopWatch(_timer);
		}

		protected override void Because_of()
		{
			Sut.Start();
		}

		[Specification]
		public void Should_call_the_Because_of_method_before_starting_the_specification()
		{
			_timer.AssertWasCalled(x => x.Start(null), opt => opt.IgnoreArguments());
		}
	}

	public class StopWatch
	{
		private readonly ITimer _timer;

		public StopWatch()
		{
		}

		public StopWatch(ITimer timer)
		{
			_timer = timer;
		}

		public void Start()
		{
			_timer.Start("");
		}
	}

	public interface ITimer
	{
		bool Start(string reason);
		void Start();
	}
}
