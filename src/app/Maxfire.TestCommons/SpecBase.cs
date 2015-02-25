using System;
using System.Diagnostics;
using System.Reflection;
using Rhino.Mocks;

namespace Maxfire.TestCommons
{
	/// <summary>
	/// SpecBase that has no dependency on any (xunit.net, NUnit, MbUnit) library.
	/// </summary>
	public abstract class SpecBase
	{
		protected void Teardown()
		{
			Cleanup();
		}

		protected virtual void Because_of()
		{
		}

		protected virtual void Cleanup()
		{
		}

		protected TType CreateDependency<TType>()
			where TType : class
		{
			return MockRepository.GenerateMock<TType>();
		}

		protected TType CreateStub<TType>()
			where TType : class
		{
			return MockRepository.GenerateStub<TType>();
		}

		protected void Spec_not_implemented()
		{
			MethodBase caller = new StackTrace().GetFrame(1).GetMethod();

			Spec_not_implemented(caller.DeclaringType.Name + "." + caller.Name);
		}

		protected void Spec_not_implemented(string specName)
		{
			Console.WriteLine("Specification not implemented : " + specName);
		}
	}
}