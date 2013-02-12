using System.Collections.Generic;
using System.Linq;
using System.Web;
using Maxfire.Core.Extensions;

namespace Maxfire.Web.Mvc
{
	public class RequestDataProvider : IDataProvider<string>
	{
		private readonly HttpRequestBase _request;

		public RequestDataProvider(HttpRequestBase request)
		{
			_request = request;
		}

		public string GetData(string key)
		{
			return _request[key];
		}
	}

	public class DictionaryDataProvider : IDataProvider<string>
	{
		const string IDS_TO_REUSE_KEY = "__collectionIndex_IdsToReuse_";

		private readonly IDictionary<string, object> _dictionary;

		public DictionaryDataProvider(IDictionary<string, object> dictionary)
		{
			_dictionary = dictionary;
		}

		public string GetData(string key)
		{
			return _dictionary.GetValueOrDefault(IDS_TO_REUSE_KEY + key) as string;
		}
	}

	public class CompositeDataProvider : IDataProvider<string>
	{
		private readonly IEnumerable<IDataProvider<string>> _dataProviders;

		public CompositeDataProvider(IEnumerable<IDataProvider<string>> dataProviders)
		{
			_dataProviders = dataProviders;
		}

		public string GetData(string key)
		{
			return _dataProviders.Map(provider => provider.GetData(key)).FirstOrDefault(data => data != null);
		}
	}
}