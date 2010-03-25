using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Maxfire.CommonServiceLocator.CastleAdapter.UnitTests.Components;
using Microsoft.Practices.ServiceLocation;

namespace Maxfire.CommonServiceLocator.CastleAdapter.UnitTests
{
	public class ServiceLocatorFixture
	{
		public IServiceLocator ServiceLocator { get; private set; }

		public ServiceLocatorFixture()
		{
			IKernel container = new DefaultKernel()
				.Register(
				AllTypes.Of<ILogger>()
					.FromAssembly(typeof(ILogger).Assembly)
					.WithService.FirstInterface()
				);

			ServiceLocator = new CastleServiceLocator(container);
		}
	}
}