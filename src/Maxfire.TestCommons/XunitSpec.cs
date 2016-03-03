using System;

namespace Maxfire.TestCommons
{
	/// <summary>
	/// Abstract Spec class using xunit.net
	/// </summary>
	public abstract class XunitSpec : Spec, IDisposable
	{
		protected XunitSpec()
		{
			Setup();
		}

		public void Dispose()
		{
			Teardown();
		}
	}

	/// <summary>
	/// Abstract Spec class using xunit.net
	/// </summary>
	public abstract class XunitSpec<TContext> : Spec<TContext>, IDisposable
	{
		protected XunitSpec()
		{
			Setup();
		}

		public void Dispose()
		{
			Teardown();
		}
	}
}