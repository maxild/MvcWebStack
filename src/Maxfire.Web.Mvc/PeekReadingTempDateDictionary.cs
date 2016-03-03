using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Maxfire.Web.Mvc
{
    /// <summary>
    /// By accessing TempData using this wrapper, TempData will surely be persisted
    /// (Normally reads will remove temp data, therefore we use Peek below)
    /// </summary>
    public class PeekReadingTempDateDictionary : IDictionary<string, object>
    {
        private readonly TempDataDictionary _tempData;

        public PeekReadingTempDateDictionary(TempDataDictionary tempData)
        {
            _tempData = tempData;
        }

        private IDictionary<string, object> Bag
        {
            get { return _tempData; }
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _tempData.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
        {
            Bag.Add(item);
        }

        public void Clear()
        {
            _tempData.Clear();
        }

        bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
        {
            return Bag.Contains(item);
        }

        void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            Bag.CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
        {
            return Bag.Remove(item);
        }

        public int Count
        {
            get { return _tempData.Count; }
        }

        bool ICollection<KeyValuePair<string, object>>.IsReadOnly
        {
            get { return Bag.IsReadOnly; }
        }

        public bool ContainsKey(string key)
        {
            return _tempData.ContainsKey(key);
        }

        public void Add(string key, object value)
        {
            _tempData.Add(key, value);
        }

        public bool Remove(string key)
        {
            return _tempData.Remove(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            bool found = _tempData.ContainsKey(key);
            value = _tempData.Peek(key);
            return found;
        }

        public object this[string key]
        {
            get { return _tempData.Peek(key); }
            set { _tempData[key] = value; }
        }

        public ICollection<string> Keys
        {
            get { return _tempData.Keys; }
        }

        public ICollection<object> Values
        {
            get { return _tempData.Values; }
        }
    }
}
