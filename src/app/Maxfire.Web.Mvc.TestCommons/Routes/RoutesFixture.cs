using System.Web.Routing;

namespace Maxfire.Web.Mvc.TestCommons.Routes
{
	/// <summary>
	/// Combines a route table and a NameValueSerializer (used to serialize captured action argumnets to 
	/// route values before generating urls)
	/// </summary>
	public abstract class RoutesFixture
	{
		protected RoutesFixture(INameValueSerializer nameValueSerializer = null)
		{
			_nameValueSerializer = nameValueSerializer;
		}

		private RouteCollection _routes;
		public RouteCollection Routes
		{
			get { return _routes ?? (_routes = GetRoutes()); }
		}

		private RouteCollection GetRoutes()
		{
			var routes = new RouteCollection();
			RegisterRoutes(routes);
			return routes;
		}

		protected abstract void RegisterRoutes(RouteCollection routes);

		private INameValueSerializer _nameValueSerializer;
		public INameValueSerializer NameValueSerializer
		{
			get { return _nameValueSerializer ?? (_nameValueSerializer = GetNameValueSerializer()); }
		}

		protected virtual INameValueSerializer GetNameValueSerializer()
		{
			return new DefaultNameValueSerializer();
		}
	}
}