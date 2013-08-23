using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;

namespace Maxfire.Web.Mvc
{
	public class DynamicNameValueCollection: DynamicObject
	{
		private readonly Func<NameValueCollection> _thunk;

		public DynamicNameValueCollection(Func<NameValueCollection> thunk)
		{
			// we use a delegate thunk such that controllers and views 
			// can bind a closure to Forms, QueryString etc.
			_thunk = thunk;
		}

		private NameValueCollection Bag
		{
			get
			{
				NameValueCollection collection = _thunk();
				return collection;
			}
		}

		// Implementing this function improves the debugging experience as it provides the debugger with the list of all
		// the properties currently defined on the object
		public override IEnumerable<string> GetDynamicMemberNames()
		{
			return Bag.AllKeys;
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			result = Bag[binder.Name];
			// we always return a result even if the key does not exist
			return true;
		}

		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			return false;
		}
	}
}