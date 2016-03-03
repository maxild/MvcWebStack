using System.Collections.Generic;

namespace Maxfire.Web.Mvc
{
	public abstract class SimpleNameValueSerializer<T> : INameValueSerializer
	{
		public IDictionary<string, object> GetValues(object model, string prefix)
		{
			if (model == null || model.GetType() != typeof(T))
			{
				return new Dictionary<string, object>();
			}

			return GetValuesCore((T)model, prefix);
		}

		protected abstract IDictionary<string, object> GetValuesCore(T value, string prefix);
	}
}