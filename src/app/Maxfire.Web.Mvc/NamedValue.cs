using System;
using System.Collections.Generic;

namespace Maxfire.Web.Mvc
{
	/// <summary>
	/// Wrapper around a named key-value pair in either ViewData or TempData.
	/// </summary>
	public class NamedValue<TValue>
	{
		private readonly string _key;
		private readonly Func<IDictionary<string, object>> _dictionaryAccessor;

		public NamedValue(string key, Func<IDictionary<string, object>> dictionaryAccessor)
		{
			// we use a delegate thunk such that controllers and views 
			// can bind a closure to ViewData, TempDate etc. This way
			// we do not have to worry about TempData pointing to any 
			// new object
			_dictionaryAccessor = dictionaryAccessor;
			_key = key;
		}

		private IDictionary<string, object> Bag
		{
			get
			{
				IDictionary<string, object> dictionary = _dictionaryAccessor();
				return dictionary;
			}
		}

		/// <summary>
		/// Access og mutate the value
		/// </summary>
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

		/// <summary>
		/// Remove the value
		/// </summary>
		public void Delete()
		{
			DeleteFrom(Bag);
		}

		private void DeleteFrom(IDictionary<string, object> bag)
		{
			if (bag.ContainsKey(_key))
			{
				bag.Remove(_key);
			}
		}
	}
}