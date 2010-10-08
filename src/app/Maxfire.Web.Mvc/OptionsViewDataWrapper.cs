using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using Maxfire.Core.Extensions;
using Maxfire.Web.Mvc.Html.Extensions;

namespace Maxfire.Web.Mvc
{
	/// <summary>
	/// Container for transfer of options (list of options for select, radio button list, checkbox list)
	/// from Controller to View.
	/// </summary>
	public class OptionsViewDataWrapper
	{
		private const string OPTIONS_KEY = "__OptionsContainer__";
		private readonly ViewDataDictionary _viewData;

		public OptionsViewDataWrapper(ViewDataDictionary viewData)
		{
			_viewData = viewData;
		}

		public IEnumerable<SelectListItem> GetOptionsFor<TViewModel, TProperty>(Expression<Func<TViewModel, TProperty>> expression) where TViewModel : class
		{
			string key = expression.GetHtmlFieldNameFor(_viewData);
			return Options.GetValueOrDefault(key);
		}

		public void SetOptionsFor<TViewModel>(Expression<Func<TViewModel, object>> expression, IEnumerable<SelectListItem> options) where TViewModel : class
		{
			string key = expression.GetHtmlFieldNameFor(_viewData);
			Options[key] = options;
		}

		private IDictionary<string, IEnumerable<SelectListItem>> Options
		{
			get
			{
				return _viewData.GetOrCreate(OPTIONS_KEY, () => 
					new Dictionary<string, IEnumerable<SelectListItem>>(StringComparer.OrdinalIgnoreCase)
				) as IDictionary<string, IEnumerable<SelectListItem>>;}
		}
	}
}