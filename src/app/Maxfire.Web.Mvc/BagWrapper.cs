using System;
using System.Collections.Generic;

namespace Maxfire.Web.Mvc
{
	public abstract class BagWrapper<TDictionary, TValue>
		where TDictionary : IDictionary<string, object>
	{
		private readonly Func<TDictionary> _thunk;
		private readonly string _key;

		protected BagWrapper(Func<TDictionary> thunk, string key)
		{
			// we use a delegate thunk such that controllers and views 
			// can bind a closure to ViewData, TempDate etc. This way
			// we do not have to worry about TempData pointing to any 
			// new object
			_thunk = thunk;
			_key = key;
		}

		private TDictionary Bag
		{
			get
			{
				TDictionary dictionary = _thunk();
				return dictionary;
			}
		}

		public TValue Value
		{
			get
			{
				object value;
				if (Bag.TryGetValue(_key, out value))
				{
					return (TValue)value;
				}
				return default(TValue);
			}
			set { Bag[_key] = value; }
		}

		public void RemoveValue()
		{
			if (Bag.ContainsKey(_key))
			{
				Bag.Remove(_key);
			}
		}
	}
}