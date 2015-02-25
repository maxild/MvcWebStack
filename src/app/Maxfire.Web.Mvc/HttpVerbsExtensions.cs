using System.Web.Mvc;

namespace Maxfire.Web.Mvc
{
	public static class HttpVerbsExtensions
	{
		public static FormMethod ToFormMethod(this HttpVerbs httpMethod)
		{
			switch (httpMethod)
			{
				case HttpVerbs.Get:
					return FormMethod.Get;
				default:
					return FormMethod.Post;
			}
		}

		public static bool FormNeedHttpMethodOverride(this HttpVerbs httpMethod)
		{
			switch (httpMethod)
			{
				case HttpVerbs.Get:
				case HttpVerbs.Post:
					return false;
				default:
					return true;
			}
		}
	}
}