using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Maxfire.Web.Mvc
{
	public class DynamicBag<TDictionary> : DynamicObject
		where TDictionary : IDictionary<string, object>
	{
		private readonly Func<TDictionary> _thunk;

		public DynamicBag(Func<TDictionary> thunk)
		{
			// we use a delegate thunk such that controllers and views 
			// can bind a closure to ViewData, TempDate etc. This way
			// we do not have to worry about TempData pointing to any 
			// new object
			_thunk = thunk;
		}

		private TDictionary Bag
		{
			get
			{
				TDictionary dictionary = _thunk();
				return dictionary;
			}
		}

		// Implementing this function improves the debugging experience as it provides the debugger with the list of all
		// the properties currently defined on the object
		public override IEnumerable<string> GetDynamicMemberNames()
		{
			return Bag.Keys;
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			Bag.TryGetValue(binder.Name, out result);
			// we always return a result even if the key does not exist
			return true;
		}

		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			Bag[binder.Name] = value;
			// you can always set a key in the dictionary so return true
			return true;
		}
	}

}