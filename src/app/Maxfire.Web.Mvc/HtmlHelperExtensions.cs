using System;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Maxfire.Web.Mvc.Html.Extensions;

namespace Maxfire.Web.Mvc
{
	public static class HtmlHelperExtensions
	{
		const string COLLECTION_NAME_KEY = "__collectionNameScope";

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
			const string IDS_TO_REUSE_KEY = "__collectionIndexRenderer_IdsToReuse";

			// TODO: Maybe better to create CollectionIndex(Store) helper for each collectionName
			// We need to use the same sequence of IDs following a server-side validation failure,  
			// otherwise the framework won't render the validation error messages next to each item.
			var collectionIndexStore = (CollectionIndexStore)htmlHelper.ViewContext.HttpContext.Items[IDS_TO_REUSE_KEY];
			if (collectionIndexStore == null)
			{
				// TODO: collectionIndices is stored in request params (this should be configurable)
				var collectionIndicesProvider = new RequestDataProvider(htmlHelper.ViewContext.HttpContext.Request);
				htmlHelper.ViewContext.HttpContext.Items[IDS_TO_REUSE_KEY] =
					collectionIndexStore = new CollectionIndexStore(collectionIndicesProvider);
			}

			collectionName = collectionName ?? htmlHelper.GetCollectionName();
			string itemIndex = collectionIndexStore.GetNextItemIndex(collectionName);

			// autocomplete="off" is needed to work around a very annoying Chrome behaviour whereby it reuses old values after the user clicks "Back", which causes the xyz.index and xyz[...] values to get out of sync.
			const string HIDDEN_ITEM_INDEX = "<input type=\"hidden\" name=\"{0}.index\" autocomplete=\"off\" value=\"{1}\" />";

			// Insert hidden field with collectionName.index = itemIndex
			htmlHelper.ViewContext.Writer.WriteLine(HIDDEN_ITEM_INDEX, collectionName, htmlHelper.Encode(itemIndex));

			return htmlHelper.BeginHtmlFieldPrefixScope(string.Format("{0}[{1}]", collectionName, itemIndex));
		}

		private static string GetCollectionName(this HtmlHelper htmlHelper)
		{
			return htmlHelper.ViewContext.HttpContext.Items[COLLECTION_NAME_KEY] as string;
		}

		class CollectionNameScope : IDisposable
		{
			private readonly HttpContextBase _httpContext;
			private readonly string _previousCollectionName;

			public CollectionNameScope(HttpContextBase httpContext, string collectionName)
			{
				_previousCollectionName = httpContext.Items[COLLECTION_NAME_KEY] as string;
				httpContext.Items[COLLECTION_NAME_KEY] = collectionName;
				_httpContext = httpContext;
			}

			public void Dispose()
			{
				_httpContext.Items[COLLECTION_NAME_KEY] = _previousCollectionName;
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