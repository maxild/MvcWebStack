using System.Web.Routing;
using Xunit;

namespace Maxfire.Web.Mvc.TestCommons.Routes
{
	public abstract class RoutesRegisteredBy<TFixture> : RoutesTesterBase, IUseFixture<TFixture> 
		where TFixture : RoutesFixture, new()
	{
		protected RoutesFixture Fixture { get; set; }

		protected override void RegisterRoutes(RouteCollection routes)
		{
			Fixture.RegisterRoutes(routes);
		}

		void IUseFixture<TFixture>.SetFixture(TFixture fixture)
		{
			Fixture = fixture;
		}
	}
}