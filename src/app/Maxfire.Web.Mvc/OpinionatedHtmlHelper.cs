using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;

namespace Maxfire.Web.Mvc
{
	public class OpinionatedHtmlHelper<TViewModel> : HtmlHelper<TViewModel> where TViewModel: class
	{
		public OpinionatedHtmlHelper(ViewContext viewContext, IViewDataContainer viewDataContainer) : base(viewContext, viewDataContainer)
		{
		}

		public OpinionatedHtmlHelper(ViewContext viewContext, IViewDataContainer viewDataContainer, RouteCollection routeCollection) : base(viewContext, viewDataContainer, routeCollection)
		{
		}

		public virtual IEnumerable<SelectListItem> GetOptionsFor<TProperty>(Expression<Func<TViewModel, TProperty>> expression)
		{
			return OptionsWrapper.GetOptionsFor(expression) ?? GetOptionsOfType<TProperty>();
		}

		private OptionsViewDataWrapper _optionsWrapper;
		protected OptionsViewDataWrapper OptionsWrapper
		{
			get { return _optionsWrapper ?? (_optionsWrapper = new OptionsViewDataWrapper(ViewData)); }
		}

		private static IEnumerable<SelectListItem> GetOptionsOfType<T>()
		{
			if (typeof(T) == typeof(bool) || typeof(T) == typeof(bool?))
			{
				return OptionsAdapter2.Boolean();
			}
			if (typeof(T).IsEnum)
			{
				return OptionsAdapter2.FromEnumTexts<T>();
			}

			return null;
		}
	}
}