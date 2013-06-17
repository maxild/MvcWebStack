using System.Web.Routing;
using Xunit;

namespace Maxfire.Web.Mvc.TestCommons.Routes
{
	public abstract class RoutesRegisteredBy<TFixture> : RoutesTesterBase, IUseFixture<TFixture> 
		where TFixture : RoutesFixture, new()
	{
		
		protected override INameValueSerializer NameValueSerializer
		{
			get { return Fixture.NameValueSerializer; }
		}

		protected override RouteCollection Routes
		{
			get { return Fixture.Routes; }
		}

		protected RoutesFixture Fixture { get; set; }
		void IUseFixture<TFixture>.SetFixture(TFixture fixture)
		{
			Fixture = fixture;
		}
	}
}