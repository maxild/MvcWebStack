using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Maxfire.Core.Extensions;

namespace Maxfire.Web.Mvc
{
	/// <summary>
	/// Helper class to access any posted 'collectionName.index' values that must be recycled
	/// on any page when rendering form elements in order for validation feedback to work.
	/// (see also http://haacked.com/archive/2008/10/23/model-binding-to-a-list.aspx and
	/// http://www.hanselman.com/blog/ASPNETWireFormatForModelBindingToArraysListsCollectionsDictionaries.aspx).
	/// An instance of this class should be stored in the HTTP context (lifetime = request).
	/// </summary>
	/// <remarks>
	/// It is VERY important to use the BetterDefaultModelBinder to bind to collections, otherwise
	/// modelstate will not contain the posted 'collectionName.index' comma-separated value that 
	/// needs to be recycled to the view.
	/// </remarks>
	public class CollectionIndexStore
	{
		private readonly Dictionary<string, Queue<string>> _queues;
		private readonly ModelStateDictionary _modelStateDictionary;

		public CollectionIndexStore(ModelStateDictionary modelStateDictionary)
		{
			_queues = new Dictionary<string, Queue<string>>(StringComparer.OrdinalIgnoreCase);
			_modelStateDictionary = modelStateDictionary;
		}

		public string GetNextItemIndex(string collectionName)
		{
			var indexesToReuse = GetQueue(collectionName);
			// we remove reused id from the beginning of the queue (FIFO)
			string itemIndex = indexesToReuse.Count > 0 ? indexesToReuse.Dequeue() : Guid.NewGuid().ToString();
			return itemIndex;
		}

		private Queue<string> GetQueue(string collectionName)
		{
			return _queues.GetOrCreate(collectionName, () => GetCollectionIndexesToReuse(collectionName));
		}

		/// <summary>
		/// Get any previously used item indices for a named collection
		/// </summary>
		private Queue<string> GetCollectionIndexesToReuse(string collectionName)
		{
			Queue<string> indexesToReuse = null;
			string indexesKey = collectionName + ".index";
			ModelState modelState;
			if (_modelStateDictionary.TryGetValue(indexesKey, out modelState) && modelState.Value != null)
			{
				string[] indexes = modelState.Value.ConvertTo(typeof (string[])) as string[] ??
				                   TryParseCommaSeparated(modelState.Value.AttemptedValue);
				if (indexes != null)
				{
					indexesToReuse = new Queue<string>(indexes.Length);
					foreach (string itemIndex in indexes)
					{
						// we insert items to the end of the queue (FIFO)
						indexesToReuse.Enqueue(itemIndex);
					}
				}
			}
			return indexesToReuse ?? new Queue<string>();
		}

		private static string[] TryParseCommaSeparated(string s)
		{
			return !string.IsNullOrEmpty(s) ? s.Split(',') : null;
		}
	}
}