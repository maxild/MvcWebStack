using System.Web.Routing;
using Xunit;

namespace Maxfire.Web.Mvc.TestCommons.Routes
{
	public abstract class RoutesRegisteredBy<TFixture> : RoutesTesterBase, IClassFixture<TFixture>
		where TFixture : RoutesFixture, new()
	{
	    protected RoutesRegisteredBy(RoutesFixture fixture)
	    {
	        Fixture = fixture;
	    }

	    protected override INameValueSerializer NameValueSerializer => Fixture.NameValueSerializer;

	    protected override RouteCollection Routes => Fixture.Routes;

	    protected RoutesFixture Fixture { get; }
	}
}
