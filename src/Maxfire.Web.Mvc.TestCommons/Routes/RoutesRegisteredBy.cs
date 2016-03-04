using System.Web.Routing;
using Xunit;

namespace Maxfire.Web.Mvc.TestCommons.Routes
{
    /// <summary>
    /// ABC to perform routes testing.
    /// </summary>
    /// <typeparam name="TFixture"></typeparam>
	public abstract class RoutesRegisteredBy<TFixture> : RoutesTesterBase, IClassFixture<TFixture>
		where TFixture : RoutesFixture, new()
	{
        /// <summary>
        /// Constructor used by xUnit.net to inject the test context (aka class fixture)
        /// </summary>
        /// <param name="fixture">
        /// The test context (aka class fixture) that xUnit.net have just created by invoking
        /// the parameterless constructor of the fixture class.
        /// </param>
	    protected RoutesRegisteredBy(TFixture fixture)
	    {
	        Fixture = fixture;
	    }

	    protected override INameValueSerializer NameValueSerializer => Fixture.NameValueSerializer;

	    protected override RouteCollection Routes => Fixture.Routes;

	    protected TFixture Fixture { get; }
	}
}
