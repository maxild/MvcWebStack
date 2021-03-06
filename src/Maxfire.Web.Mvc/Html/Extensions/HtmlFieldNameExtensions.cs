using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using Maxfire.Core.Reflection;

namespace Maxfire.Web.Mvc.Html.Extensions
{
    public static class HtmlFieldNameExtensions
    {
        public static string GetHtmlFieldNameFor<TViewModel, TProperty>(this Expression<Func<TViewModel, TProperty>> expression)
        {
            string htmlFieldName = expression.GetNameFor();
            return htmlFieldName;
        }

        public static string GetHtmlFieldNameFor<TViewModel, TProperty>(this Expression<Func<TViewModel, TProperty>> expression, OpinionatedHtmlHelper<TViewModel> htmlHelper)
            where TViewModel : class
        {
            string htmlFieldName = expression.GetNameFor();
            return htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName);
        }

        public static string GetHtmlFieldNameFor<TViewModel, TProperty>(this Expression<Func<TViewModel, TProperty>> expression, ViewDataDictionary viewData)
        {
            string htmlFieldName = expression.GetNameFor();
            return viewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName);
        }

        public static string GetHtmlFieldIdFor<TViewModel, TProperty>(this Expression<Func<TViewModel, TProperty>> expression)
        {
            return Html401IdUtil.CreateSanitizedId(expression.GetHtmlFieldNameFor());
        }

        public static string GetHtmlFieldIdFor<TViewModel, TProperty>(this Expression<Func<TViewModel, TProperty>> expression, OpinionatedHtmlHelper<TViewModel> htmlHelper)
            where TViewModel : class
        {
            return Html401IdUtil.CreateSanitizedId(expression.GetHtmlFieldNameFor(htmlHelper));
        }

        public static string GetHtmlFieldIdFor<TViewModel, TProperty>(this Expression<Func<TViewModel, TProperty>> expression, ViewDataDictionary viewData)
        {
            return Html401IdUtil.CreateSanitizedId(expression.GetHtmlFieldNameFor(viewData));
        }
    }
}
