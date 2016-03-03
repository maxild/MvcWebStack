using System.Collections.Generic;

namespace Maxfire.TestCommons.AssertExtensions
{
	public static class DictionaryAssertExtensions
	{
		public static IDictionary<TKey, TValue> ShouldContainKey<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
		{
			dictionary.ContainsKey(key).ShouldBeTrue();
			return dictionary;
		}

		public static IDictionary<TKey, TValue> ShouldNotContainKey<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
		{
			dictionary.ContainsKey(key).ShouldBeFalse();
			return dictionary;
		}
	}
}