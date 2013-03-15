using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Maxfire.Web.Mvc.ValueProviders
{
	public class BetterValueProviderCollection : ValueProviderCollection, IKeyEnumerableValueProvider
	{
		public virtual IEnumerable<string> GetKeys()
		{
			return (from provider in this.OfType<IKeyEnumerableValueProvider>()
			        let keys = provider.GetKeys()
			        where keys != null
			        select keys).Aggregate(Enumerable.Empty<string>(), (uniqueKeys, keys) => uniqueKeys.Union(keys));
		}
	}
}