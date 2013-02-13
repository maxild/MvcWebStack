using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Maxfire.Web.Mvc.Html.Extensions;

namespace Maxfire.Web.Mvc
{
	public static class HtmlHelperExtensions
	{
		public static IDisposable BeginHtmlFieldPrefixScope(this HtmlHelper html, string htmlFieldPrefix)
		{
			return new HtmlFieldPrefixScope(html.ViewData.TemplateInfo, htmlFieldPrefix);
		}

		public static IDisposable BeginCollectionFor<TModel, TValue>(this HtmlHelper<TModel> html,
		                                                                         Expression<Func<TModel, TValue>> expression)
		{
			string collectionName = expression.GetHtmlFieldNameFor(html.ViewContext.ViewData);
			return html.BeginCollection(collectionName);
		}

		public static IDisposable BeginCollection(this HtmlHelper html, string collectionName)
		{
			return new CollectionNameScope(html.ViewContext.HttpContext, collectionName);
		}

		public static IDisposable BeginCollectionItem(this HtmlHelper htmlHelper, string collectionName = null)
		{
			collectionName = collectionName ?? htmlHelper.GetCollectionName();
			string itemIndex = htmlHelper.GetCollectionIndexStore().GetNextItemIndex(collectionName); // GUID keys
			return htmlHelper.BeginCollectionItemHelper(collectionName, itemIndex);
		}

		public static IDisposable BeginCollectionItem<TModel>(this HtmlHelper<TModel> htmlHelper,
		                                                      Func<TModel, object> keyAccesor = null,
		                                                      string collectionName = null)
		{
			collectionName = collectionName ?? htmlHelper.GetCollectionName();

			// We need to use the same sequence of IDs following a server-side validation failure,  
			// otherwise the framework won't render the validation error messages next to each item.
			string itemIndex = keyAccesor != null
				? Convert.ToString(keyAccesor(htmlHelper.ViewData.Model), CultureInfo.InvariantCulture)  // natural key
				: htmlHelper.GetCollectionIndexStore().GetNextItemIndex(collectionName);                 // GUID keys

			return htmlHelper.BeginCollectionItemHelper(collectionName, itemIndex);
		}

		private static IDisposable BeginCollectionItemHelper(this HtmlHelper htmlHelper, string collectionName, string itemIndex)
		{
			collectionName = collectionName ?? htmlHelper.GetCollectionName();

			// autocomplete="off" is needed to work around a very annoying Chrome behaviour whereby it reuses old values after the user clicks "Back", which causes the xyz.index and xyz[...] values to get out of sync.
			const string HIDDEN_ITEM_INDEX = "<input type=\"hidden\" name=\"{0}.index\" autocomplete=\"off\" value=\"{1}\" />";

			// Insert hidden field with collectionName.index = itemIndex
			htmlHelper.ViewContext.Writer.WriteLine(HIDDEN_ITEM_INDEX, collectionName, htmlHelper.Encode(itemIndex));

			return htmlHelper.BeginHtmlFieldPrefixScope(string.Format("{0}[{1}]", collectionName, itemIndex));
		}

		private static CollectionIndexStore GetCollectionIndexStore(this HtmlHelper htmlHelper)
		{
			const string COLLECTION_INDEX_STORE_KEY = "__collectionIndexStore";
			var collectionIndexStore = (CollectionIndexStore) htmlHelper.ViewContext.HttpContext.Items[COLLECTION_INDEX_STORE_KEY];
			if (collectionIndexStore == null)
			{
				htmlHelper.ViewContext.HttpContext.Items[COLLECTION_INDEX_STORE_KEY] =
					collectionIndexStore = new CollectionIndexStore(htmlHelper.ViewContext.ViewData.ModelState);
			}
			return collectionIndexStore;
		}

		private static string GetCollectionName(this HtmlHelper htmlHelper)
		{
			return CollectionNameScope.GetCollectionName(htmlHelper.ViewContext.HttpContext);
		}

		class CollectionNameScope : IDisposable
		{
			const string COLLECTION_NAME_KEY = "__collectionNameScope";
			private readonly HttpContextBase _httpContext;
			private readonly string _previousCollectionName;

			public static string GetCollectionName(HttpContextBase httpContext)
			{
				return httpContext.Items[COLLECTION_NAME_KEY] as string;
			}

			static void SetCollectionName(HttpContextBase httpContext, string collectionName)
			{
				httpContext.Items[COLLECTION_NAME_KEY] = collectionName;
			}

			public CollectionNameScope(HttpContextBase httpContext, string collectionName)
			{
				_previousCollectionName = GetCollectionName(httpContext);
				SetCollectionName(httpContext, collectionName);
				_httpContext = httpContext;
			}

			public void Dispose()
			{
				SetCollectionName(_httpContext, _previousCollectionName);
			}
		}

		class HtmlFieldPrefixScope : IDisposable
		{
			private readonly TemplateInfo _templateInfo;
			private readonly string _previousHtmlFieldPrefix;

			public HtmlFieldPrefixScope(TemplateInfo templateInfo, string htmlFieldPrefix)
			{
				_templateInfo = templateInfo;
				_previousHtmlFieldPrefix = templateInfo.HtmlFieldPrefix;
				templateInfo.HtmlFieldPrefix = htmlFieldPrefix;
			}

			public void Dispose()
			{
				_templateInfo.HtmlFieldPrefix = _previousHtmlFieldPrefix;
			}
		}

		public static string SiteResource(this HtmlHelper htmlHelper, string path)
		{
			IUrlHelper urlHelper = htmlHelper.ViewContext.View as IUrlHelper;
			if (urlHelper != null)
			{
				return urlHelper.SiteResource(path);
			}
			return UrlHelper.GenerateContentUrl(path, htmlHelper.ViewContext.HttpContext);
		}
	}
}