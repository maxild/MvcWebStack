using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using Maxfire.Web.Mvc;
using Maxfire.Web.Mvc.FluentHtml;
using Maxfire.Web.Mvc.FluentHtml.Html;

namespace Maxfire.Spark.Web.Mvc
{
	public static class HtmlFormExtensions
	{
		public static HtmlFormEndTagWriter BeginResourceForm<TRestfulController, TEditModel, TInputModel, TId>
			(this OpinionatedResourceSparkView<TEditModel, TInputModel, TId> view, Expression<Func<TEditModel, object>> id)
			where TRestfulController : Controller, IRestfulController<TInputModel, TId>
			where TInputModel : class, IEntityViewModel<TId>
			where TEditModel : EditModelFor<TInputModel>
		{
			string url = view.ViewModel.Input.IsTransient
			             	? view.UrlFor<TRestfulController>(x => x.Create(null)).ToString()
			             	: view.UrlFor<TRestfulController>(x => x.Update(null)).Id(view.ViewModel.Input.Id).ToString();

			// Todo: Create HtmlTag.Form, HtmlAttribute.Action and Method
			var formBuilder = new TagBuilder("form");
			formBuilder.MergeAttribute("action", url);
			formBuilder.MergeAttribute("method", "post");

			view.Render(formBuilder.ToString(TagRenderMode.StartTag));
			view.Render("\n");

			string innerHtml = view.Hidden(id).ToString();
			if (!view.ViewModel.Input.IsTransient)
			{
				var methodBuilder = new TagBuilder(HtmlTag.Input);
				methodBuilder.MergeAttribute(HtmlAttribute.Type, "hidden");
				methodBuilder.MergeAttribute(HtmlAttribute.Name, "_method");
				methodBuilder.MergeAttribute(HtmlAttribute.Value, "PUT");
				innerHtml += methodBuilder.ToString(TagRenderMode.SelfClosing);
			}

			var fieldsetBuilder = new TagBuilder("div");
			fieldsetBuilder.AddCssClass("hidden");
			fieldsetBuilder.InnerHtml = innerHtml;

			view.Render(fieldsetBuilder.ToString(TagRenderMode.Normal));

			return new HtmlFormEndTagWriter(view);
		}
	}
}