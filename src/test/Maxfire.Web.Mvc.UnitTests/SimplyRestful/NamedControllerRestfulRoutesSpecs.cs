using System.Web.Routing;
using Maxfire.Web.Mvc.SimplyRestful;
using Maxfire.Web.Mvc.TestCommons.Routes;
using Xunit;

namespace Maxfire.Web.Mvc.UnitTests.SimplyRestful
{
	public class NamedControllerRestfulRoutesSpecs : RoutesFixture
	{
		public override void RegisterRoutes(RouteCollection routes)
		{
			SimplyRestfulRouteHandler.BuildRoutes(routes, "prices", SimplyRestfulRouteHandler.MatchPositiveInteger, "Priser");
		}

		public class UrlGeneration : RoutesFixtureBase, IUseFixture<NamedControllerRestfulRoutesSpecs>
		{
			public void SetFixture(NamedControllerRestfulRoutesSpecs fixture)
			{
				Fixture = fixture;
			}

			[Fact]
			public void Index()
			{
				UrlGeneration
					.WithController("Priser").AndAction("Index")
					.ShouldGenerateUriPathOf("/prices");
			}

			[Fact]
			public void Show()
			{
				UrlGeneration
					.WithController("Priser").AndAction("Show").AndRouteValue("id", "10")
					.ShouldGenerateUriPathOf("/prices/10");
			}

			[Fact]
			public void New()
			{
				UrlGeneration
					.WithController("Priser").AndAction("New")
					.ShouldGenerateUriPathOf("/prices/new");
			}

			[Fact]
			public void Edit()
			{
				UrlGeneration
					.WithController("Priser").AndAction("Edit").AndRouteValue("id", "3")
					.ShouldGenerateUriPathOf("/prices/3/edit");
			}

			[Fact]
			public void Create()
			{
				UrlGeneration
					.WithController("Priser").AndAction("Create")
					.ShouldGenerateUriPathOf("/prices");
			}

			[Fact]
			public void Update()
			{
				UrlGeneration
					.WithController("Priser").AndAction("Update").AndRouteValue("id", "10")
					.ShouldGenerateUriPathOf("/prices/10");
			}

			[Fact]
			public void Destroy()
			{
				UrlGeneration
					.WithController("Priser").AndAction("Destroy").AndRouteValue("id", "10")
					.ShouldGenerateUriPathOf("/prices/10");
			}
		}

		public class UrlRecognition : RoutesFixtureBase, IUseFixture<NamedControllerRestfulRoutesSpecs>
		{
			public void SetFixture(NamedControllerRestfulRoutesSpecs fixture)
			{
				Fixture = fixture;
			}

			[Fact]
			public void Index()
			{
				GetRequestFor("/prices")
					.ShouldMatchController("Priser").AndAction("Index");
			}

			[Fact]
			public void Show()
			{
				GetRequestFor("/prices/6")
					.ShouldMatchController("Priser").AndAction("Show").AndRouteValue("id", "6");
			}

			[Fact]
			public void New()
			{
				GetRequestFor("/prices/new")
					.ShouldMatchController("Priser").AndAction("New");
			}

			[Fact]
			public void Edit()
			{
				GetRequestFor("/prices/5/edit")
					.ShouldMatchController("Priser").AndAction("Edit").AndRouteValue("id", "5");
			}

			[Fact]
			public void Create()
			{
				PostRequestFor("/prices")
					.ShouldMatchController("Priser").AndAction("Create");
			}

			[Fact]
			public void Update()
			{
				PutRequestFor("/prices/10")
					.ShouldMatchController("Priser").AndAction("Update").AndRouteValue("id", "10");
			}

			[Fact]
			public void Update2()
			{
				PseudoPutRequestFor("/prices/10")
					.ShouldMatchController("Priser").AndAction("Update").AndRouteValue("id", "10");
			}

			[Fact]
			public void Destroy()
			{
				DeleteRequestFor("/prices/11")
					.ShouldMatchController("Priser").AndAction("Destroy").AndRouteValue("id", "11");
			}

			[Fact]
			public void Destroy2()
			{
				PseudoDeleteRequestFor("/prices/11")
					.ShouldMatchController("Priser").AndAction("Destroy").AndRouteValue("id", "11");
			}
		}
	}
}