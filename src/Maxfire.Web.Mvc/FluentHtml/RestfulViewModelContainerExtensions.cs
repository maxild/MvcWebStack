using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using JetBrains.Annotations;
using Maxfire.Core.Extensions;
using Maxfire.Web.Mvc.FluentHtml.Html;

namespace Maxfire.Web.Mvc.FluentHtml
{
	[UsedImplicitly]
	public static class RestfulViewModelContainerExtensions
	{
		[UsedImplicitly]
		public static HtmlFormDescriptor BeginForm<TController>(this IUrlResponseWriter view, Expression<Action<TController>> action, FormMethod method)
			where TController : Controller
		{
			// Todo: Create HtmlTag.Form, HtmlAttribute.Action and Method
			//TagBuilder tagBuilder = new TagBuilder("form");

			string url = view.UrlFor(action).ToString();
			string methodAsString = HtmlHelper.GetFormMethodString(method);

			//tagBuilder.MergeAttribute("action", url);
			//tagBuilder.MergeAttribute("method", methodAsString, true);

			//view.Render(tagBuilder.ToString(TagRenderMode.StartTag));
			//return new HtmlFormEndTagWriter(view);

			return new HtmlFormDescriptor(view, url, methodAsString);
		}
	}

	public class HtmlFormDescriptor
	{
		private readonly IUrlResponseWriter _view;
		private readonly string _action;
		private readonly string _method;
		private string _id;

		public HtmlFormDescriptor(IUrlResponseWriter view, string action, string method)
		{
			_view = view;
			_action = action;
			_method = method;
		}

		public string Action
		{
			get { return _action; }
		}

		public string Method
		{
			get { return _method; }
		}

		public IUrlResponseWriter View
		{
			get { return _view; }
		}

		public HtmlFormDescriptor Id(string id)
		{
			_id = id;
			return this;
		}

		public IDisposable Instance
		{
			get
			{
				TagBuilder tagBuilder = new TagBuilder("form");

				tagBuilder.MergeAttribute("action", Action, true);
				tagBuilder.MergeAttribute("method", Method, true);
				if (_id.IsNotEmpty())
				{
					tagBuilder.MergeAttribute(HtmlAttribute.Id, _id, true);
				}
				View.Render(tagBuilder.ToString(TagRenderMode.StartTag));
				return new HtmlFormEndTagWriter(View);
			}
		}
	}

	public class HtmlFormEndTagWriter : IDisposable
	{
		private readonly IResponseWriter _responseWriter;

		public HtmlFormEndTagWriter(IResponseWriter responseWriter)
		{
			_responseWriter = responseWriter;
		}

		public void Dispose()
		{
			_responseWriter.Render("</form>");
		}
	}
}
