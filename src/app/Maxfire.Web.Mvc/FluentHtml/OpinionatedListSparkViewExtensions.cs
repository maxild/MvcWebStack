using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Maxfire.Core.Reflection;

namespace Maxfire.Web.Mvc.FluentHtml
{
	[UsedImplicitly]
	public static class OpinionatedListSparkViewExtensions
	{
		[UsedImplicitly]
		public static string LabelTextFor<TViewModelElement>(this OpinionatedListSparkView<TViewModelElement> view,
		                                                     Expression<Func<TViewModelElement, object>> propertyExpression)
		{
			return propertyExpression.GetDisplayName();
		}
	}
}