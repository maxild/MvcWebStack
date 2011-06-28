using System.Web.Routing;
using Maxfire.Web.Mvc.SimplyRestful;
using Maxfire.Web.Mvc.TestCommons.Routes;
using Xunit;

namespace Maxfire.Web.Mvc.UnitTests.SimplyRestful
{
	public class AreaRestfulRoutesSpecs : RoutesFixture
	{
		public override void RegisterRoutes(RouteCollection routes)
		{
			SimplyRestfulRouteHandler.BuildRoutes(routes, "admin/{controller}", SimplyRestfulRouteHandler.MatchPositiveInteger, null);
		}

		public class UrlGeneration : RoutesRegisteredBy<AreaRestfulRoutesSpecs>
		{
			[Fact]
			public void Index()
			{
				UrlGeneration
					.WithController("Login").AndAction("Index")
					.ShouldGenerateUriPathOf("/admin/login");
			}

			[Fact]
			public void Show()
			{
				UrlGeneration
					.WithController("Login").AndAction("Show").AndRouteValue("id", "10")
					.ShouldGenerateUriPathOf("/admin/login/10");
			}

			[Fact]
			public void New()
			{
				UrlGeneration
					.WithController("Login").AndAction("New")
					.ShouldGenerateUriPathOf("/admin/login/new");
			}

			[Fact]
			public void Edit()
			{
				UrlGeneration
					.WithController("Login").AndAction("Edit").AndRouteValue("id", "3")
					.ShouldGenerateUriPathOf("/admin/login/3/edit");
			}

			[Fact]
			public void Create()
			{
				UrlGeneration
					.WithController("Login").AndAction("Create")
					.ShouldGenerateUriPathOf("/admin/login");
			}

			[Fact]
			public void Update()
			{
				UrlGeneration
					.WithController("Login").AndAction("Update").AndRouteValue("id", "10")
					.ShouldGenerateUriPathOf("/admin/login/10");
			}

			[Fact]
			public void Destroy()
			{
				UrlGeneration
					.WithController("Login").AndAction("Destroy").AndRouteValue("id", "10")
					.ShouldGenerateUriPathOf("/admin/login/10");
			}
		}

		public class UrlRecognition : RoutesRegisteredBy<AreaRestfulRoutesSpecs>
		{
			[Fact]
			public void Index()
			{
				GetRequestFor("/admin/login")
					.ShouldMatchController("Login").AndAction("Index");
			}

			[Fact]
			public void Show()
			{
				GetRequestFor("/admin/login/6")
					.ShouldMatchController("Login").AndAction("Show").AndRouteValue("id", "6");
			}

			[Fact]
			public void New()
			{
				GetRequestFor("/admin/login/new")
					.ShouldMatchController("Login").AndAction("New");
			}

			[Fact]
			public void Edit()
			{
				GetRequestFor("/admin/login/5/edit")
					.ShouldMatchController("Login").AndAction("Edit").AndRouteValue("id", "5");
			}

			[Fact]
			public void Create()
			{
				PostRequestFor("/admin/login")
					.ShouldMatchController("Login").AndAction("Create");
			}

			[Fact]
			public void Update()
			{
				PutRequestFor("/admin/login/10")
					.ShouldMatchController("Login").AndAction("Update").AndRouteValue("id", "10");
			}

			[Fact]
			public void Update2()
			{
				PseudoPutRequestFor("/admin/login/10")
					.ShouldMatchController("Login").AndAction("Update").AndRouteValue("id", "10");
			}

			[Fact]
			public void Destroy()
			{
				DeleteRequestFor("/admin/login/11")
					.ShouldMatchController("Login").AndAction("Destroy").AndRouteValue("id", "11");
			}

			[Fact]
			public void Destroy2()
			{
				PseudoDeleteRequestFor("/admin/login/11")
					.ShouldMatchController("Login").AndAction("Destroy").AndRouteValue("id", "11");
			}
		}
	}
}