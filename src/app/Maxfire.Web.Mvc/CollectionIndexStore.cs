using System;
using System.Collections.Generic;
using Maxfire.Core.Extensions;

namespace Maxfire.Web.Mvc
{
	/// <summary>
	/// Helper class to access any posted 'collectionName.index' values that must be recycled
	/// on any page when rendering form elements in order for validation feedback to work.
	/// (see also http://haacked.com/archive/2008/10/23/model-binding-to-a-list.aspx).
	/// An instance of this class should be stored in the HTTP context (lifetime = request).
	/// </summary>
	public class CollectionIndexStore
	{
		private readonly Dictionary<string, Queue<string>> _queues;
		private readonly IDataProvider<string> _collectionIndexProvider;

		public CollectionIndexStore(IDataProvider<string> collectionIndexProvider)
		{
			_queues = new Dictionary<string, Queue<string>>(StringComparer.OrdinalIgnoreCase);
			_collectionIndexProvider = collectionIndexProvider;
		}

		public string GetNextItemIndex(string collectionName)
		{
			var idsToReuse = GetQueue(collectionName);
			// we remove reused id from the beginning of the queue (FIFO)
			string itemIndex = idsToReuse.Count > 0 ? idsToReuse.Dequeue() : Guid.NewGuid().ToString();
			return itemIndex;
		}

		private Queue<string> GetQueue(string collectionName)
		{
			return _queues.GetOrCreate(collectionName, () => GetIdsToReuse(collectionName));
		}

		/// <summary>
		/// Get any previously used item indices for a named collection
		/// </summary>
		private Queue<string> GetIdsToReuse(string collectionName)
		{
			Queue<string> queue = null;
			string collectionIndexString = _collectionIndexProvider.GetData(collectionName + ".index");
			if (!string.IsNullOrEmpty(collectionIndexString))
			{
				var collectionIndexArray = collectionIndexString.Split(',');
				queue = new Queue<string>(collectionIndexArray.Length);
				foreach (string itemIndex in collectionIndexArray)
				{
					// we insert items to the end of the queue (FIFO)
					queue.Enqueue(itemIndex);
				}
			}
			return queue ?? new Queue<string>();
		}

	}
}