using System.Web.Routing;

namespace Maxfire.Web.Mvc.TestCommons.Routes
{
	public abstract class RoutesFixtureBase : RoutesTesterBase
	{
		protected RoutesFixture Fixture { get; set; }

		protected override void RegisterRoutes(RouteCollection routes)
		{
			Fixture.RegisterRoutes(routes);
		}
	}
}