using System;

namespace Maxfire.Web.Mvc
{
	public interface INameValueSerializerProvider
	{
		INameValueSerializer GetSerializer(Type modelType);
	}
}