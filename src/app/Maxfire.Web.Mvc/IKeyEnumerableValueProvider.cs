using System.Collections.Generic;
using System.Web.Mvc;

namespace Maxfire.Web.Mvc
{
	public interface IKeyEnumerableValueProvider : IValueProvider
	{
		IEnumerable<string> GetKeys();
	}
}