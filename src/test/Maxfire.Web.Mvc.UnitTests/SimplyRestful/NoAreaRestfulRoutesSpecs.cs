using System.Web.Routing;
using Maxfire.Web.Mvc.SimplyRestful;
using Maxfire.Web.Mvc.TestCommons.Routes;
using Xunit;

namespace Maxfire.Web.Mvc.UnitTests.SimplyRestful
{
	public class NoAreaRestfulRoutesSpecs : RoutesFixture
	{
		public override void RegisterRoutes(RouteCollection routes)
		{
			SimplyRestfulRouteHandler.BuildRoutes(routes);
		}

		public class UrlGeneration : RoutesFixtureBase, IUseFixture<NoAreaRestfulRoutesSpecs>
		{
			public void SetFixture(NoAreaRestfulRoutesSpecs fixture)
			{
				Fixture = fixture;
			}

			[Fact]
			public void Index()
			{
				UrlGeneration
					.WithController("Users").AndAction("Index")
					.ShouldGenerateUriPathOf("/users");
			}

			[Fact]
			public void Show()
			{
				UrlGeneration
					.WithController("Users").AndAction("Show").AndRouteValue("id", "10")
					.ShouldGenerateUriPathOf("/users/10");
			}

			[Fact]
			public void New()
			{
				UrlGeneration
					.WithController("Users").AndAction("New")
					.ShouldGenerateUriPathOf("/users/new");
			}

			[Fact]
			public void Edit()
			{
				UrlGeneration
					.WithController("Users").AndAction("Edit").AndRouteValue("id", "3")
					.ShouldGenerateUriPathOf("/users/3/edit");
			}

			[Fact]
			public void Create()
			{
				UrlGeneration
					.WithController("Users").AndAction("Create")
					.ShouldGenerateUriPathOf("/users");
			}

			[Fact]
			public void Update()
			{
				UrlGeneration
					.WithController("Users").AndAction("Update").AndRouteValue("id", "10")
					.ShouldGenerateUriPathOf("/users/10");
			}

			[Fact]
			public void Destroy()
			{
				UrlGeneration
					.WithController("Users").AndAction("Destroy").AndRouteValue("id", "10")
					.ShouldGenerateUriPathOf("/users/10");
			}
		}

		public class UrlRecognition : RoutesFixtureBase, IUseFixture<NoAreaRestfulRoutesSpecs>
		{
			public void SetFixture(NoAreaRestfulRoutesSpecs fixture)
			{
				Fixture = fixture;
			}

			[Fact]
			public void Index()
			{
				GetRequestFor("/users")
					.ShouldMatchController("Users").AndAction("Index");
			}

			[Fact]
			public void Show()
			{
				GetRequestFor("/users/6")
					.ShouldMatchController("Users").AndAction("Show").AndRouteValue("id", "6");
			}

			[Fact]
			public void New()
			{
				GetRequestFor("/users/new")
					.ShouldMatchController("Users").AndAction("New");
			}

			[Fact]
			public void Edit()
			{
				GetRequestFor("/users/5/edit")
					.ShouldMatchController("Users").AndAction("Edit").AndRouteValue("id", "5");
			}

			[Fact]
			public void Create()
			{
				PostRequestFor("/users")
					.ShouldMatchController("Users").AndAction("Create");
			}

			[Fact]
			public void Update()
			{
				PutRequestFor("/users/10")
					.ShouldMatchController("Users").AndAction("Update").AndRouteValue("id", "10");
			}

			[Fact]
			public void Update2()
			{
				PseudoPutRequestFor("/users/10")
					.ShouldMatchController("Users").AndAction("Update").AndRouteValue("id", "10");
			}

			[Fact]
			public void Destroy()
			{
				DeleteRequestFor("/users/11")
					.ShouldMatchController("Users").AndAction("Destroy").AndRouteValue("id", "11");
			}

			[Fact]
			public void Destroy2()
			{
				PseudoDeleteRequestFor("/users/11")
					.ShouldMatchController("Users").AndAction("Destroy").AndRouteValue("id", "11");
			}
		}
	}
}