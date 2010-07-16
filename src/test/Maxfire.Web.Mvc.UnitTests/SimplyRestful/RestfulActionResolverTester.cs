using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Routing;
using Maxfire.TestCommons.AssertExtensions;
using Maxfire.Web.Mvc.SimplyRestful;
using Rhino.Mocks;
using Xunit;

namespace Maxfire.Web.Mvc.UnitTests.SimplyRestful
{
	public class RestfulActionResolverTester
	{
		private readonly HttpContextBase _httpContext;
		private readonly HttpRequestBase _httpRequest;
		private readonly IRestfulActionResolver _resolver;
		private RouteData _routeData;
		private NameValueCollection _form;
		private RequestContext _requestContext;
		
		public RestfulActionResolverTester()
		{
			_httpContext = MockRepository.GenerateStub<HttpContextBase>();
			_httpRequest = MockRepository.GenerateStub<HttpRequestBase>();
			_resolver = new RestfulActionResolver();
		}

		[Fact]
		public void ResolveAction_WithNullRequest_Throws()
		{
			_httpContext.Stub(r => r.Request).Return(null);
			_requestContext = new RequestContext(_httpContext, new RouteData());

			Assert.Throws<NullReferenceException>(() => _resolver.ResolveAction(_requestContext));
		}

		[Fact]
		public void ResolveAction_WithEmptyRequestHttpMethod_ReturnsNoAction()
		{
			GivenContext("", null);
			_resolver.ResolveAction(_requestContext).ShouldEqual(RestfulAction.None);
		}

		[Fact]
		public void ResolveAction_WithNonPostRequest_ReturnsNoAction()
		{
			GivenContext("GET", null);
			_resolver.ResolveAction(_requestContext).ShouldEqual(RestfulAction.None);
		}

		[Fact]
		public void ResolveAction_WithPostRequestAndNullForm_ReturnsNoAction()
		{
			GivenContext("POST", null);
			_resolver.ResolveAction(_requestContext).ShouldEqual(RestfulAction.None);
		}

		[Fact]
		public void ResolveAction_WithPostRequestAndEmptyFormMethodValue_ReturnsNoAction()
		{
			GivenContext("POST", "");
			_resolver.ResolveAction(_requestContext).ShouldEqual(RestfulAction.None);
		}

		[Fact]
		public void ResolveAction_WithPostRequestAndInvalidFormMethodValue_ReturnsNoAction()
		{
			GivenContext("POST", "GOOSE");
			_resolver.ResolveAction(_requestContext).ShouldEqual(RestfulAction.None);
		}

		[Fact]
		public void ResolveAction_WithPostRequestAndFormMethodValuePUT_ReturnsUpdateAction()
		{
			GivenContext("POST", "PUT");
			_resolver.ResolveAction(_requestContext).ShouldEqual(RestfulAction.Update);
		}

		[Fact]
		public void ResolveAction_WithPostRequestAndFormMethodValueDELETE_ReturnsDestroyAction()
		{
			GivenContext("POST", "DELETE");
			_resolver.ResolveAction(_requestContext).ShouldEqual(RestfulAction.Destroy);
		}

		private void GivenContext(string httpMethod, string formMethod)
		{
			_httpContext.Stub(c => c.Request).Return(_httpRequest).Repeat.Any();
			_httpRequest.Stub(r => r.HttpMethod).Return(httpMethod).Repeat.Any();

			_routeData = new RouteData();
			_routeData.Values.Add("controller", "FooController");
			_routeData.Values.Add("action", "SomeWeirdAction");

			if (formMethod != null)
			{
				_form = new NameValueCollection { { "_method", formMethod } };
				_httpRequest.Stub(r => r.Form).Return(_form).Repeat.Any();
			}

			_requestContext = new RequestContext(_httpContext, _routeData);
		}
	}
}