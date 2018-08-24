using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;
using JetBrains.Annotations;
using Maxfire.TestCommons.AssertExtensions;
using Xunit;

namespace Maxfire.Web.Mvc.UnitTests
{
	[UsedImplicitly]
	[SuppressMessage("ReSharper", "UnusedMethodReturnValue.Local")]
	public class RedirectToActionTester
	{
		[Fact]
		public void RedirectToActionOnSameController()
		{
			var controller = new FirstController();

			var result = controller.RedirectToAction(x => x.Index()) as RedirectToRouteResult;

            Assert.NotNull(result);
			result.RouteValues.GetRequiredString("Controller").ShouldEqual("First");
			result.RouteValues.GetRequiredString("Action").ShouldEqual("Index");
		}

		[Fact]
		public void RedirectToActionOnDifferentController()
		{
			var controller = new SecondController();

			var result = controller.RedirectToAction<FirstController>(x => x.Index()) as RedirectToRouteResult;

		    Assert.NotNull(result);
			result.RouteValues.GetRequiredString("Controller").ShouldEqual("First");
			result.RouteValues.GetRequiredString("Action").ShouldEqual("Index");
		}

		[Fact]
		public void RedirectToActionInheritedFromSuperLayerControllerOnSameController()
		{
			var controller = new FirstController();

			var result = controller.Show() as RedirectToRouteResult;

		    Assert.NotNull(result);
			result.RouteValues.GetRequiredString("Controller").ShouldEqual("First");
			result.RouteValues.GetRequiredString("Action").ShouldEqual("Show");
		}


		[Fact]
		public void ExampleOfCommonErrorWhenUsingRedirectToAction()
		{
			var controller = new AnotherController();

			var result = controller.ShowWithFailureRedirectToAction() as RedirectToRouteResult;

		    Assert.NotNull(result);
			result.RouteValues.GetRequiredString("Controller").ShouldEqual("SuperLayer"); // <-- ERROR (Expected: Another) -->
			result.RouteValues.GetRequiredString("Action").ShouldEqual("ShowWithFailureRedirectToAction");
		}

		[Fact]
		public void RedirectToActionInheritedFromSuperLayerControllerOnDifferentController()
		{
			var controller = new SecondController();

			var result = controller.Create() as RedirectToRouteResult;

		    Assert.NotNull(result);
			result.RouteValues.GetRequiredString("Controller").ShouldEqual("First");
			result.RouteValues.GetRequiredString("Action").ShouldEqual("Show");
		}

		// It is important to have a TController generic argument in order to be able
		// to use RedirectToAction from within the super layer controller type, because
		// otherwise the controller name will
		class SuperLayerController<TController> : OpinionatedController
			where TController : SuperLayerController<TController>
		{
			public ActionResult Show()
			{
				return this.RedirectToAction<TController>(x => x.Show());
			}
		}
		class SuperLayerController : OpinionatedController
		{
			public ActionResult ShowWithFailureRedirectToAction()
			{
				return this.RedirectToAction(x => x.ShowWithFailureRedirectToAction());
			}
		}
		class FirstController : SuperLayerController<FirstController>
		{
			public ActionResult Index()
			{
				return this.RedirectToAction(x => x.Index());
			}
		}
		class SecondController : OpinionatedController
		{
			public ActionResult Create()
			{
				return this.RedirectToAction<FirstController>(x => x.Show());
			}
		}
		class AnotherController : SuperLayerController {}
	}
}
